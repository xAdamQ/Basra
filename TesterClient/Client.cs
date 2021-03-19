using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.SignalR.Client;

namespace TesterClient
{
    public class Client
    {
        public HubConnection Connection;

        public int Id { get; set; }

        public Client(int id)
        {
            Id = id;
        }

        public async Task Connect()
        {
            Connection = new HubConnectionBuilder()
                .WithUrl("http://localhost:5000/connect?access_token=" + new Random().Next()).Build();

            SetupRpcs();

            await Connection.StartAsync();
        }

        private void SetupRpcs()
        {
            Connection.On("RoomIsFilling", () => { System.Console.WriteLine("RoomIsFilling is called on client " + Id); });
            Connection.On("StartRoom", () => { System.Console.WriteLine("StartRoom is called"); });
            Connection.On("InitialDistribute", () => { System.Console.WriteLine("InitialDistribute is called on client " + Id); });
            Connection.On("Distribute", () => { System.Console.WriteLine("Distribute is called on client " + Id); });
            Connection.On("OverrideMyLastThrow", () => { System.Console.WriteLine("OverrideMyLastThrow is called on client " + Id); });
            Connection.On("CurrentOppoThrow", () => { System.Console.WriteLine("CurrentOppoThrow is called on client " + Id); });
        }

        public async Task RequestRoom(int genre, int bet, int capacity)
        {
            await Connection.InvokeAsync("RequestRoom", genre, bet, capacity);
        }

        public async void RpcCall(string rpc, object[] rpcArgs)
        {
            if (rpcArgs.Length == 1)
                await Connection.InvokeAsync(rpc, rpcArgs[0]);
            else if (rpcArgs.Length == 2)
                await Connection.InvokeAsync(rpc, rpcArgs[0], rpcArgs[1]);
            else if (rpcArgs.Length == 3)
                await Connection.InvokeAsync(rpc, rpcArgs[0], rpcArgs[1], rpcArgs[2]);
            else if (rpcArgs.Length == 4)
                await Connection.InvokeAsync(rpc, rpcArgs[0], rpcArgs[1], rpcArgs[2], rpcArgs[3]);

        }
    }
}