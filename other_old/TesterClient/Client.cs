using System;
using System.Collections.Generic;
using System.Linq;
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

        public int ConvertCardIndexToValue(int index)
        {
            return (index % 13) + 1;
        }

        public async Task Connect()
        {
            Connection = new HubConnectionBuilder()
                .WithUrl("http://localhost:5000/connect?access_token=" + new Random().Next()).Build();

            SetupRpcs();

            Connection.ServerTimeout = TimeSpan.MaxValue;
            await Connection.StartAsync();
        }

        private void SetupRpcs()
        {
            Connection.On("RoomIsFilling",
                () => { System.Console.WriteLine("RoomIsFilling is called on client " + Id); });
            Connection.On<int, List<DisplayUser>>("StartRoom",
                (turn, du) =>
                {
                    System.Console.WriteLine(
                        $"StartRoom is called on {Id}, my turn is: {turn} all users: {string.Join("\n", du)}");
                });
            Connection.On<List<int>, List<int>>("InitialDistribute", (hand, ground) =>
            {
                System.Console.WriteLine
                (
                    $"InitialDistribute is called on {Id}, " +
                    $"hand is: {string.Join(", ", hand)} with values {string.Join(", ", hand.Select(v => ConvertCardIndexToValue(v)))} " +
                    $"ground is: {string.Join(", ", ground)} with values {string.Join(", ", ground.Select(v => ConvertCardIndexToValue(v)))} "
                );
            });
            Connection.On<int[]>("Distribute",
                (hand) =>
                {
                    System.Console.WriteLine(
                        $"Distribute is called on {Id}, my hand is: {string.Join(", ", hand)} with values {string.Join(", ", hand.Select(v => ConvertCardIndexToValue(v)))}");
                });
            Connection.On("OverrideMyLastThrow",
                () => { System.Console.WriteLine("OverrideMyLastThrow is called on client " + Id); });
            Connection.On<int>("CurrentOppoThrow",
                (card) => { System.Console.WriteLine("CurrentOppoThrow is called on client " + Id); });
        }

        public async void RpcCall(string rpc, object[] rpcArgs)
        {
            if (rpcArgs.Length == 0)
                await Connection.InvokeAsync(rpc);
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

    public class DisplayUser
    {
        public string Name { get; set; }
        public int PlayedRooms { get; set; }
        public int Wins { get; set; }

        public override string ToString()
        {
            return $"Name: {Name} PlayedRooms: {PlayedRooms} Wins: {Wins}";
        }
    }
}