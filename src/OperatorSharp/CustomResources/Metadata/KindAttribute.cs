using System;

namespace OperatorSharp.CustomResources.Metadata
{
    [System.AttributeUsage(System.AttributeTargets.Class, Inherited = false, AllowMultiple = false)]
    public class KindAttribute : Attribute
    {
        public KindAttribute(string kind)
        {
            Kind = kind;
        }

        public string Kind { get; }
    }
}