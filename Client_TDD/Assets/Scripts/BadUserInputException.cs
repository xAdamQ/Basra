using System;

[Serializable]
public class BadUserInputException : Exception
{
    public BadUserInputException()
    {
    }
    public BadUserInputException(string message) : base(message)
    {
    }
    // public BadUserInputException(string message, System.Exception inner) : base(message, inner) { }
    // public BadUserInputException(System.Runtime.Serialization.SerializationInfo info, System.Runtime.Serialization.StreamingContext context) : base(info, context) { }
}