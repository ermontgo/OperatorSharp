using System.Text.Json.Serialization;

namespace OperatorSharp.CustomResources
{
    public class CustomResourceStatus
    {
        [JsonPropertyName("observedGeneration")]
        public long? ObservedGeneration { get; set; }
    }
}
