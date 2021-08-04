using Microsoft.AspNetCore.WebUtilities;
using System;
using System.Linq;
using Newtonsoft.Json;
using System.Text;
using System.Security.Cryptography;
using Microsoft.Extensions.Configuration;
using Microsoft.AspNetCore.Identity;
using System.Threading.Tasks;
using Basra.Server.Exceptions;
using Basra.Server.Helpers;
using Basra.Server.Models;
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
        private readonly ISessionRepo _sessionRepo;

        public FbigSecurityManager(IConfiguration configuration, IMasterRepo masterRepo, ISessionRepo sessionRepo)
        {
            _configuration = configuration;
            _masterRepo = masterRepo;
            _sessionRepo = sessionRepo;
            _appSecret = _configuration["Secrets:AppSecret"];
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

        public bool ValidateToken(string token, out string playerId)
        {
            playerId = null;
            try
            {
                var tokenParts = token.Split('.');

                if (!VerifySignature(tokenParts))
                {
                    return false;
                }

                var connectBody = DeserialzeConnectBody(tokenParts[1]);

                if (!IsRecentConnection(connectBody.IssuedAt))
                {
                    return false;
                }

                playerId = connectBody.PlayerId;
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        /// <summary>
        /// checks if the user exist and make a new one if not
        /// the last 2 args are not used if you won't sign up and would be usually null
        /// </summary>
        public async Task<User> SignInAsync(string fbUserId, string name, string pictureUrl)
        {
            var user = await _masterRepo.GetUserByFbidAsync(fbUserId);

            if (user == null) return await SignUpAsync(fbUserId, name, pictureUrl);

            return user;
        }

        private async Task<User> SignUpAsync(string fbUserId, string name, string pictureUrl)
        {
            var user = new User
            {
                Fbid = fbUserId,
                Name = name,
                PictureUrl = pictureUrl,
                Money = 200,
            };

            await _masterRepo.CreateUserAsync(user);
            //todo the result of creation maybe failure

            await _masterRepo.SaveChangesAsync();
            return user;
        }
    }
}