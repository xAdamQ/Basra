using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Logging;

namespace Basra.Server.Services
{
    public class LobbyManager
    {
        private readonly ILogger _logger;
        private readonly IHubContext<MasterHub> _masterHub;
        private readonly IMasterRepo _masterRepo;
        private readonly ISessionRepo _sessionRepo;
        private readonly IServerLoop _serverLoop;

        public LobbyManager(IHubContext<MasterHub> masterHub, IMasterRepo masterRepo, ISessionRepo sessionRepo,
            IServerLoop serverLoop)
        {
            _masterHub = masterHub;
            _masterRepo = masterRepo;
            _sessionRepo = sessionRepo;
            _serverLoop = serverLoop;
            // _logger = logger;
        }

    }
}