using System;
using System.Text;
using Microsoft.Extensions.Logging;

namespace TesterClient
{
    public class ConsoleLogger : ILogger
    {
        public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception exception,
            Func<TState, Exception, string> formatter)
        {
            var sb = new StringBuilder();

            sb.Append(logLevel.ToString())
                .Append(formatter(state, exception));

            if (exception != null)
            {
                sb.Append('\n').Append(exception);
            }

            Console.WriteLine(sb.ToString());
        }
        public bool IsEnabled(LogLevel logLevel)
        {
            throw new NotImplementedException();
        }
        public IDisposable BeginScope<TState>(TState state)
        {
            throw new NotImplementedException();
        }
    }
}