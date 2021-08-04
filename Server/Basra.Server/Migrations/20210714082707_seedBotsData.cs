using Microsoft.EntityFrameworkCore.Migrations;

namespace Basra.Server.Migrations
{
    public partial class seedBotsData : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "Users",
                columns: new[] { "Id", "BasraCount", "BigBasraCount", "Draws", "EatenCardsCount", "Fbid", "IsMoneyAidClaimable", "LastMoneyAimRequestTime", "Level", "Money", "Name", "OwnedBackgroundIds", "OwnedCardBackIds", "OwnedTitleIds", "PictureUrl", "PlayedRoomsCount", "RequestedMoneyAidToday", "SelectedCardback", "SelectedTitleId", "TotalEarnedMoney", "WinStreak", "WonRoomsCount", "XP" },
                values: new object[] { "999", 0, 0, 2, 0, null, false, null, 7, 1000, "botA", "[0,3]", "[8]", "[1]", "https://pbs.twimg.com/profile_images/723902674970750978/p8JWhWxP_400x400.jpg", 9, 0, 1, 0, 0, 0, 2, 34 });

            migrationBuilder.InsertData(
                table: "Users",
                columns: new[] { "Id", "BasraCount", "BigBasraCount", "Draws", "EatenCardsCount", "Fbid", "IsMoneyAidClaimable", "LastMoneyAimRequestTime", "Level", "Money", "Name", "OwnedBackgroundIds", "OwnedCardBackIds", "OwnedTitleIds", "PictureUrl", "PlayedRoomsCount", "RequestedMoneyAidToday", "SelectedCardback", "SelectedTitleId", "TotalEarnedMoney", "WinStreak", "WonRoomsCount", "XP" },
                values: new object[] { "9999", 0, 0, 2, 0, null, false, null, 8, 1100, "botB", "[3]", "[0,8]", "[0,1]", "https://pbs.twimg.com/profile_images/723902674970750978/p8JWhWxP_400x400.jpg", 11, 0, 2, 0, 0, 0, 3, 44 });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Users",
                keyColumn: "Id",
                keyValue: "999");

            migrationBuilder.DeleteData(
                table: "Users",
                keyColumn: "Id",
                keyValue: "9999");
        }
    }
}
