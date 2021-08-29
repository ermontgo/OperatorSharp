using k8s;
using k8s.Models;
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
            var builder = new V1CustomResourceDefinitionBuilder();

            var definition = builder.BuildDefinition(typeof(ExampleResource));

            var yamlCrd = Yaml.SaveToString(definition);

            if (definition.Crd is V1CustomResourceDefinition crd)
            {
                Assert.IsTrue(crd.Spec.Versions.First().Schema.OpenAPIV3Schema.Type != null);
            }
            else Assert.Fail("The CRD returned was not of the correct type");
        }
    }
}
