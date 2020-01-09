using k8s;
using OperatorSharp.CustomResources;
using System;
using System.Collections.Generic;
using System.Text;

namespace OperatorSharp.Filters
{
    public class IgnoreMessageOperatorFilter<TCustomResource> : IOperatorFilter<TCustomResource>
        where TCustomResource : CustomResource
    {
        private readonly Predicate<TCustomResource> predicate;

        public IgnoreMessageOperatorFilter(Predicate<TCustomResource> predicate)
        {
            this.predicate = predicate;
        }

        public bool Execute(WatchEventType eventType, TCustomResource resource)
        {
            return predicate(resource);
        }
    }
}
