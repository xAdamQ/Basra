using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace Basra.Server.Services
{
    //return money to every player
    //

    //session are sticky

    public class ServerLoop : BackgroundService
    {
        private Dictionary<string, CancellationTokenSource> TurnCancellations =
            new Dictionary<string, CancellationTokenSource>();

        private Dictionary<string, RoomUser> RoomUsers = new Dictionary<string, RoomUser>();


        private static int creations = 0;

        private readonly IServiceScopeFactory _serviceScopeFactory;

        public ServerLoop(IServiceScopeFactory serviceScopeFactory)
        {
            _serviceScopeFactory = serviceScopeFactory;

            creations++;
            Console.WriteLine($"sl is  created {creations} times");

            StartTurn("");
        }

        public void StartTurn(string userId)
        {
            var turnTime = 10000;
            var cSource = new CancellationTokenSource();
            TurnCancellations.Add(userId, cSource);

            using (var scope = _serviceScopeFactory.CreateScope())
            {
                var mr = scope.ServiceProvider.GetService<MasterRepo>();
            }

            Task.Delay(turnTime).ContinueWith(t => OnTurnTimout(userId), cSource.Token);
        }

        private async Task OnTurnTimout(string userId)
        {
            await RandomPlay();
        }

        private async Task RandomPlay(string userId)
        {
            //pick user
            //

            var randomCardIndex = StaticRandom.GetRandom(Cards.Count);

            await Task.WhenAll
            (
                Play(randomCardIndex),
                Program.HubContext.Clients.Client(ConnectionId).SendAsync("OverrideMyLastThrow", randomCardIndex)
                // Structure.SendAsync("OverrideThrow", card)
            );
        }


        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            Console.WriteLine("excute,,,,,,");
        }
    }
}