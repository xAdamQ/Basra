using System;
using System.Threading.Tasks;
using Basra.Server.Services;
using Basra.Server.Identity;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;

namespace Basra.Server
{
    [ApiController]
    [Route("base")]//basra if it's in tuxul domain, master in basra.com domain
    public class MasterController : ControllerBase
    {
        private readonly IConfiguration _configuration;
        private readonly SignInManager<BasraIdentityUser> _signInManager;
        private readonly UserManager<BasraIdentityUser> _userManager;
        private readonly FbigSecurityManager _securityManager;

        public MasterController(IConfiguration configuration, SignInManager<BasraIdentityUser> signInManager, UserManager<BasraIdentityUser> userManager, FbigSecurityManager securityManager)
        {
            _configuration = configuration;
            _signInManager = signInManager;
            _userManager = userManager;
            _securityManager = securityManager;
        }

        // [HttpPost]
        // [AllowAnonymous]//has relation with authentication middleware
        // public async Task<ActionResult> Connect(Models.Connect model)
        // {
        //     string userId = null;
        //     try
        //     {
        //         var tokenParts = model.Token.Split('.');

        //         if (!_securityManager.VerifySignature(tokenParts))
        //         {
        //             return BadRequest();
        //         }//dodn't pass validation

        //         var connectBody = _securityManager.DeserialzeConnectBody(tokenParts[1]);

        //         if (!_securityManager.RecentConnection(connectBody.IssuedAt))
        //         {
        //             return BadRequest();
        //         }

        //         userId = connectBody.PlayerId;
        //     }
        //     catch (Exception)
        //     {
        //         return BadRequest();
        //     }//todo: are you sure it's a bad request not internal server error?, you should use specific excepected errors for user fault

        //     await Task.Delay(50);

        //     // var user = await _userManager.FindByIdAsync(userId);
        //     //you will save this token incase
        //     //you will make another http requests (including reconnecting)

        //     //the final result is establishing the connection and log to the db or badrequest

        //     // var a = JwtRegisteredClaimNames.Email;//"email" string
        //     // var b = JwtRegisteredClaimNames.Jti;//"jti" string

        //     //in normal scenario
        //     //1- sign in with credentials and make a token and send it back
        //     //2- use this token in every reqeust

        //     //in my case
        //     //sign in with the generated token
        //     //allow only a certain time period to make http with it again
        //     //so you have to procees the token everytime??

        //     //he stores some claims 

        //     // new Microsoft.IdentityModel.Tokens.SigningCredentials();
        //     // new JwtSecurityTokenHandler().WriteToken()//make up a string using the token

        //     //any data in the requestpayload is signed
        //     //the possible metadat I can send is client type and version
        //     //but why don't send this with an object beside the token?
        //     //the only thing I need it signed is his Id

        //     return Ok(new { message = "succc", id = userId });

        // }

        // [Route("/games")]
        [HttpGet]
        [Authorize]
        public ActionResult GetGames()
        {
            return Ok(new { game1 = "game1Val", game3 = "game3Val", game2 = "game2Val", });
        }
    }
}