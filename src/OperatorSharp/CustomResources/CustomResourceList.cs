using System;
using System.Collections.Generic;

namespace OperatorSharp.CustomResources
{
    public class CustomResourceList<TCustomResource> : CustomResource 
        where TCustomResource: CustomResource 
    {
        public List<TCustomResource> Items { get; set; }
    }
}