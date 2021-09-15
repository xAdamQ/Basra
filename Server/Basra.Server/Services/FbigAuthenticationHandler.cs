using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Security.Claims;
using System.Security.Principal;
using System.Text.Encodings.Web;
using System.Threading.Tasks;
using Basra.Server.Models;
using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Basra.Server.Services
{
    public class FbigAuthenticationSchemeOptions : AuthenticationSchemeOptions
    {
    }

    public class FbigAuthenticationHandler : AuthenticationHandler<FbigAuthenticationSchemeOptions>
    {
        public const string PROVIDER_NAME = "Fbig";

        private readonly FbigSecurityManager _fbigSecurityManager;
        private ILogger<FbigAuthenticationHandler> _logger;

        public FbigAuthenticationHandler(IOptionsMonitor<FbigAuthenticationSchemeOptions> options,
            ILoggerFactory loggerFac,
            UrlEncoder encoder, ISystemClock clock, ILogger<FbigAuthenticationHandler> logger,
            FbigSecurityManager fbigSecurityManager)
            : base(options, loggerFac, encoder, clock)
        {
            _fbigSecurityManager = fbigSecurityManager;
            _logger = logger;
        }


        protected override Task HandleChallengeAsync(AuthenticationProperties properties)
        {
            _logger.LogInformation("handling challenge: " +
                                   JsonConvert.SerializeObject(properties));
            return base.HandleChallengeAsync(properties);
        }

        protected override Task HandleForbiddenAsync(AuthenticationProperties properties)
        {
            _logger.LogInformation("handling forbidden: " +
                                   JsonConvert.SerializeObject(properties));
            return base.HandleForbiddenAsync(properties);
        }

        protected override Task InitializeHandlerAsync()
        {
            _logger.LogInformation("init hanlder: ");
            return base.InitializeHandlerAsync();
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

        protected override async Task<AuthenticateResult> HandleAuthenticateAsync()
        {
            _logger.LogInformation("some one is logging in");

            try
            {
                Request.Query.TryGetValue("access_token", out var figToken);
                Request.Query.TryGetValue("huaweiAuthCode", out var huaweiAuthCode);

                Request.Query.TryGetValue("name", out var name);
                Request.Query.TryGetValue("pictureUrl", out var pictureUrl);
                Request.Query.TryGetValue("demo", out var demo);
                //can have exceptions, they are caught here

                User user;
                if (String.IsNullOrEmpty(demo)) //not demo
                {
                    if (string.IsNullOrEmpty(figToken)) //hauwei
                    {
                        var token = await GetTokenByHuaweiAuthCode(huaweiAuthCode);

                        if (token == null)
                            return AuthenticateResult.Fail(
                                "huawei auth failed with given auth code");

                        var userData = await GetUserDatByToken(token);

                        user = await _fbigSecurityManager.SignInAsync(userData.openId,
                            userData.name, userData.picUrl);
                    }
                    else //real fig
                    {
                        if (!_fbigSecurityManager.ValidateToken(figToken, out var playerId))
                            return AuthenticateResult.Fail("Unauthorized");

                        user = await _fbigSecurityManager.SignInAsync(playerId, name, pictureUrl);
                    }
                }
                else
                {
                    user = await _fbigSecurityManager.SignInAsync(figToken, name, pictureUrl);
                }


                var genericClaims = new List<Claim>
                {
                    new(ClaimTypes.NameIdentifier, user.Id),

                    //this is the identifier used in the signalr, this claim type "NameIdentifier" could be changed with IUserIdProvider
                    //new Claim(ClaimTypes.Name, user.UserName),
                    //new Claim(ClaimTypes.Email, user.Email),
                    // new Claim("UserType", "General"),//role?
                }; //this the only claims I can obtain from the payload

                var genericIdentity =
                    new ClaimsIdentity(genericClaims, /*Scheme.Name*/ PROVIDER_NAME);
                //fbig shoud (in theory) have more than idnetity, but the auth provider is the same.. how to differentiat

                var principal = new GenericPrincipal(genericIdentity, null);

                var ticket = new AuthenticationTicket(principal, Scheme.Name);

                // _logger.LogInformation($"login succeeded for player: {user.Id}");
                return AuthenticateResult.Success(ticket);
            }
            catch (Exception exception)
            {
                _logger.LogInformation($"login failed due to exception for player: {exception}");
                return AuthenticateResult.Fail("Unauthorized");
            } //todo: are you sure it's a bad request not internal server error?, you should use specific expected errors for user fault
        }

        public static async Task<string> GetTokenByHuaweiAuthCode(string code)
        {
            var data = new FormUrlEncodedContent(new Dictionary<string, string>()
            {
                { "grant_type", "authorization_code" },
                { "client_id", "104645983" },
                {
                    "client_secret",
                    "70fa010a05f4a3c9fc389d0046cd69cacc7b50f8d26b09e65940c9ef37abf416"
                },
                { "code", code },
                { "redirect_uri", "hms://redirect_uri" }
            });

            var url = "https://oauth-login.cloud.huawei.com/oauth2/v3/token";
            using var client = new HttpClient();

            var response = await client.PostAsync(url, data);

            string result = response.Content.ReadAsStringAsync().Result;

            dynamic obj = JObject.Parse(result);

            return obj.access_token;
        }

        public static async Task<(string name, string picUrl, string openId)> GetUserDatByToken(
            string token)
        {
            var data = new FormUrlEncodedContent(new Dictionary<string, string>()
            {
                { "access_token", token },
                { "getNickName", "1" },
            });

            var url = "https://account.cloud.huawei.com/rest.php?nsp_svc=GOpen.User.getInfo";
            using var client = new HttpClient();

            var response = await client.PostAsync(url, data);

            string result = response.Content.ReadAsStringAsync().Result;

            dynamic obj = JObject.Parse(result);

            return (obj.displayName, obj.headPictureURL, obj.openID);
        }
    }
}