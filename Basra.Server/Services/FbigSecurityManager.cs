using Microsoft.AspNetCore.WebUtilities;
using System;
using System.Linq;
using Newtonsoft.Json;
using System.Text;
using Basra.Server.Models;
using System.Security.Cryptography;
using Microsoft.Extensions.Configuration;
using Microsoft.AspNetCore.Identity;
using System.Threading.Tasks;
using Basra.Server.Helpers;
using Microsoft.EntityFrameworkCore;
//using Basra.Server.Identity;

namespace Basra.Server.Services
{
    public class FbigSecurityManager
    {
        //I think the issuer/audiance is important
        //it defines who can use the token and the token maker
        private readonly string _appSecret;
        private readonly IConfiguration _configuration;
        private readonly IMasterRepo _masterRepo;

        //private readonly SignInManager<User> _signInManager;
        //private readonly UserManager<User> _userManager;
        //private readonly IdentityConetxt _masterContext;

        public FbigSecurityManager(IConfiguration configuration, IMasterRepo masterRepo)
        //SignInManager<User> signInManager, UserManager<User> userManager, IdentityConetxt masterContext)
        {
            _configuration = configuration;
            _masterRepo = masterRepo;
            _appSecret = _configuration["Secrets:AppSecret"];

            //_signInManager = signInManager;
            //_userManager = userManager;
            //_masterContext = masterContext;
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

        private bool IsRecentConnection(int timestamp)
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

            if (!IsRecentConnection(connectBody.IssuedAt))
            {
                return false;
            }

            return true;
        }

        /// <summary>
        /// check if the user exist and make a new one if note
        /// </summary>
        public async Task<User> SignInAsync(string fbUserId)
        {
            //todo if (_userManager.FindByIdAsync(fbUserId) == null)
            // _userManager.FindByLoginAsync()
            // if (_masterContext.Users.Any(u => u.FbId == fbUserId))
            //var user = await _masterContext.Users.FirstOrDefaultAsync(u => u.FbId == fbUserId);

            User user = null;

            try
            {
                user = await _masterRepo.GetUserByFbidAsync(fbUserId);
            }
            catch (Exception)
            {
                user = await SignUpAsync(fbUserId);
            }

            if (user == null)
            {
                user = await SignUpAsync(fbUserId);
            }
            else if (user.IsActive)
            {
                throw new Exception("the user is already logged in");
            }
            //else if (ActiveUser.All.Any(u => u.Id == user.Id))
            //{
            //    throw new Exception("the user is already logged in");
            //}

            return user;

            // todo 
            // await _signInManager.SignInWithClaimsAsync()
            // SignInAsync(new BasraIdentityUser(), isPersistent: false, "fbig");
            //issues a cookie
        }

        private async Task<User> SignUpAsync(string fbUserId)
        {
            //var user = new User
            //{
            //    FbId = fbUserId,
            //    UserName = "AsAName_" + fbUserId,
            //};

            //await _userManager.CreateAsync(user);

            var user = await _masterRepo.CreateUserAsync(fbUserId);
            _masterRepo.SaveChanges();
            return user;

            //todo the result maybe failure
            //return user;
        }

    }
}