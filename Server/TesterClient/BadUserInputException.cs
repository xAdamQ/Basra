using System;

namespace Basra.Server.Exceptions
{
    [Serializable]
    public class BadUserInputException : Exception
    {
        public BadUserInputException() : base()
        {
        }
        public BadUserInputException(string message) : base(message)
        {
        }
        // public BadUserInputException(string message, System.Exception inner) : base(message, inner) { }
        // public BadUserInputException(System.Runtime.Serialization.SerializationInfo info, System.Runtime.Serialization.StreamingContext context) : base(info, context) { }
    }
}