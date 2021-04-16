using System.Collections.Generic;
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
            var mockClients = new Mock<IHubClients>();
            var groups = new Mock<IClientProxy>();

            mockClients.Setup(_ => _.GroupExcept(It.IsAny<string>(), It.IsAny<IReadOnlyList<string>>())).Returns(groups.Object);
            hub.Setup(_ => _.Clients).Returns(mockClients.Object);

            return hub;
        }

    }
}