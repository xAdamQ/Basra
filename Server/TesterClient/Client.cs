using Microsoft.AspNetCore.SignalR.Client;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Basra.Common;
using Microsoft.Extensions.Logging;

// ReSharper disable TemplateIsNotCompileTimeConstantProblem

namespace TesterClient
{
    public class Client
    {
        public HubConnection Connection { get; private set; }

        public int Id { get; }

        private readonly ILogger _logger;

        public Client(int id, ILogger logger)
        {
            Id = id;
            _logger = logger;
        }

        public async Task Connect(HubConnection hubConnection)
        {
            Connection = hubConnection;

            SetupRpcs();

            await Connection.StartAsync();
        }

        private void SetupRpcs()
        {
            Connection.On<PersonalFullUserInfo, MinUserInfo[], MinUserInfo[], ActiveRoomState>(
                "InitGame",
                (p, y, t, a) =>
                {
                    _logger.LogInformation($"init game called on {Id} with\n" +
                                           $"personal info is {JsonConvert.SerializeObject(p, Formatting.Indented)}\n" +
                                           $"yesterday champs are {JsonConvert.SerializeObject(y, Formatting.Indented)}\n" +
                                           $"topFriends are {JsonConvert.SerializeObject(t, Formatting.Indented)}\n" +
                                           $"with active room state {JsonConvert.SerializeObject(a, Formatting.Indented)}\n");
                });

            Connection.On<int, int, List<FullUserInfo>, int>("PrepareRequestedRoomRpc",
                (betChoice, capacityChoice, oppos, turn) =>
                    _logger.LogInformation($"PrepareRequestedRoomRpc called on {Id} with\n" +
                                           $"oppo info are {JsonConvert.SerializeObject(oppos, Formatting.Indented)}\n" +
                                           $"and turn {turn}\n"));


            Connection.On<ThrowResult>("MyThrowResult", res =>
                _logger.LogInformation($"MyThrowResult is called on {Id} with\n" +
                                       $"with res {JsonConvert.SerializeObject(res, Formatting.Indented)}\n" +
                                       $"with values {JsonConvert.SerializeObject(res.EatenCardsIds.Select(ConvertCardIndexToValue), Formatting.Indented)}\n"));

            Connection.On<List<int>, List<int>>("StartGameRpc", (hand, ground) =>
                _logger.LogInformation
                (
                    $"StartGameRpc is called on {Id},\n" +
                    $"hand is: {string.Join(", ", hand)} with values {string.Join(", ", hand.Select(ConvertCardIndexToValue))}\n" +
                    $"ground is: {string.Join(", ", ground)} with values {string.Join(", ", ground.Select(ConvertCardIndexToValue))}\n"
                ));

            Connection.On<int[]>("Distribute", hand =>
                _logger.LogInformation
                (
                    $"Distribute is called on {Id}\n" +
                    $"my hand is: {string.Join(", ", hand)}\n" +
                    $"with values {string.Join(", ", hand.Select(v => ConvertCardIndexToValue(v)))}\n"
                ));

            Connection.On<ThrowResult>("ForcePlay",
                res =>
                {
                    _logger.LogInformation($"ForcePlay is called on client {Id}\n" +
                                           $"with card {res.ThrownCard} with value {ConvertCardIndexToValue(res.ThrownCard)}\n" +
                                           $"and full res {JsonConvert.SerializeObject(res, Formatting.Indented)}\n" +
                                           $"with values {JsonConvert.SerializeObject(res.EatenCardsIds.Select(_ => ConvertCardIndexToValue(_)), Formatting.Indented)}\n");
                });

            Connection.On<ThrowResult>("CurrentOppoThrow",
                res =>
                {
                    _logger.LogInformation($"CurrentOppoThrow is called on client {Id}\n" +
                                           $"with card {res.ThrownCard} with value {ConvertCardIndexToValue(res.ThrownCard)}\n" +
                                           $"and full res {JsonConvert.SerializeObject(res, Formatting.Indented)}\n" +
                                           $"with values {JsonConvert.SerializeObject(res.EatenCardsIds.Select(_ => ConvertCardIndexToValue(_)), Formatting.Indented)}\n");
                });


            Connection.On<PersonalFullUserInfo>("UpdatePersonalInfo", info =>
                _logger.LogInformation($"UpdatePersonalInfo is called on {Id} with\n" +
                                       $"info is {JsonConvert.SerializeObject(info, Formatting.Indented)}\n"));

            Connection.On<FinalizeResult>("FinalizeRoom", (res) =>
                _logger.LogInformation($"UpdatePersonalInfo is called on {Id} with\n" +
                                       $"res {JsonConvert.SerializeObject(res, Formatting.Indented)}\n"));

            Connection.On<int, string>("ShowMessage", (sender, msgId) =>
                _logger.LogInformation($"ShowMessage is called on {Id} with\n" +
                                       $"sender {sender}\n" +
                                       $"message id {msgId}\n"));

            Connection.On<MinUserInfo>("ChallengeRequest", (sender) =>
                _logger.LogInformation($"ShowMessage is called on {Id} with\n" +
                                       $"sender {JsonConvert.SerializeObject(sender)}\n"));

            Connection.On<int>("RespondChallenge", (response) =>
                _logger.LogInformation($"ShowMessage is called on {Id} with\n" +
                                       $"response is {response}\n"));


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

        public static string[] Rpcs =
        {
            "GetPersonalUserData",
            "GetUserData",
            "RequestRandomRoom",
            "Ready",
            "AskForMoneyAid",
            "ClaimMoneyAid",
            "BuyCardback",
            "BuyBackground",
            "SelectCardback",
            "Throw",
            "InformTurnTimeout",
            "Surrender",
            "GetFullRoomState",
            "BuieTest",
            "ThrowExc",
            "ShowMessage"
        };
    }
}