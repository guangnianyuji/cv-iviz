using System;

namespace Iviz.MsgsGen
{
    public class MessageGenException : Exception
    {
        protected MessageGenException()
        {
        }

        protected MessageGenException(string message) : base(message)
        {
        }

        protected MessageGenException(string message, Exception innerException) : base(message, innerException)
        {
        }
    }

    public class MessageParseException : MessageGenException
    {
        public MessageParseException()
        {
        }

        public MessageParseException(string message) : base(message)
        {
        }

        public MessageParseException(string message, Exception innerException) : base(message, innerException)
        {
        }
    }

    public class MessageStructException : MessageGenException
    {
        public MessageStructException() : base(
            "The selected message cannot be a struct because it contains a string, an array, or a non-struct field")
        {
        }

        public MessageStructException(string message) : base(message)
        {
        }

        public MessageStructException(string message, Exception innerException) : base(message, innerException)
        {
        }
    }
    
    public class MessageDependencyException : MessageGenException
    {
        public MessageDependencyException(string message) : base(message)
        {
        }

        public MessageDependencyException(string message, Exception innerException) : base(message, innerException)
        {
        }
    }    
}