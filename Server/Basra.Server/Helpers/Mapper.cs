using System;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Basra.Models.Client;

namespace Basra.Server.Helpers
{
    public static class Mapper
    {
        public static PersonalFullUserInfo ConvertUserDataToClient(User user)
        {
            return new()
            {
                Money = user.Money,
                PlayedRoomsCount = user.PlayedRoomsCount,
                Name = user.Name,
                PictureUrl = user.PictureUrl,
                WonRoomsCount = user.WonRoomsCount,
                MoneyAimTimeLeft =
                    user.LastMoneyAimRequestTime == null ? null : DateTime.UtcNow - user.LastMoneyAimRequestTime,
                OwnedCardBackIds = user.OwnedCardBackIds,
                SelectedCardback = user.SelectedCardback,
                DrawRoomsCount = user.Draws,
            };
        }

        public static Expression<Func<User, FullUserInfo>> UserToFullUserInfoProjection => u => new FullUserInfo
        {
            Id = u.Id,
            Name = u.Name,
            PlayedRoomsCount = u.PlayedRoomsCount,
            WonRoomsCount = u.WonRoomsCount,
            EatenCardsCount = u.EatenCardsCount,
            WinStreak = u.WinStreak,
            BasraCount = u.BasraCount,
            BigBasraCount = u.BigBasraCount,
            OwnedBackgroundsIds = u.OwnedBackgroundIds,
            OwnedCardBackIds = u.OwnedCardBackIds,
            TotalEarnedMoney = u.TotalEarnedMoney,
            Level = u.Level,
            SelectedTitleId = u.SelectedTitleId,
            PictureUrl = u.PictureUrl
        };

        public static Func<User, FullUserInfo> UserToFullUserInfoFunc = UserToFullUserInfoProjection.Compile();
    }
}