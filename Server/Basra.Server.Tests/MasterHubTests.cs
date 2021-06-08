using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.SignalR;
using Moq;
using Xunit;

namespace Basra.Server.Tests
{
    public class MasterHubTests
    {
        public static Mock<IHubContext<MasterHub>> GetMockWithSendFuns()
        {
            var hub = new Mock<IHubContext<MasterHub>>();
            var hubClient = new Mock<IHubClients>();
            var clientProxy = new Mock<IClientProxy>();

            hubClient.Setup(_ => _.GroupExcept(It.IsAny<string>(), It.IsAny<IReadOnlyList<string>>()))
                .Returns(clientProxy.Object);
            hubClient.Setup(_ => _.User(It.IsAny<string>())).Returns(clientProxy.Object);

            hub.Setup(_ => _.Clients).Returns(hubClient.Object);

            return hub;
        }
    }
}