using System;

namespace OperatorSharp.CustomResources.Metadata
{
    [System.AttributeUsage(System.AttributeTargets.Class, Inherited = false, AllowMultiple = false)]
    public class ResourceScopeAttribute : System.Attribute
    {        
        public ResourceScopeAttribute(ResourceScopes resourceScope)
        {
            ResourceScope = resourceScope;
        }
        
        public ResourceScopes ResourceScope { get; }
    }
}