using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using OperatorSharp.Filters;
using OperatorSharp.UnitTests.Examples;
using System;
using System.Collections.Generic;
using System.Text;

namespace OperatorSharp.UnitTests
{
    [TestClass]
    public class OperatorTests
    {
        [TestMethod]
        public void WhenBlockingFiltersAreAdded_HandledItemsIs0()
        {
            var logger = NullLogger<ExampleOperator>.Instance;
            ExampleOperator @operator = new ExampleOperator(null, logger);
            @operator.AddFilter(new IgnoreStatusUpdatesOperatorFilter<ExampleResource, ExampleStatus>());

            var message = new ExampleResource() { Metadata = new k8s.Models.V1ObjectMeta { Generation = 1 }, Status = new ExampleStatus { ObservedGeneration = 1 } };
            @operator.OnHandleItem(k8s.WatchEventType.Added, message);

            Assert.AreEqual(0, @operator.HandledItems);
        }

        [TestMethod]
        public void WhenNoFiltersAreAdded_HandledItemsIs1()
        {
            var logger = NullLogger<ExampleOperator>.Instance;
            ExampleOperator @operator = new ExampleOperator(null, logger);

            var message = new ExampleResource() { Metadata = new k8s.Models.V1ObjectMeta { Generation = 1 }, Status = new ExampleStatus { ObservedGeneration = 1 } };
            @operator.OnHandleItem(k8s.WatchEventType.Added, message);

            Assert.AreEqual(1, @operator.HandledItems);
        }

        [TestMethod]
        public void WhenPassingFiltersAreAdded_HandledItemsIs1()
        {
            var logger = NullLogger<ExampleOperator>.Instance;
            ExampleOperator @operator = new ExampleOperator(null, logger);
            @operator.AddFilter(new IgnoreStatusUpdatesOperatorFilter<ExampleResource, ExampleStatus>());

            var message = new ExampleResource() { Metadata = new k8s.Models.V1ObjectMeta { Generation = 2 }, Status = new ExampleStatus { ObservedGeneration = 1 } };
            @operator.OnHandleItem(k8s.WatchEventType.Added, message);

            Assert.AreEqual(1, @operator.HandledItems);
        }
    }
}
