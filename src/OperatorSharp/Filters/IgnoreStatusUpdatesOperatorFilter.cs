using OperatorSharp.CustomResources;
using System;
using System.Collections.Generic;
using System.Text;

namespace OperatorSharp.Filters
{
    public class IgnoreStatusUpdatesOperatorFilter<TCustomResource, TStatus> : IgnoreMessageOperatorFilter<TCustomResource>
        where TCustomResource : CustomResource, IStatusEnabledCustomResource<TStatus>
        where TStatus: IStatus
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
