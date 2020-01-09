using k8s;
using OperatorSharp.CustomResources;
using System;
using System.Collections.Generic;
using System.Text;

namespace OperatorSharp.Filters
{
    public interface IOperatorFilter<TCustomResource> where TCustomResource : CustomResource
    {
        bool Execute(WatchEventType eventType, TCustomResource resource);
    }
}
