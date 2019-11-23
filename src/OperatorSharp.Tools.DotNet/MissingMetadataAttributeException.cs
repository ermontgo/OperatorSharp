using System;
using System.Runtime.Serialization;

namespace OperatorSharp.Tools.DotNet
{
    [Serializable]
    internal class MissingMetadataAttributeException : Exception
    {
        public MissingMetadataAttributeException()
        {
        }

        public MissingMetadataAttributeException(string message) : base(message)
        {
        }

        public MissingMetadataAttributeException(string message, Exception innerException) : base(message, innerException)
        {
        }

        protected MissingMetadataAttributeException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}