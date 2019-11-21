using System;
using System.Threading;
using System.Threading.Tasks;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using k8s;
using OperatorSharp.CustomResources.Metadata;
using Microsoft.Extensions.Logging;

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

        public static ApiVersion ApiVersion => GetAttribute<TCustomResource, ApiVersionAttribute>().ApiVersion;
        public static string PluralName => GetAttribute<TCustomResource, PluralNameAttribute>().PluralName;

        public abstract void HandleItem(WatchEventType eventType, TCustomResource item);

        public abstract void HandleException(Exception ex);

        public override Task WatchAsync(CancellationToken token, string watchedNamespace)
        {
            try 
            {
                Logger.LogDebug("Starting operator for {operator} operator", GetType().Name);
                string plural = PluralName;

                Logger.LogDebug("Initiating watch for {resource}", plural);
                var result = Client.ListNamespacedCustomObjectWithHttpMessagesAsync(
                    ApiVersion.Group, ApiVersion.Version, watchedNamespace, plural, watch: true, timeoutSeconds: 30000, cancellationToken: token
                );

                Logger.LogInformation("Watching {plural} resource in {namespace} namespace", plural, watchedNamespace);
                result.Watch<TCustomResource, object>((type, item) => HandleItem(type, item), (ex) => HandleException(ex));

                return result;
            }
            catch (Exception ex) {
                HandleException(ex);
                throw new OperatorStartupException("An error occurred while initializing the operator", ex);
            }
        }

        public static TAttribute GetAttribute<TResource, TAttribute>() where TAttribute: Attribute 
        {
            TAttribute attribute = Attribute.GetCustomAttribute(typeof(TResource), typeof(TAttribute)) as TAttribute;

            return attribute;
        }
    }
}