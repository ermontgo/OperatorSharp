using System;
using System.Threading;
using System.Threading.Tasks;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using k8s;
using OperatorSharp.CustomResources.Metadata;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using OperatorSharp.Filters;
using k8s.Autorest;

namespace OperatorSharp
{
    public abstract class Operator 
    {
        public abstract Task WatchAsync(CancellationToken token, string watchedNamespace);
    }

    public abstract class Operator<TCustomResource> : Operator
        where TCustomResource: CustomResources.CustomResource
    {

        public Operator(IKubernetes client, ILogger<Operator<TCustomResource>> logger)
        {
            Client = client;
            Logger = logger;
        }

        protected IKubernetes Client { get; private set; }
        public ILogger<Operator<TCustomResource>> Logger { get; }

        private TaskCompletionSource<bool> tcs;

        protected readonly List<IOperatorFilter<TCustomResource>> Filters = new List<IOperatorFilter<TCustomResource>>();

        public static ApiVersion ApiVersion => GetAttribute<TCustomResource, ApiVersionAttribute>().ApiVersion;
        public static string PluralName => GetAttribute<TCustomResource, PluralNameAttribute>().PluralName;

        public abstract void HandleItem(WatchEventType eventType, TCustomResource item);

        public void OnHandleItem(WatchEventType eventType, TCustomResource item)
        {
            bool continuePipeline = true;
            foreach (var filter in Filters)
            {
                continuePipeline &= filter.Execute(eventType, item);

                if (!continuePipeline)
                {
                    Logger.LogWarning("Message {type}:{name} ({version}) stopped processing due to results of {filter} filter", item.Kind, item.Metadata.Name, item.Metadata.ResourceVersion, filter.GetType().Name);
                    break;
                }
            }

            if (continuePipeline) HandleItem(eventType, item);
        }
        
        public abstract void HandleException(Exception ex);
        public void OnHandleException(Exception ex)
        {
            try
            {
                HandleException(ex);
            }
            catch (Exception thrownEx)
            {
                tcs.SetException(thrownEx);
            }
        }

        public override Task WatchAsync(CancellationToken token, string watchedNamespace)
        {
            try 
            {
                Logger.LogDebug("Starting operator for {operator} operator", GetType().Name);
                string plural = PluralName.ToLower();

                tcs = new TaskCompletionSource<bool>();

                Logger.LogDebug("Initiating watch for {resource}", plural);
                Task<HttpOperationResponse<object>> result = null;
                if (GetAttribute<TCustomResource, ResourceScopeAttribute>().ResourceScope == ResourceScopes.Namespaced)
                {
                    Logger.LogInformation("Watching {plural} resource in {namespace} namespace", plural, watchedNamespace);
                    result = Client.CustomObjects.ListNamespacedCustomObjectWithHttpMessagesAsync(
                        ApiVersion.Group, ApiVersion.Version, watchedNamespace, plural, watch: true, timeoutSeconds: 30000, cancellationToken: token
                    );
                }
                else
                {
                    Logger.LogInformation("Watching {plural} resource in cluster", plural, watchedNamespace);
                    result = Client.CustomObjects.ListClusterCustomObjectWithHttpMessagesAsync(ApiVersion.Group, ApiVersion.Version, plural, watch: true, timeoutSeconds: 30000, cancellationToken: token);
                }
                result.ConfigureAwait(false);

                result.Watch<TCustomResource, object>((type, item) => OnHandleItem(type, item), (ex) => OnHandleException(ex), () => OnClosed());

                return Task.WhenAll(result, tcs.Task);
            }
            catch (Exception ex) {
                HandleException(ex);
                throw new OperatorStartupException("An error occurred while initializing the operator", ex);
            }
        }

        public virtual void OnClosed()
        {
            tcs.SetResult(true);
            Logger.LogWarning("Operator {type} closed", this.GetType().ToString());
        }

        public static TAttribute GetAttribute<TResource, TAttribute>() where TAttribute: Attribute 
        {
            TAttribute attribute = Attribute.GetCustomAttribute(typeof(TResource), typeof(TAttribute)) as TAttribute;

            return attribute;
        }
    }
}