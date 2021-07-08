using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Basra.Models.Client;
using Basra.Server.Helpers;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Logging;

namespace Basra.Server.Services
{
    public interface IFinalizeManager
    {
        Task FinalizeRoom(Room room, RoomUser resignedUser = null);
    }

    public class FinalizeManager : IFinalizeManager
    {
        private readonly IHubContext<MasterHub> _masterHub;
        private readonly IMasterRepo _masterRepo;
        private readonly ISessionRepo _sessionRepo;
        private readonly ILogger<FinalizeManager> _logger;
        public FinalizeManager(IHubContext<MasterHub> masterHub, IMasterRepo masterRepo, ISessionRepo sessionRepo,
            ILogger<FinalizeManager> logger)
        {
            _masterHub = masterHub;
            _masterRepo = masterRepo;
            _sessionRepo = sessionRepo;
            _logger = logger;
        }

        public async Task FinalizeRoom(Room room, RoomUser resignedUser = null)
        {
            LastEat(room);

            room.SetUsersDomains(typeof(UserDomain.App.Room.FinishedRoom)); //does this has usage or u can just use lobby idle? 

            var roomDataUsers = await _masterRepo.GetUsersByIds(room.RoomActors.Select(_ => _.Id).ToList());
            //todo test if the result is the same order as roomUsers

            var scores = CalcScores(room.RoomActors, resignedUser);
            var xpReports = UpdateUserStates(room, roomDataUsers, scores);
            await Task.WhenAll(roomDataUsers.Select(u => LevelWorks(u)));

            await _masterRepo.SaveChangesAsync();

            await SendFinalizeResult(room.RoomUsers, roomDataUsers, xpReports, room.LastEater.TurnId);

            RemoveDisconnectedUsers(room.RoomUsers);

            room.RoomUsers.ForEach(ru => _sessionRepo.DeleteRoomUser(ru));
            _sessionRepo.DeleteRoom(room);

            room.SetUsersDomains(typeof(UserDomain.App.Lobby.Idle));
        }

        /// <summary>
        /// score for resigned user is -1
        /// </summary> 
        private List<int> CalcScores(List<RoomActor> roomActors, RoomUser resignedUser = null)
        {
            var lastedUsers = resignedUser == null ? roomActors : roomActors.Where(_ => _ != resignedUser).ToList();

            var biggestEatenCount = lastedUsers.Max(u => u.EatenCardsCount);
            var biggestEaters = lastedUsers.Where(u => u.EatenCardsCount == biggestEatenCount).ToArray();

            var scores = new List<int>();

            foreach (var roomActor in roomActors)
            {
                if (roomActor == resignedUser)
                {
                    scores.Add(-1);
                    continue;
                }

                scores.Add(roomActor.BasraCount * 10 +
                           roomActor.BigBasraCount * 30 +
                           (biggestEaters.Contains(roomActor) ? 30 : 0));
            }

            return scores;
        }

