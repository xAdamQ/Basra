using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.SignalR.Client;
using System.Collections.Generic;
using System.Threading.Tasks;
using Xunit;
using Xunit.Abstractions;

namespace Basra.Server.Tests.Integration
{
    public class IntegrationTests
    {
        private readonly WebApplicationFactory<Startup> _factory;
        private readonly ITestOutputHelper _testOutputHelper;
        private List<Client> Clients { get; } = new();

        public IntegrationTests(ITestOutputHelper testOutputHelper)
        {
            _testOutputHelper = testOutputHelper;
            _factory = new CustomWebApplicationFactory<Startup>(_testOutputHelper);
        }

        async Task<Client> MakeClient()
        {
            var c = new Client(Clients.Count, _testOutputHelper);
            Clients.Add(c);
            _testOutputHelper.WriteLine("a new client is made with index: " + (Clients.Count - 1));

            await c.Connect(_factory.Server);

            return c;
        }

        [Fact]
        public async Task TestFirstDistribute()
        {
            var c = await MakeClient();
            var c2 = await MakeClient();

            await Task.Delay(50);

            await c.Connection.InvokeAsync("RequestRandomRoom", 0, 0);
            await c2.Connection.InvokeAsync("RequestRandomRoom", 0, 0);

            await Task.Delay(50);

            await c.Connection.InvokeAsync("Ready");
            await c2.Connection.InvokeAsync("Ready");

            await Task.Delay(50);

            for (int i = 0; i < 4; i++)
            {
                await c.Connection.InvokeAsync("Throw", 0);
                await c2.Connection.InvokeAsync("Throw", 0);
            }
        }

        [Fact(Timeout = 99999999)]
        public async Task TestForcePlay()
        {
            var c = await MakeClient();
            var c2 = await MakeClient();

            await c.Connection.InvokeAsync("RequestRandomRoom", 0, 0);
            await c2.Connection.InvokeAsync("RequestRandomRoom", 0, 0);

            await c.Connection.InvokeAsync("Ready");
            await c2.Connection.InvokeAsync("Ready");

            await c.Connection.InvokeAsync("Throw", 0);
            await Task.Delay(8000);
            await c.Connection.InvokeAsync("Throw", 0);
        }

        [Fact(Timeout = 99999999)]
        public async Task TestMissedTurn()
        {
            var c = await MakeClient();
            var c2 = await MakeClient();

            await c.Connection.InvokeAsync("RequestRandomRoom", 0, 0);
            await c2.Connection.InvokeAsync("RequestRandomRoom", 0, 0);

            await c.Connection.InvokeAsync("Ready");
            await c2.Connection.InvokeAsync("Ready");

            await c.Connection.InvokeAsync("Throw", 0);
            await c2.Connection.InvokeAsync("MissTurn");
            await c.Connection.InvokeAsync("Throw", 0);
        }

        [Fact(Timeout = 99999999)]
        public async Task TestReadyTimeout()
        {
            var c = await MakeClient();
            var c2 = await MakeClient();

            await c.Connection.InvokeAsync("RequestRandomRoom", 0, 0);
            await c2.Connection.InvokeAsync("RequestRandomRoom", 0, 0);

            await c2.Connection.InvokeAsync("Ready");

            await Task.Delay(7500);
        }

        [Fact(Timeout = 99999999)]
        public async Task TestPendingTimeout()
        {
            var c = await MakeClient();

            await c.Connection.InvokeAsync("RequestRandomRoom", 0, 0);

            await Task.Delay(1200);
        }

        [Fact(Timeout = 99999999)]
        public async Task TestBotRoomStart()
        {
            var c = await MakeClient();

            await c.Connection.InvokeAsync("RequestRandomRoom", 0, 0);

            await Task.Delay(2500);

            await c.Connection.InvokeAsync("Ready");
        }

        [Fact(Timeout = 99999999)]
        public async Task TestBotRoomPlay()
        {
            var c = await MakeClient();
            await Task.Delay(100);

            await c.Connection.InvokeAsync("RequestRandomRoom", 0, 0);

            await Task.Delay(2500);

            await c.Connection.InvokeAsync("Ready");
            await Task.Delay(100);

            await c.Connection.InvokeAsync("Throw", 0);

            await Task.Delay(100);

            // await c.Connection.InvokeAsync("Throw", 0);
            // await Task.Delay(400);
        }

        [Fact(Timeout = 99999999)]
        public async Task ThrowExc()
        {
            // var c = await MakeClient();

            // await c.Connection.SendAsync("ThrowExc");

            await Task.Delay(100);

            // _testOutputHelper.WriteLine("done");
        }

        [Fact(Timeout = 99999999)]
        public async Task TestBotRoomDistribute()
        {
            var c = await MakeClient();
            await Task.Delay(100);

            await c.Connection.InvokeAsync("RequestRandomRoom", 0, 0);

            await Task.Delay(2500);

            await c.Connection.InvokeAsync("Ready");
            await Task.Delay(100);

            for (int i = 0; i < 4; i++)
            {
                await c.Connection.InvokeAsync("Throw", 0);
                await Task.Delay(3000);
            }
        }

        [Fact(Timeout = 99999999)]
        public async Task TestFinalizeGame()
        {
            var c = await MakeClient();
            var c2 = await MakeClient();

            await Task.Delay(50);

            await c.Connection.InvokeAsync("RequestRandomRoom", 0, 0);
            await c2.Connection.InvokeAsync("RequestRandomRoom", 0, 0);


            await c.Connection.InvokeAsync("Ready");
            await c2.Connection.InvokeAsync("Ready");

            for (int i = 0; i < 8; i++)
            {
                await c.Connection.InvokeAsync("Throw", 0);
                await c2.Connection.InvokeAsync("Throw", 0);
            }

            await Task.Delay(100);
        }

        [Fact(Timeout = 99999999)]
        public async Task Reconnect_ShouldGetRoomStates()
        {
            var c = await MakeClient();
            var c2 = await MakeClient();

            await Task.Delay(50);

            await c.Connection.InvokeAsync("RequestRandomRoom", 0, 0);
            await c2.Connection.InvokeAsync("RequestRandomRoom", 0, 0);

            await c.Connection.InvokeAsync("Ready");
            await c2.Connection.InvokeAsync("Ready");

            for (int i = 0; i < 2; i++)
            {
                await c.Connection.InvokeAsync("Throw", 0);
                await c2.Connection.InvokeAsync("Throw", 0);
            }

            await c2.Connection.StopAsync();
            await Task.Delay(200);

            await c2.Connect(_factory.Server);
            await Task.Delay(200);

            await c2.Connection.StopAsync();
            await Task.Delay(200);

            await c2.Connect(_factory.Server);
            await Task.Delay(200);

            await c.Connection.StopAsync();
            await Task.Delay(200);

            await c.Connect(_factory.Server);
            await Task.Delay(200);
        }
    }
}