using k8s;
using Microsoft.Extensions.Logging;
using OperatorSharp.Filters;
using System;
using System.Collections.Generic;
using System.Text;

namespace OperatorSharp.UnitTests.Examples
{
    internal class ExampleOperator : Operator<ExampleResource>
    {
        public ExampleOperator(IKubernetes client, ILogger<Operator<ExampleResource>> logger) : base(client, logger)
        {
        }

        public int HandledItems { get; set; }

        public override void HandleException(Exception ex)
        {
            throw new NotImplementedException();
        }

        public override void HandleItem(WatchEventType eventType, ExampleResource item)
        {
            HandledItems++;
        }

        public void AddFilter(IOperatorFilter<ExampleResource> filter)
        {
            Filters.Add(filter);
        }
    }
}