        /// <returns> the added xp and for what </returns>
        private List<RoomXpReport> UpdateUserStates(Room room, List<User> dataUsers, List<int> scores)
        {
            var xpReports = new List<RoomXpReport>();
            for (int i = 0; i < room.Capacity; i++) xpReports.Add(new RoomXpReport());

            var betWithoutTicket = (int) (room.Bet / 1.1f);
            var totalBet = betWithoutTicket * room.Capacity;
            var maxScore = scores.Max();
            var betXp = CalcBetXp(room.BetChoice);

            var winnerIndices = scores.Select((score, i) => score == maxScore ? i : -1)
                .Where(scoreIndex => scoreIndex != -1)
                .ToList();
            var loserIndices = Enumerable.Range(0, room.Capacity)
                .Where(i => !winnerIndices.Contains(i))
                .ToList();
            var resignedUserIndex = scores.IndexOf(-1); //if no resign it retrun -1

            //drawers
            if (winnerIndices.Count > 1)
            {
                var moneyPart = totalBet / winnerIndices.Count;
                foreach (var userIndex in winnerIndices)
                {
                    var dUser = dataUsers[userIndex];
                    dUser.Draws++;
                    dUser.Money += moneyPart;
                    dUser.TotalEarnedMoney += moneyPart;

                    dUser.XP += xpReports[userIndex].Competition = (int) Room.DrawXpPercent * betXp;
                }
            }
            //winner
            else
            {
                var dUser = dataUsers[winnerIndices[0]];
                dUser.WonRoomsCount++;
                dUser.Money += totalBet;
                dUser.TotalEarnedMoney += totalBet;
                dUser.WinStreak++;

                dUser.XP += xpReports[0].Competition = (int) Room.WinXpPercent * betXp;
            }


            //losers
            foreach (var loserIndex in loserIndices)
            {
                var dUser = dataUsers[loserIndex];
                if (loserIndex != resignedUserIndex)
                    dUser.XP += xpReports[loserIndex].Competition = (int) Room.LoseXpPercent * betXp;

                dUser.WinStreak = 0;
            }

            for (int i = 0; i < room.Capacity; i++)
            {
                var dUser = dataUsers[i];
                var roomActor = room.RoomActors[i];

                dUser.PlayedRoomsCount++;

                dUser.EatenCardsCount += roomActor.EatenCardsCount;
                dUser.BasraCount += roomActor.BasraCount;
                dUser.BigBasraCount += roomActor.BigBasraCount;

                if (i != resignedUserIndex)
                {
                    dUser.XP += xpReports[i].Basra = roomActor.BasraCount * (int) Room.BasraXpPercent * betXp;
                    dUser.XP += xpReports[i].BigBasra = roomActor.BigBasraCount * (int) Room.BigBasraXpPercent * betXp;

                    if (roomActor.EatenCardsCount > Room.GreatEatThreshold)
                        dataUsers[i].XP += xpReports[i].GreatEat = (int) Room.GreatEatXpPercent * betXp;
                }
            }

            return xpReports;
        }

        private int CalcBetXp(int betChoice) => (int) (100 * MathF.Pow(betChoice, 1.4f)) + 100;

        private void LastEat(Room room)
        {
            room.LastEater.EatenCardsCount += room.GroundCards.Count;

            room.GroundCards.Clear();
        }

        /// <summary>
        /// check current level againest xp to level up and send to client
        /// functions that takes data user as param dosn't save changes
        /// </summary>
        private async Task LevelWorks(User roomDataUser) //separate this to be called on every XP change 
        {
            var calcedLevel = Room.GetLevelFromXp(roomDataUser.XP);
            if (calcedLevel > roomDataUser.Level)
            {
                var increasedLevels = calcedLevel - roomDataUser.Level;
                var totalMoneyReward = 0;
                for (int j = 0; j < increasedLevels; j++)
                {
                    totalMoneyReward += 100;
                    //todo give level up rewards (money equation), add to test
                    //todo test this function logic
                }

                roomDataUser.Level = calcedLevel;
                roomDataUser.Money += totalMoneyReward;

                await _masterHub.Clients.User(roomDataUser.Id)
                    .SendAsync("LevelUp", calcedLevel, totalMoneyReward);
            }
        }


        private async Task SendFinalizeResult(List<RoomUser> roomUsers, List<User> roomDataUsers, List<RoomXpReport> roomXpReports, int
            lastEaterTurnId)
        {
            var dUsersMapped = roomUsers.Join(roomDataUsers, ru => ru.Id, du => du.Id,
                (_, du) => du).ToList();
            var xpRepMapped = roomUsers.Join(roomXpReports, rUser => rUser.TurnId,
                roomXpReports.IndexOf, (_, report) => report).ToList();

            var finalizeTasks = new List<Task>();
            for (int i = 0; i < roomUsers.Count; i++)
            {
                var finalizeResult = new FinalizeResult
                {
                    RoomXpReport = xpRepMapped[i],
                    PersonalFullUserInfo = Mapper.ConvertUserDataToClient(dUsersMapped[i]),
                    LastEaterTurnId = lastEaterTurnId,
                };

                finalizeTasks.Add(_masterHub.Clients.User(roomUsers[i].Id)
                    .SendAsync("FinalizeRoom", finalizeResult));
            }

            _logger.LogInformation("finalize called");

            await Task.WhenAll(finalizeTasks);
        }


        private void RemoveDisconnectedUsers(List<RoomUser> roomUsers)
        {
            foreach (var roomUser in roomUsers.Where(ru => ru.ActiveUser.Disconnected))
                //where filtered with new collection, I don't know the performance but I will see how
                //linq works under the hood, because I think the created collection doesn't affect performance
                _sessionRepo.RemoveActiveUser(roomUser.ActiveUser.Id);
        }
    }
}