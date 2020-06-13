using Microsoft.Extensions.DependencyInjection;
using OperatorSharp;
using OperatorSharp.CustomResources;
using System;
using System.Collections.Generic;
using System.Text;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class OperatorDependencyInjectionExtensions
    {
        public static ServiceCollection AddOperator<TResource, TOperator>(this ServiceCollection services) where TResource : CustomResource
            where TOperator : Operator<TResource>
        {
            services.AddScoped<IEventRecorder<TResource>, EventRecorder<TResource>>();
            services.AddScoped<TOperator>();

            return services;
        }
    }
}
