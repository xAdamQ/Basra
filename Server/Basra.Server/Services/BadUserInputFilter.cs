using System;
using System.Threading.Tasks;
using Basra.Server.Exceptions;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Logging;

namespace Basra.Server.Services
{
    public class BadUserInputFilter : IHubFilter
    {
        private readonly MasterHub.MethodDomains _methodDomains;
        private readonly ISessionRepo _sessionRepo;
        private readonly ILogger<BadUserInputFilter> _logger;
        private readonly IRequestCache _requestCache;
        public BadUserInputFilter(MasterHub.MethodDomains methodDomains, ISessionRepo sessionRepo,
            ILogger<BadUserInputFilter> logger, IRequestCache requestCache)
        {
            _methodDomains = methodDomains;
            _sessionRepo = sessionRepo;
            _logger = logger;
            _requestCache = requestCache;
        }

        public async ValueTask<object> InvokeMethodAsync(HubInvocationContext invocationContext,
            Func<HubInvocationContext, ValueTask<object>> next)
        {
            //HubException from here can terminate the invokation before calling the hub method or any next, and the clietn will recieve an error

            //practical: you can add custom attribute on the method and fetch it's data from here
            //E.G:
            // var languageFilter = (LanguageFilterAttribute)Attribute.GetCustomAttribute(
            //             invocationContext.HubMethod, typeof(LanguageFilterAttribute));
            // if (languageFilter != null &&
            //     invocationContext.HubMethodArguments.Count > languageFilter.FilterArgument &&
            //     invocationContext.HubMethodArguments[languageFilter.FilterArgument] is string str)

            _logger.LogInformation(
                $"Calling hub method '{invocationContext.HubMethodName}'" +
                $" with args {string.Join(", ", invocationContext.HubMethodArguments)}");


            // todo can the active user be null in this stage? seems not
            var activeUser = _sessionRepo.GetActiveUser(invocationContext.Context.UserIdentifier);
            var domain = _methodDomains.GetDomain(invocationContext.HubMethodName);

            if (domain == null)
            {
                throw new BadUserInputException(
                    "the user is invoking a function that doesn't exist or it's not an rpc");
            }
            if (!activeUser.Domain.IsSubclassOf(domain) && !activeUser.Domain.IsEquivalentTo(domain))
            {
                throw new BadUserInputException(
                    $"the called function with domain {domain} is not valid in the current user domain {activeUser.Domain}");
            }

            _requestCache.Init(invocationContext.Context.UserIdentifier);

            try
            {
                return await next(invocationContext);
                // invokes the next filter. And if it's the last filter, invokes the hub method
            }
            catch (BadUserInputException)
            {
                _logger.LogInformation("BadUserInputException happened");

                throw;
                // return new ValueTask<int>(1);
                // return new ValueTask<User>(new User {Name = "test name on the returned user"});
                // return new ValueTask<object>($"there's a buie exc on the server {e.Message}");
            }
            catch (Exception ex)
            {
                _logger.LogInformation($"Exception calling '{invocationContext.HubMethodName}': {ex}");
                throw;
            }
        }

        // Optional method
        public Task OnConnectedAsync(HubLifetimeContext context, Func<HubLifetimeContext, Task> next)
        {
            return next(context);
        }

        // Optional method
        public Task OnDisconnectedAsync(HubLifetimeContext context, Exception exception,
            Func<HubLifetimeContext, Exception, Task> next)
        {
            return next(context, exception);
        }
    }
}