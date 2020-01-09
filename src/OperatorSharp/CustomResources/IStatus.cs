using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace OperatorSharp.CustomResources
{
    public interface IStatus
    {
        [JsonProperty("observedGeneration")]
        public int ObservedGeneration { get; set; }
    }
}
