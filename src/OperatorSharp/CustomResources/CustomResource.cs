using System;
using System.Text.Json.Serialization;
using k8s;
using k8s.Models;
using OperatorSharp.CustomResources.Metadata;

namespace OperatorSharp.CustomResources
{
    // Adapted from https://github.com/engineerd/kubecontroller-csharp
    public abstract class CustomResource : KubernetesObject
    {
        [JsonPropertyName("metadata")]
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
        [JsonPropertyName("spec")]
        public TSpec Spec { get; set; }
    }

    public abstract class CustomResource<TSpec, TStatus> : CustomResource<TSpec>, IStatusEnabledCustomResource, IStatusEnabledCustomResource<TStatus>
        where TStatus: CustomResourceStatus
    {
        [JsonPropertyName("status")]
        public TStatus Status { get; set; }
    }
}
