using System;

namespace OperatorSharp.CustomResources.Metadata
{
    [System.AttributeUsage(System.AttributeTargets.Class, Inherited = false, AllowMultiple = false)]
    public class PluralNameAttribute : Attribute
    {
        public PluralNameAttribute(string pluralName)
        {
            PluralName = pluralName;
        }

        public string PluralName { get; }
    }
}