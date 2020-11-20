using Microsoft.AspNetCore.WebUtilities;
using System;
using Newtonsoft.Json;
using System.Text;
using Basra.Server.Models;
using System.Security.Cryptography;
using Microsoft.Extensions.Configuration;
using Microsoft.AspNetCore.Identity;
using System.Threading.Tasks;
using System.Linq;
using Basra.Server.Helpers;

namespace Basra.Server.Services
{
    public class FbigSecurityManager
    {
        //I think the issuer/audiance is important
        //it defines who can use the token and the token maker
        private readonly string _appSecret;
        private readonly IConfiguration _configuration;
        private readonly SignInManager<BasraIdentityUser> _signInManager;
        private readonly UserManager<BasraIdentityUser> _userManager;
        private readonly MasterContext _masterContext;

        public FbigSecurityManager(IConfiguration configuration, SignInManager<BasraIdentityUser> signInManager, UserManager<BasraIdentityUser> userManager, MasterContext masterContext)
        {
            _configuration = configuration;
            _appSecret = _configuration["Secrets:AppSecret"];
            _signInManager = signInManager;
            _userManager = userManager;
            _masterContext = masterContext;
        }

        private bool VerifySignature(string[] token)
        {
            byte[] hash = null;
            using (var hmac = new HMACSHA256(Encoding.Default.GetBytes(_appSecret)))
            {
                hash = hmac.ComputeHash(Encoding.Default.GetBytes(token[1]));
            }

            var hash64 = Base64UrlTextEncoder.Encode(hash);

            return hash64 == token[0];
        }

        private ConnectBody DeserialzeConnectBody(string code)
        {
            var json = Encoding.Default.GetString(Base64UrlTextEncoder.Decode(code));
            return JsonConvert.DeserializeObject<ConnectBody>(json, Helper.SnakePropertyNaming);
        }

        private bool RecentConnection(int timestamp)
        {
            return true;
        }

        public bool ValidateToken(string token, out ConnectBody connectBody)
        {
            connectBody = null;

            var tokenParts = token.Split('.');

            if (!VerifySignature(tokenParts))
            {
                return false;
            }

            connectBody = DeserialzeConnectBody(tokenParts[1]);

            if (!RecentConnection(connectBody.IssuedAt))
            {
                return false;
            }

            return true;
        }

        /// <summary>
        /// returns the user id and if the operations wasn't successful returns null string
        /// </summary>
        public async Task<string> SignInAsync(string fbUserId)
        {
            //todo if (_userManager.FindByIdAsync(fbUserId) == null)
            if (_masterContext.Users.FirstOrDefault(u => u.FbId == fbUserId) == null)
            {
                var signupResult = await SignUpAsync(fbUserId);
                if (!signupResult) return null;
            }

            var user = _masterContext.Users.FirstOrDefault(u => u.FbId == fbUserId);

            //todo _signInManager.SignInAsync()

            return user.Id;
        }

        private async Task<bool> SignUpAsync(string fbUserId)
        {
            var user = new BasraIdentityUser
            {
                FbId = fbUserId,
                UserName = fbUserId,
            };

            var result = await _userManager.CreateAsync(user);

            return result.Succeeded;
        }

    }
}