using Microsoft.AspNetCore.SignalR;

namespace Basra.Server.Asp
{
    public class MasterManager
    {
        public static MasterManager I { get; set; }
        public IHubContext<MasterHub> HubContext { get; }

        public MasterManager(IHubContext<MasterHub> hubContext)
        {
            I = this;
            HubContext = hubContext;
        }
    }
}