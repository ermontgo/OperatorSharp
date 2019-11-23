using System;
using k8s;
using k8s.Models;
using Newtonsoft.Json;

namespace OperatorSharp.CustomResources
{
    // Adapted from https://github.com/engineerd/kubecontroller-csharp
    public abstract class CustomResource : KubernetesObject 
    {
        [JsonProperty(PropertyName = "metadata")]
        public V1ObjectMeta Metadata { get; set; }
    }

    public abstract class CustomResource<TSpec> : CustomResource
    {
        [JsonProperty(PropertyName = "spec")]
        public TSpec Spec { get; set; }
    }

    public abstract class CustomResource<TSpec, TStatus> : CustomResource<TSpec>, IStatusEnabledCustomResource
    {
        [JsonProperty(PropertyName = "status")]
        public TStatus Status { get; set; }
    }
}
