using k8s;
using System;

namespace OperatorSharp.Tools.DotNet
{
    public interface ICustomResourceDefinitionBuilder
    {
        CustomResourceDefinitionContext BuildDefinition(Type resourceType);
    }
}