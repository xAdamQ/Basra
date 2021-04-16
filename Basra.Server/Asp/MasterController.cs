using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using Basra.Server.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using System.Security.Claims;
using Basra.Server.Extensions;
using Microsoft.Extensions.Logging;

namespace Basra.Server
{
    public interface IMasterController
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="genre">whether round or room</param>
        /// <param name="bet"></param>
        /// <param name="capacity"></param>
        /// <returns></returns>
        Task RequestRoom(int genre, int bet, int capacity);

        Task RequestFriendlyRoom(string[] userIds, int bet, int capacity);
        Task BuyCardBack(int id);
        Task BuyBackground(int id);
        Task RequestProfile(string userId);
        Task BuyScratchWin();
        Task BuyShuffleWin();
    }

    [ApiController]
    [Route("base")] //basra if it's in tuxul domain, master in basra.com domain
    public class MasterController : ControllerBase
    {
        private readonly IConfiguration _configuration;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly ILogger _logger;
        private readonly IMasterRepo _masterRepo;
        private readonly ISessionRepo _sessionRepo;
        private readonly IServerLoop _serverLoop;

        public MasterController(IMasterRepo masterRepo, ISessionRepo sessionRepo,
            IServerLoop serverLoop, IConfiguration configuration, IHttpContextAccessor httpContextAccessor)
        {
            _masterRepo = masterRepo;
            _sessionRepo = sessionRepo;
            _serverLoop = serverLoop;
            // _logger = logger;
            _configuration = configuration;
            _httpContextAccessor = httpContextAccessor;
        }

        [HttpGet]
        [Authorize]
        public ActionResult TestGet()
        {
            return Ok(new {game1 = "game1Val", game3 = "game3Val", game2 = "game2Val",});
        }

        //
        // [HttpPost]
        // [AllowAnonymous] //has relation with authentication middleware
        // public async Task<ActionResult> Connect(Models.Connect model)
        // {
        //     string userId = null;
        //     try
        //     {
        //         var tokenParts = model.Token.Split('.');
        //
        //         if (!_securityManager.VerifySignature(tokenParts))
        //         {
        //             return BadRequest();
        //         } //dodn't pass validation
        //
        //         var connectBody = _securityManager.DeserialzeConnectBody(tokenParts[1]);
        //
        //         if (!_securityManager.RecentConnection(connectBody.IssuedAt))
        //         {
        //             return BadRequest();
        //         }
        //
        //         userId = connectBody.PlayerId;
        //     }
        //     catch (Exception)
        //     {
        //         return BadRequest();
        //     } //todo: are you sure it's a bad request not internal server error?, you should use specific excepected errors for user fault
        //
        //     await Task.Delay(50);
        //
        //     // var user = await _userManager.FindByIdAsync(userId);
        //     //you will save this token incase
        //     //you will make another http requests (including reconnecting)
        //
        //     //the final result is establishing the connection and log to the db or badrequest
        //
        //     // var a = JwtRegisteredClaimNames.Email;//"email" string
        //     // var b = JwtRegisteredClaimNames.Jti;//"jti" string
        //
        //     //in normal scenario
        //     //1- sign in with credentials and make a token and send it back
        //     //2- use this token in every reqeust
        //
        //     //in my case
        //     //sign in with the generated token
        //     //allow only a certain time period to make http with it again
        //     //so you have to procees the token everytime??
        //
        //     //he stores some claims 
        //
        //     // new Microsoft.IdentityModel.Tokens.SigningCredentials();
        //     // new JwtSecurityTokenHandler().WriteToken()//make up a string using the token
        //
        //     //any data in the requestpayload is signed
        //     //the possible metadat I can send is client type and version
        //     //but why don't send this with an object beside the token?
        //     //the only thing I need it signed is his Id
        //
        //     return Ok(new {message = "succc", id = userId});
        // }

        // [Route("/games")]

        // [HttpGet]
        // [Authorize]
        // public ActionResult GetGames()
        // {
        //     return Ok(new {game1 = "game1Val", game3 = "game3Val", game2 = "game2Val",});
        // }

        // [Authorize]
        // [HttpPost]
        // public async Task<ActionResult> RequestRoom(int genre, int bet, int capacity)
        // {
        //     var userId = _httpContextAccessor.HttpContext.User.FindFirst(ClaimTypes.NameIdentifier).Value;
        //
        //     var room = _sessionRepo.GetPendingRoom(genre, capacity);
        //     if (room == null)
        //     {
        //         room = _sessionRepo.MakeRoom(genre, capacity);
        //         _logger.LogInformation("a new room is made");
        //     }
        //
        //     //currently we don't save connected users
        //     var rUser = _sessionRepo.AddRoomUser(userId, room);
        //
        //     if (room.Capacity == room.RoomUsers.Count)
        //     {
        //         _logger.LogInformation("a room is ready and will start");
        //         await StartRoom(room);
        //     }
        //     else
        //     {
        //         _sessionRepo.KeepRoom(room);
        //         await _masterHub.Clients.User(rUser.UserId).SendAsync("RoomIsFilling");
        //     }
        // }
        //
        // private async Task StartRoom(Room room)
        // {
        //     room.Deck = GenerateDeck();
        //
        //     List<int> GenerateDeck()
        //     {
        //         var deck = new List<int>();
        //         for (int i = 0; i < Room.DeckSize; i++)
        //         {
        //             deck.Add(i);
        //         }
        //
        //         deck.Shuffle();
        //         return deck;
        //     }
        //
        //     room.GroundCards = room.Deck.CutRange(RoomUser.HandSize);
        //
        //     var dUsers = _masterRepo.GetRoomDisplayUsersAsync(room);
        //     var rUsers = room.RoomUsers;
        //
        //     var tasks = new List<Task>();
        //     for (int i = 0; i < room.Capacity; i++)
        //     {
        //         rUsers[i].TurnId = i;
        //
        //         tasks.Add(_masterHub.Groups.AddToGroupAsync(rUsers[i].ConnectionId, "room" + room.Id));
        //         tasks.Add(_masterHub.Clients.User(rUsers[i].UserId).SendAsync("StartRoom", i, dUsers));
        //     }
        //
        //     await Task.WhenAll(tasks);
        //
        //     StartTurn(rUsers[0]);
        // }
    }
}