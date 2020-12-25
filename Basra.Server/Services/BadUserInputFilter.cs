using System;
using System.Threading.Tasks;
using Basra.Server.Exceptions;
using Microsoft.AspNetCore.SignalR;

namespace Basra.Server.Services
{
    public class BadUserInputFilter : IHubFilter
    {
        public async ValueTask<object> InvokeMethodAsync(HubInvocationContext invocationContext, Func<HubInvocationContext, ValueTask<object>> next)
        {
            //HubException from here can terminate the invokation before calling the hub method or any next, and the clietn will recieve an error

            //practical: you can add custom attribute on the method and fetch it's data from here
            //E.G:
            // var languageFilter = (LanguageFilterAttribute)Attribute.GetCustomAttribute(
            //             invocationContext.HubMethod, typeof(LanguageFilterAttribute));
            // if (languageFilter != null &&
            //     invocationContext.HubMethodArguments.Count > languageFilter.FilterArgument &&
            //     invocationContext.HubMethodArguments[languageFilter.FilterArgument] is string str)

            Console.WriteLine($"Calling hub method '{invocationContext.HubMethodName}'");
            if (invocationContext.HubMethodArguments.Count != 0)
            {
                var stringArgs = "with args: ";
                foreach (var arg in invocationContext.HubMethodArguments)
                {
                    stringArgs += arg.ToString() + "  --  ";
                }
                Console.WriteLine(stringArgs);
            }

            // try
            // {
            return await next(invocationContext);
            //invokes the next filter. And if it's the last filter, invokes the hub method
            // }
            // catch (BadUserInputException)
            // {
            //     // throw new HubException();
            //     // Console.WriteLine("BadUserInputException happened");
            //     return new ValueTask<object>("error 0");
            // }
            // catch (Exception ex)
            // {
            //     Console.WriteLine($"Exception calling '{invocationContext.HubMethodName}': {ex}");
            //     throw;
            // }
        }

        // Optional method
        public Task OnConnectedAsync(HubLifetimeContext context, Func<HubLifetimeContext, Task> next)
        {
            return next(context);
        }

        // Optional method
        public Task OnDisconnectedAsync(HubLifetimeContext context, Exception exception, Func<HubLifetimeContext, Exception, Task> next)
        {
            return next(context, exception);
        }
    }
}