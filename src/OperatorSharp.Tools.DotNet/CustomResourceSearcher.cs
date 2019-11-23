using OperatorSharp.CustomResources;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace OperatorSharp.Tools.DotNet
{
    public class CustomResourceSearcher
    {
        public IEnumerable<Type> FindCustomResourceTypes(Assembly assembly)
        {
            return assembly.GetTypes().Where(t => t.IsSubclassOf(typeof(CustomResource)));
        }
    }
}
