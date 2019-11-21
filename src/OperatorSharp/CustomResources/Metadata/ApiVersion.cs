using System;

namespace OperatorSharp.CustomResources.Metadata
{
    public class ApiVersion 
    {
        public ApiVersion(string apiVersion) {
            var parts = apiVersion.Split('/');

            Group = parts[0];
            Version = parts[1];
        }

        public string Group { get; internal set; }
        public string Version { get; internal set; }

        public override string ToString()
        {
            return $"{Group}/{Version}";
        }
    }
}