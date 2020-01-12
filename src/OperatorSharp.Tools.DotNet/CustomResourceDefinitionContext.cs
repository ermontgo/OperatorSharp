using k8s;
using System;
using System.Collections.Generic;
using System.Text;

namespace OperatorSharp.Tools.DotNet
{
    public abstract class CustomResourceDefinitionContext
    {
        public string Name { get; set; }

        public IKubernetesObject Crd { get; protected set; }

        public abstract string ToYaml();
    }

    public class CustomResourceDefinitionContext<TResult> : CustomResourceDefinitionContext where TResult: class, IKubernetesObject
    {
        public CustomResourceDefinitionContext(string name, TResult crd)
        {
            Name = name;
            Crd = crd;
        }

        public override string ToYaml()
        {
            return Yaml.SaveToString(Crd as TResult);
        }
    }
}
