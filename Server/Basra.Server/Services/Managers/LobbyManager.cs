using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Basra.Server.Exceptions;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Logging;
using System.Linq;
using AutoMapper;
using Basra.Models.Client;
using Hangfire;

namespace Basra.Server.Services
{
    public interface ILobbyManager
    {
        Task RequestMoneyAid(ActiveUser activeUser);
        Task ClaimMoneyAim(ActiveUser activeUser);

        Task BuyCardBack(int cardbackId);
        Task BuyBackground(int backgroundId);
        Task SelectCardback(int cardbackId);
    }

    //todo split this to shop and other things for example
    public class LobbyManager : ILobbyManager
    {
        private readonly IMasterRepo _masterRepo;
        private readonly IBackgroundJobClient _backgroundJobClient;
        private readonly IRequestCache _requestCache;

        public LobbyManager(IMasterRepo masterRepo, IBackgroundJobClient backgroundJobClient,
            IRequestCache requestCache)
        {
            _masterRepo = masterRepo;
            _backgroundJobClient = backgroundJobClient;
            _requestCache = requestCache;
        } //todo try to eliminate room manager

        public async Task RequestMoneyAid(ActiveUser activeUser)
        {
            var user = await _masterRepo.GetUserByIdAsyc(activeUser.Id);
            if (user.IsMoneyAidProcessing)
                throw new BadUserInputException("the user requested money while there's a waiting request");
            if (user.RequestedMoneyAidToday >= 4)
                throw new BadUserInputException("the user was trying to request money aid above limit");
            if (user.Money >= Room.MinBet)
                throw new BadUserInputException("the user was trying to request money aid while he have enough money");
            //not tested because logic is trivial

            user.LastMoneyAimRequestTime = DateTime.UtcNow;
            user.RequestedMoneyAidToday++;

            await _masterRepo.SaveChangesAsync();

            _backgroundJobClient.Schedule(() => MakeMoneyAimClaimable(activeUser.Id), TimeSpan.FromMinutes(15));
        } //trivial to test, best in integration
        private async Task MakeMoneyAimClaimable(string userId)
        {
            var user = await _masterRepo.GetUserByIdAsyc(userId);

            user.IsMoneyAidClaimable = true;
            user.LastMoneyAimRequestTime = null;

            //we don't notify the client for 2 reasons:
            //he could be inactive and when he request/start he know the remaining time
            //when he checks for claimable flag he can claim
            await _masterRepo.SaveChangesAsync();
        } //issue changes, no test

        public async Task ClaimMoneyAim(ActiveUser activeUser)
        {
            var user = await _masterRepo.GetUserByIdAsyc(activeUser.Id);

            if (!user.IsMoneyAidClaimable)
                throw new BadUserInputException("the user was trying to claim not claimable money aid");

            user.IsMoneyAidClaimable = false;
            user.Money += Room.MinBet;

            await _masterRepo.SaveChangesAsync();
            //I didn't send the user to let the client sync the state and figure out because the
            //situation is customized so not returning error in itself means the client can make the receive money
            //animation and ui update
        } //don't test

        //todo choose a better place to store this
        /// <summary>
        /// I can't remove items in the future, that's why their price order is the id
        /// </summary>
        private static readonly int[] CardbackPrices = {50, 65, 100, 450, 600, 700, 1800, 2000, 2600};
        /// <summary>
        /// I can't remove items in the future, that's why their price order is the id
        /// </summary>
        private static readonly int[] BackgroundPrices = {50, 65, 100, 450};

        public async Task BuyCardBack(int cardbackId)
        {
            var user = await _requestCache.GetUser();

            if (cardbackId >= CardbackPrices.Length)
                throw new BadUserInputException("client give cardback id exceed count");

            if (user.Money < CardbackPrices[cardbackId])
                throw new BadUserInputException("the client is trying to buy cardback without enough money");
            if (user.OwnedCardBackIds.Contains(cardbackId))
                throw new BadUserInputException("the client is trying to buy cardback that he already owns");

            user.Money -= CardbackPrices[cardbackId];
            user.OwnedCardBackIds.Add(cardbackId);
            //and here the issue is raised
            //(1) if the client got success result, then the money is taken and the card is bought and he can do this
            //logic of updating the money and unlock cardback
            //(2) but also I can sync the whole user data so the money will be updated and the cardback will be unlocked
            //so the difference between the 2 approached is (1) I know what is the result in the client
            //(2) I don't know, I will update the data
            //and to avoid updating the whole data you can pass the change name and value as return
            //which is something the client will expect also so this is meaningless

            await _masterRepo.SaveChangesAsync();
        }
        public async Task BuyBackground(int backgroundId)
        {
            var user = await _requestCache.GetUser();

            if (backgroundId >= CardbackPrices.Length)
                throw new BadUserInputException("client give background id exceed count");
            if (user.Money < BackgroundPrices[backgroundId])
                throw new BadUserInputException("the client is trying to buy background without enough money");
            if (user.OwnedBackgroundIds.Contains(backgroundId))
                throw new BadUserInputException("the client is trying to buy background that he already owns");

            user.Money -= BackgroundPrices[backgroundId];
            user.OwnedBackgroundIds.Add(backgroundId);

            await _masterRepo.SaveChangesAsync();
        }
        //I tested cardback bu bu it's applicable on both

        public async Task SelectCardback(int cardbackId)
        {
            var user = await _requestCache.GetUser();

            if (!user.OwnedCardBackIds.Contains(cardbackId))
                throw new BadUserInputException(
                    $"the user is trying to select a cardback {cardbackId} he doesn't own {string.Join(", ", user.OwnedCardBackIds)}");

            if (user.SelectedCardback == cardbackId) return;

            user.SelectedCardback = cardbackId;

            await _masterRepo.SaveChangesAsync();
        } //trivial to test
    }
}