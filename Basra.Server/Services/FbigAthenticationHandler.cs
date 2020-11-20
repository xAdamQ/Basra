using Basra.Server.Models;
using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Security.Claims;
using System.Security.Principal;
using System.Text.Encodings.Web;
using System.Threading.Tasks;

namespace Basra.Server.Services
{
    //for scheme
    public class FbigAuthenticationSchemeOptions : AuthenticationSchemeOptions { }

    public class FbigAuthenticationHandler : AuthenticationHandler<FbigAuthenticationSchemeOptions>
    {
        public const string PROVIDER_NAME = "Fbig";

        private readonly FbigSecurityManager _fbigSecurityManager;

        public FbigAuthenticationHandler(
            IOptionsMonitor<FbigAuthenticationSchemeOptions> options,
            ILoggerFactory logger,
            UrlEncoder encoder,
            ISystemClock clock,
            FbigSecurityManager fbigSecurityManager)
        : base(options, logger, encoder, clock)
        {
            _fbigSecurityManager = fbigSecurityManager;
        }


        /// <summary>
        /// gets string after 6 chars (bearer)
        /// exceptions: token header doesn't exist - does't have 6 chars (for bearer {so it can be any word with 6 char!})
        /// exceptions caught on higher level, you can identify them to avoid your faults
        /// look at the disabled function
        /// </summary>
        private string GetAuthorizationHeader()
        {
            string authorizationHeader = Request.Headers["Authorization"];
            return authorizationHeader.Substring("bearer".Length).Trim();
        }
        /*old authorization header function

                    #region validate token header structure
                    if (!Request.Headers.ContainsKey("Authorization"))
                    {
                        return AuthenticateResult.Fail("Unauthorized");
                    }

                    string authorizationHeader = Request.Headers["Authorization"];
                    if (string.IsNullOrEmpty(authorizationHeader))
                    {
                        return AuthenticateResult.NoResult();
                    }

                    if (!authorizationHeader.StartsWith("bearer", StringComparison.OrdinalIgnoreCase))
                    {
                        return AuthenticateResult.Fail("Unauthorized");
                    }

                    string token = authorizationHeader.Substring("bearer".Length).Trim();

                    if (string.IsNullOrEmpty(token))
                    {
                        return AuthenticateResult.Fail("Unauthorized");
                    }
                    #endregion

        */

        private string GetAccessTokenQueryParam()
        {
            return Request.Query["access_token"];
        }

        protected override async Task<AuthenticateResult> HandleAuthenticateAsync()
        {
            await Task.Delay(20);

            try
            {
                var token = GetAccessTokenQueryParam();
                //can have exceptions, they are caught here

                //2343 _fbigSecurityManager.ValidateToken(token, out ConnectBody connectBody);
                //can have exceptions, they are caught here

                //2343 var fbUserId = connectBody.PlayerId;

                var userId = await _fbigSecurityManager.SignInAsync(token/*the token is the fbid for testing*/);
                if (string.IsNullOrEmpty(userId))
                {
                    return AuthenticateResult.Fail("Unauthorized");
                }

                var genericClaims = new List<Claim>
                {
                    new Claim(ClaimTypes.NameIdentifier, userId.ToString()),
                     //this is the identifier used in the signalr, this claim type "NameIdentifier" could be changed with IUserIdProvider
                    new Claim(ClaimTypes.Name, "tst name"),
                    new Claim(ClaimTypes.Email, "tst mail"),
                    // new Claim("UserType", "General"),//role?
                };//this the only claims I can obtain from the payload

                var genericIdentity = new ClaimsIdentity(genericClaims, /*Scheme.Name*/ PROVIDER_NAME);
                //fbig shoud (in theory) have more than idnetity, but the auth provider is the same.. how to differentiat

                var principal = new GenericPrincipal(genericIdentity, null);

                var ticket = new AuthenticationTicket(principal, Scheme.Name);

                Debug.WriteLine($"login succeeded for player: {userId}");

                return AuthenticateResult.Success(ticket);
            }
            catch (Exception)
            {
                return AuthenticateResult.Fail("Unauthorized");
            }//todo: are you sure it's a bad request not internal server error?, you should use specific excepected errors for user fault

        }
    }
}