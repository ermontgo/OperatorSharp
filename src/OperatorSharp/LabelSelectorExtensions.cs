using System;
using System.Linq;
using k8s.Models;

namespace OperatorSharp
{
    public static class LabelSelectorExtensions
    {
        public static string BuildSelector(this V1LabelSelector selector)
        {
            return string.Join(",", selector.MatchLabels.Select(m => $"{m.Key}={m.Value}"));
        }
    }
}