using Microsoft.AspNetCore.SignalR;

namespace Basra.Server.Exceptions
{
    [System.Serializable]
    public class BadUserInputException : HubException
    {
        public BadUserInputException() : base()
        {
        }

        public BadUserInputException(string message) : base(message)
        {
        }
    }
}