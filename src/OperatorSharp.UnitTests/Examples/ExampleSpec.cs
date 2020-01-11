namespace OperatorSharp.UnitTests.Examples
{
    internal class ExampleSpec
    {
        public string FirstName { get; set; }

        public string LastName { get; set; }

        public int Age { get; set; }

        public string[] Roles { get; set; }

        public k8s.Models.V1LabelSelector Selector { get; set; }
    }
}