using System;
using BestHTTP.SignalRCore;
namespace Basra.Client
{
    public class ReconnectPolicy : IRetryPolicy
    {
        static int MaxRetries = 20;
        int Retries;

        public TimeSpan? GetNextRetryDelay(RetryContext context)
        {
            Retries++;

            if (Retries > MaxRetries)
                return null;
            else
                return TimeSpan.FromSeconds(2);
        }
    }
}