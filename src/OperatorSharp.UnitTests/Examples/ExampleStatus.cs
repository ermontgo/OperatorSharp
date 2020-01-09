using OperatorSharp.CustomResources;

namespace OperatorSharp.UnitTests.Examples
{
    internal class ExampleStatus : IStatus
    {
        public int ObservedGeneration { get; set; }
    }
}