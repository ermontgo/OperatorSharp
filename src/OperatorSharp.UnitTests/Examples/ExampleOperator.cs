using k8s;
using Microsoft.Extensions.Logging;
using OperatorSharp.Filters;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace OperatorSharp.UnitTests.Examples
{
    internal class ExampleOperator : Operator<ExampleResource>
    {
        public ExampleOperator(IKubernetes client, ILogger<Operator<ExampleResource>> logger) : base(client, logger)
        {
        }

        public int HandledItems { get; set; }

        public override async Task HandleException(Exception ex)
        {
            throw new NotImplementedException();
        }

        public override async Task HandleItem(WatchEventType eventType, ExampleResource item)
        {
            HandledItems++;
        }

        public void AddFilter(IOperatorFilter<ExampleResource> filter)
        {
            Filters.Add(filter);
        }
    }
}
