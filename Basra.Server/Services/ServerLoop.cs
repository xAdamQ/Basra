using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace Basra.Server.Services
{
    public interface IServerLoop
    {
        Task SetupTurnTimout(RoomUser roomUser);
        void CutTurnTimout(RoomUser roomUser);
    }

    public class ServerLoop : IServerLoop
    {
        private Dictionary<RoomUser, CancellationTokenSource> TurnCancellations;

        private static int creations = 0;

        private readonly IServiceScopeFactory _serviceScopeFactory;

        public ServerLoop(IServiceScopeFactory serviceScopeFactory)
        {
            TurnCancellations = new Dictionary<RoomUser, CancellationTokenSource>();

            _serviceScopeFactory = serviceScopeFactory;

            creations++;
        }

        private const int TurnTime = 10000;

        public async Task SetupTurnTimout(RoomUser roomUser)
        {
            var cSource = new CancellationTokenSource();
            TurnCancellations.Add(roomUser, cSource);

            try
            {
                await Task.Delay(TurnTime, cSource.Token);

                using (var scope = _serviceScopeFactory.CreateScope())
                {
                    var roomManager = scope.ServiceProvider.GetService<IRoomManager>();
                    await roomManager.RandomPlay(roomUser);
                }
            }
            catch (TaskCanceledException)
            {
                Console.WriteLine("user played normally within time");
            }
        }

        public void CutTurnTimout(RoomUser roomUser)
        {
            TurnCancellations[roomUser].Cancel();
        }

        // public void StartTurn(string userId)
        // {
        //     var turnTime = 10000;
        //     var cSource = new CancellationTokenSource();
        //     TurnCancellations.Add(userId, cSource);
        //
        //     Task.Delay(turnTime).ContinueWith(t => OnTurnTimout(userId), cSource.Token);
        // }
        //
        // private async Task OnTurnTimout(string userId)
        // {
        //     // await RandomPlay(userId);
        // }

        // private async Task RandomPlay(string userId)
        // {
        //     //pick user
        //     //
        //
        //     var randomCardIndex = StaticRandom.GetRandom(Cards.Count);
        //
        //     await Task.WhenAll
        //     (
        //         Play(randomCardIndex),
        //         Program.HubContext.Clients.Client(ConnectionId).SendAsync("OverrideMyLastThrow", randomCardIndex)
        //         // Structure.SendAsync("OverrideThrow", card)
        //     );
        // }


        // protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        // {
        //     Console.WriteLine("excute,,,,,,");
        // }
    }
}