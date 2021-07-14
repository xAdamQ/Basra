using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Basra.Server.Services
{
    public interface IServerLoop
    {
        void SetupTurnTimeout(RoomUser roomUser);
        void CancelTurnTimeout(RoomUser roomUser);
        /// <summary>
        /// when players a late for ready
        /// </summary>
        void SetForceStartRoomTimeout(Room room);
        void CancelForceStart(Room room);
        void SetupPendingRoomTimeoutIfNotExist(Room room);
        void CancelPendingRoomTimeout(Room room);
        void BotPlay(RoomBot roomBot);
        void CancelTurnTimeoutIfExist(RoomUser roomUser);
    }

    public class ServerLoop : IServerLoop
    {
        private readonly IServiceScopeFactory _serviceScopeFactory;
        private readonly ILogger<IServerLoop> _logger;
        private readonly ISessionRepo _sessionRepo;

        public ServerLoop(IServiceScopeFactory serviceScopeFactory, ILogger<IServerLoop> logger, ISessionRepo sessionRepo)
        {
            _sessionRepo = sessionRepo;
            _serviceScopeFactory = serviceScopeFactory;
            _logger = logger;
        }

        private Dictionary<RoomUser, CancellationTokenSource> TurnCancellations { get; } = new();
        private const int TurnTime = 700000000;//todo change
        public void SetupTurnTimeout(RoomUser roomUser)
        {
            var cSource = new CancellationTokenSource();
            TurnCancellations.Add(roomUser, cSource);

            Task.Delay(TurnTime, cSource.Token).ContinueWith(task =>
            {
                if (!task.IsCanceled) Task.Run(() => OnTurnTimeout(roomUser));
            });
        }
        private async Task OnTurnTimeout(RoomUser roomUser)
        {
            TurnCancellations.Remove(roomUser);

            using (var scope = _serviceScopeFactory.CreateScope())
            {
                var roomUserManager = scope.ServiceProvider.GetService<IRoomManager>();
                await roomUserManager.ForceUserPlay(roomUser);
            }
        }
        public void CancelTurnTimeout(RoomUser roomUser)
        {
            TurnCancellations[roomUser].Cancel();
            TurnCancellations.Remove(roomUser);
        }
        public void CancelTurnTimeoutIfExist(RoomUser roomUser)
        {
            if (TurnCancellations.ContainsKey(roomUser))
                CancelTurnTimeout(roomUser);
        }

        private Dictionary<Room, CancellationTokenSource> PendingRoomCancellations { get; } = new();
        private const int PendingRoomTimeout = 2000; //todo change
        public void SetupPendingRoomTimeoutIfNotExist(Room room)
        {
            if (PendingRoomCancellations.ContainsKey(room)) return;

            var cSource = new CancellationTokenSource();
            PendingRoomCancellations.Add(room, cSource);

            Task.Delay(PendingRoomTimeout, cSource.Token).ContinueWith(task =>
            {
                if (!task.IsCanceled) Task.Run(() => OnPendingRoomTimeout(room));
            });
        }
        private async Task OnPendingRoomTimeout(Room room)
        {
            PendingRoomCancellations.Remove(room);

            using (var scope = _serviceScopeFactory.CreateScope())
            {
                var roomRequester = scope.ServiceProvider.GetService<IMatchMaker>();
                await roomRequester.FillPendingRoomWithBots(room);
            }
        }
        public void CancelPendingRoomTimeout(Room room)
        {
            PendingRoomCancellations[room].Cancel();
            PendingRoomCancellations.Remove(room);
        }

        private Dictionary<Room, CancellationTokenSource> ForceStartCancellations { get; } = new();
        private const int ReadyTimeout = 99999999;//todo change
        public void SetForceStartRoomTimeout(Room room)
        {
            var cSource = new CancellationTokenSource();
            ForceStartCancellations.Add(room, cSource);

            Task.Delay(ReadyTimeout, cSource.Token)
                .ContinueWith(task =>
                {
                    if (!task.IsCanceled) Task.Run(() => OnForceStartTimeout(room));
                });
        }
        private async Task OnForceStartTimeout(Room room)
        {
            ForceStartCancellations.Remove(room);

            using (var scope = _serviceScopeFactory.CreateScope())
            {
                //this will fail because you don't have scope
                var roomManager = scope.ServiceProvider.GetService<IRoomManager>();
                await roomManager.StartRoom(room);
            }
        }
        public void CancelForceStart(Room room)
        {
            ForceStartCancellations[room].Cancel();
            ForceStartCancellations.Remove(room);
        }


        private const int BotPlayMin = 50, BotPlayMax = 150;//todo change

        public void BotPlay(RoomBot roomBot)
        {
            Task.Run(async () =>
            {
                await Task.Delay(StaticRandom.GetRandom(BotPlayMin, BotPlayMax));

                using var scope = _serviceScopeFactory.CreateScope();
                var roomManager = scope.ServiceProvider.GetService<IRoomManager>();
                await roomManager.BotPlay(roomBot);
            }).ContinueWith(t =>
            {
                if (t.Exception != null) throw t.Exception;
            });
        }


        // private Dictionary<Action, CancellationTokenSource> DelayCancellations { get; } = new();
        // public void Delay(int delay, Action onComplete, Object key)
        // {
        //     var cSource = new CancellationTokenSource();
        //     DelayCancellations.Add(onComplete, cSource);
        //
        //     Task.Delay(ReadyTimeout, cSource.Token).ContinueWith(_ => onComplete, cSource.Token);
        //     DelayCancellations.Remove(onComplete);
        // }
        // public void CancelDelay(Room room)
        // {
        //     ForceStartCancellations[room].Cancel();
        // }


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