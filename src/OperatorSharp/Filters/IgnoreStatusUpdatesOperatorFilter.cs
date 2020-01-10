using OperatorSharp.CustomResources;
using System;
using System.Collections.Generic;
using System.Text;

namespace OperatorSharp.Filters
{
    public class IgnoreStatusUpdatesOperatorFilter<TCustomResource, TStatus> : IgnoreMessageOperatorFilter<TCustomResource>
        where TCustomResource : CustomResource, IStatusEnabledCustomResource<TStatus>
        where TStatus: CustomResourceStatus
    {
        private static Predicate<TCustomResource> IgnoreStatusUpdatesPredicate = message =>
        {
            return message.Metadata.Generation != message.Status.ObservedGeneration;
        };
        
        public IgnoreStatusUpdatesOperatorFilter() : base(IgnoreStatusUpdatesPredicate)
        {
        }
    }
}
