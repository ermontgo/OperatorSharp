using OperatorSharp.CustomResources;

namespace OperatorSharp.UnitTests.Examples
{
    internal class ExampleStatus : CustomResourceStatus
    {
        public string Reason { get; set; }

        public string Message { get; set; }
    }
}