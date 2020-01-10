using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace OperatorSharp.CustomResources
{
    public class CustomResourceStatus
    {
        [JsonProperty("observedGeneration")]
        public long? ObservedGeneration { get; set; }
    }
}
