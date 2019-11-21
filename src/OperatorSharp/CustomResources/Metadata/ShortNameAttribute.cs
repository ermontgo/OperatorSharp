using System;

namespace OperatorSharp.CustomResources.Metadata
{
    [System.AttributeUsage(System.AttributeTargets.Class, Inherited = false, AllowMultiple = false)]
    public class ShortNameAttribute : Attribute
    {
        public ShortNameAttribute(string shortName)
        {
            ShortName = shortName;
        }

        public string ShortName { get; }
    }
}