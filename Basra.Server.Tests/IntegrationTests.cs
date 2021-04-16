using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.SignalR.Client;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.Hosting;
using Xunit;
using Xunit.Abstractions;

namespace Basra.Server.Tests
{
    public class IntegrationTests : IClassFixture<WebApplicationFactory<Startup>>
    {
        private readonly WebApplicationFactory<Startup> _factory;
        private readonly ITestOutputHelper _testOutputHelper;
        private List<Client> Clients { get; } = new();

        public IntegrationTests(WebApplicationFactory<Startup> factory, ITestOutputHelper testOutputHelper)
        {
            _factory = factory;
            _testOutputHelper = testOutputHelper;
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
        public async Task RequestRoom()
        {
            var c = await MakeClient();
            var c2 = await MakeClient();

            await c.Connection.InvokeAsync("RequestRoom", 0, 0, 0);
            await c2.Connection.InvokeAsync("RequestRoom", 0, 0, 0);
            await c2.Connection.InvokeAsync("RequestRoom", 0, 0, 0);
        }
    }
}