using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.SignalR;
using System.Linq;

//todo learn about thread safety
namespace TestServer
{
    [Authorize]
    public class MasterHub : Hub
    {
        private HubCallerContext cxt;


        public MasterHub()
        {
            if (cxt == null)
                cxt = Context;
            else
                Console.WriteLine(cxt == Context);

            Console.WriteLine(Context);
        }

        public override async Task OnConnectedAsync()
        {
            Console.WriteLine($"connection established: {Context.UserIdentifier} {Context.UserIdentifier}");

            await base.OnConnectedAsync();
        }

        public override async Task OnDisconnectedAsync(Exception exception)
        {
            Console.WriteLine($"{Context.UserIdentifier} Disconnected");

            await base.OnDisconnectedAsync(exception);
        }
    }
}