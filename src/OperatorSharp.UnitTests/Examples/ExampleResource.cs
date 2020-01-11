using OperatorSharp.CustomResources;
using OperatorSharp.CustomResources.Metadata;
using System;
using System.Collections.Generic;
using System.Text;

namespace OperatorSharp.UnitTests.Examples
{
    [ApiVersion("example.com/v1alpha1")]
    [Kind("Example")]
    [PluralName("examples")]
    [ShortName("ex")]
    [ResourceScope(ResourceScopes.Namespaced)]
    internal class ExampleResource : CustomResource<ExampleSpec, ExampleStatus>
    {
    }
}
