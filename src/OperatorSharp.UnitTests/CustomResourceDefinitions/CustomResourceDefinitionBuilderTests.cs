using k8s;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using OperatorSharp.Tools.DotNet;
using OperatorSharp.UnitTests.Examples;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace OperatorSharp.UnitTests.CustomResourceDefinitions
{
    [TestClass]
    public class CustomResourceDefinitionBuilderTests
    {
        [TestMethod]
        public void CustomResourceDefinition_GeneratesSchema()
        {
            var builder = new CustomResourceDefinitionBuilder();

            var definition = builder.BuildDefinition(typeof(ExampleResource));

            var yamlCrd = Yaml.SaveToString(definition);

            Assert.IsTrue(definition.Spec.Versions.First().Schema.OpenAPIV3Schema.Type != null);
        }
    }
}
