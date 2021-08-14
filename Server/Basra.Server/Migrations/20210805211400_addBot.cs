using Microsoft.EntityFrameworkCore.Migrations;

namespace Basra.Server.Migrations
{
    public partial class addBot : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: "9999",
                column: "PictureUrl",
                value: "https://pbs.twimg.com/profile_images/592734306725933057/s4-h_LQC.jpg");

            migrationBuilder.InsertData(
                table: "Users",
                columns: new[] { "Id", "BasraCount", "BigBasraCount", "Draws", "EatenCardsCount", "Fbid", "LastMoneyAimRequestTime", "Level", "MaxWinStreak", "Money", "Name", "OwnedBackgroundIds", "OwnedCardBackIds", "OwnedTitleIds", "PictureUrl", "PlayedRoomsCount", "RequestedMoneyAidToday", "SelectedBackground", "SelectedCardback", "SelectedTitleId", "TotalEarnedMoney", "WinStreak", "WonRoomsCount", "XP" },
                values: new object[] { "99999", 0, 0, 2, 0, null, null, 8, 0, 1100, "botC", "[3]", "[0,8]", "[0,1]", "https://d3g9pb5nvr3u7.cloudfront.net/authors/57ea8955d8de1e1602f67ca0/1902081322/256.jpg", 11, 0, 0, 2, 0, 0, 0, 3, 44 });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Users",
                keyColumn: "Id",
                keyValue: "99999");

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: "9999",
                column: "PictureUrl",
                value: "https://pbs.twimg.com/profile_images/723902674970750978/p8JWhWxP_400x400.jpg");
        }
    }
}
