using System;
using System.Runtime.Serialization;

namespace OperatorSharp
{
    [Serializable]
    internal class OperatorStartupException : Exception
    {
        public OperatorStartupException()
        {
        }

        public OperatorStartupException(string message) : base(message)
        {
        }

        public OperatorStartupException(string message, Exception innerException) : base(message, innerException)
        {
        }

        protected OperatorStartupException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}