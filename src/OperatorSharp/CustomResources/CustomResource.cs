using System;
using k8s;
using k8s.Models;
using Newtonsoft.Json;
using OperatorSharp.CustomResources.Metadata;

namespace OperatorSharp.CustomResources
{
    // Adapted from https://github.com/engineerd/kubecontroller-csharp
    public abstract class CustomResource : KubernetesObject
    {
        [JsonProperty(PropertyName = "metadata")]
        public V1ObjectMeta Metadata { get; set; }

        public ApiVersion ApiVersionMetadata => GetAttribute<ApiVersionAttribute>().ApiVersion;
        public string PluralName => GetAttribute<PluralNameAttribute>().PluralName;

        public TAttribute GetAttribute<TAttribute>() where TAttribute : Attribute
        {
            TAttribute attribute = Attribute.GetCustomAttribute(GetType(), typeof(TAttribute)) as TAttribute;

            return attribute;
        }
    }

    public abstract class CustomResource<TSpec> : CustomResource, IMetadata<V1ObjectMeta>
    {
        [JsonProperty(PropertyName = "spec")]
        public TSpec Spec { get; set; }
    }

    public abstract class CustomResource<TSpec, TStatus> : CustomResource<TSpec>, IStatusEnabledCustomResource, IStatusEnabledCustomResource<TStatus>
        where TStatus: CustomResourceStatus
    {
        [JsonProperty(PropertyName = "status")]
        public TStatus Status { get; set; }
    }
}
