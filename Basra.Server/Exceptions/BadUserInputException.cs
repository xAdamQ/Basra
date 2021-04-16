using Microsoft.AspNetCore.SignalR;

namespace Basra.Server.Exceptions
{
    [System.Serializable]
    public class BadUserInputException : HubException
    {
        // public BadUserInputException() : base() { }
        // public BadUserInputException(string message) : base(message) { }
        // public BadUserInputException(string message, System.Exception inner) : base(message, inner) { }
        // public BadUserInputException(System.Runtime.Serialization.SerializationInfo info, System.Runtime.Serialization.StreamingContext context) : base(info, context) { }
    }
}