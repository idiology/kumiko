using Daifuku.Exceptions;
using Daifuku.Validations;
using System;
using System.Collections.Generic;

namespace Library.Exceptions
{
    public class AppException : Exception, IApplicationException
    {
        public IEnumerable<ValidationError> Messages { get; }

        public AppException()
        {
        }

        public AppException(string message) : base(message)
        {
        }

        public AppException(IEnumerable<ValidationError> messages)
        {
            Messages = messages;
        }

        public AppException(string message, Exception innerException) : base(message, innerException)
        {
        }

        protected AppException(System.Runtime.Serialization.SerializationInfo info, System.Runtime.Serialization.StreamingContext context) : base(info, context)
        {
        }
    }
}