using System;
using System.Collections.Generic;
using System.Text;

namespace OperatorSharp.Tools.DotNet
{
    public static class InterfaceExtensions
    {
        public static bool Implements<I>(this Type source)
        {
            return typeof(I).IsAssignableFrom(source);
        }
    }
}
