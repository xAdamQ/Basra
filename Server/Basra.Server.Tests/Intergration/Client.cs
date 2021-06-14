using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using Basra.Models.Client;
using Microsoft.AspNetCore.SignalR.Client;
using Microsoft.AspNetCore.TestHost;
using Xunit.Abstractions;

namespace Basra.Server.Tests.Integration
{
    public class Client
    {
        public HubConnection Connection { get; private set; }
        private readonly ITestOutputHelper _testOutputHelper;

        public int Id { get; }

        public Client(int id, ITestOutputHelper testOutputHelper)
        {
            _testOutputHelper = testOutputHelper;
            Id = id;
        }

        public async Task Connect(TestServer server)
        {
            Connection = new HubConnectionBuilder()
                // .ConfigureLogging(lb => lb
                //     .AddConsole()
                //     .AddDebug()
                //     .AddFilter(l => l == LogLevel.Information)
                // )
                .WithUrl("http://localhost:5000/connect?access_token=" + Id,
                    // new Random().Next(),
                    o => o.HttpMessageHandlerFactory = _ => server.CreateHandler())
                .Build();

            SetupRpcs();

            await Connection.StartAsync();
        }

        private void SetupRpcs()
        {
            Connection.On<PersonalFullUserInfo, MinUserInfo[], MinUserInfo[]>("InitGame",
                (p, y, t) =>
                {
                    _testOutputHelper.WriteLine($"init game called on {Id} with\n" +
                                      $"personal info is {JsonSerializer.Serialize(p)}\n" +
                                      $"yesterday champs are {JsonSerializer.Serialize(y)}\n" +
                                      $"topFriends are {JsonSerializer.Serialize(t)}\n");
                });

            Connection.On<List<RoomOppoInfo>, int>("PrepareRequestedRoomRpc", (oppos, turn) =>
                _testOutputHelper.WriteLine($"PrepareRequestedRoomRpc called on {Id} with\n" +
                                  $"oppo info are {JsonSerializer.Serialize(oppos)}\n" +
                                  $"and turn {turn}\n"));


            Connection.On<ThrowResult>("MyThrowResult", res =>
                _testOutputHelper.WriteLine($"MyThrowResult is called on {Id} with\n" +
                                  $"with res {JsonSerializer.Serialize(res)}\n" +
                                  $"with values {JsonSerializer.Serialize(res.EatenCardsIds.Select(ConvertCardIndexToValue))}\n"));

            Connection.On<List<int>, List<int>>("StartGameRpc", (hand, ground) =>
                _testOutputHelper.WriteLine
                (
                    $"StartGameRpc is called on {Id},\n" +
                    $"hand is: {string.Join(", ", hand)} with values {string.Join(", ", hand.Select(ConvertCardIndexToValue))}\n" +
                    $"ground is: {string.Join(", ", ground)} with values {string.Join(", ", ground.Select(ConvertCardIndexToValue))}\n"
                ));

            Connection.On<int[]>("Distribute", hand =>
                _testOutputHelper.WriteLine
                (
                     $"Distribute is called on {Id}\n" +
                     $"my hand is: {string.Join(", ", hand)}\n" +
                     $"with values {string.Join(", ", hand.Select(v => ConvertCardIndexToValue(v)))}\n"
                ));

            Connection.On<ThrowResult>("ForcePlay",
                res =>
                {
                    _testOutputHelper.WriteLine($"ForcePlay is called on client {Id}\n" +
                                      $"with card {res.ThrownCard} with value {ConvertCardIndexToValue(res.ThrownCard)}\n" +
                                      $"and full res {JsonSerializer.Serialize(res)}\n" +
                                      $"with values {JsonSerializer.Serialize(res.EatenCardsIds.Select(_ => ConvertCardIndexToValue(_)))}\n");
                });

            Connection.On<ThrowResult>("CurrentOppoThrow",
                res =>
                {
                    _testOutputHelper.WriteLine($"CurrentOppoThrow is called on client {Id}\n" +
                                      $"with card {res.ThrownCard} with value {ConvertCardIndexToValue(res.ThrownCard)}\n" +
                                      $"and full res {JsonSerializer.Serialize(res)}\n" +
                                      $"with values {JsonSerializer.Serialize(res.EatenCardsIds.Select(_ => ConvertCardIndexToValue(_)))}\n");
                });


            Connection.On<PersonalFullUserInfo>("UpdatePersonalInfo", info =>
                _testOutputHelper.WriteLine($"UpdatePersonalInfo is called on {Id} with\n" +
                                  $"info is {JsonSerializer.Serialize(info)}\n"));

            Connection.On<FinalizeResult>("FinalizeResult", (res) =>
                _testOutputHelper.WriteLine($"UpdatePersonalInfo is called on {Id} with\n" +
                                  $"res {JsonSerializer.Serialize(res)}\n"));

            int ConvertCardIndexToValue(int index)
            {
                return (index % 13) + 1;
            }
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