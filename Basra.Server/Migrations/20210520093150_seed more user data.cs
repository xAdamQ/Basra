using Microsoft.EntityFrameworkCore.Migrations;

namespace Basra.Server.Migrations
{
    public partial class seedmoreuserdata : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: "0",
                columns: new[] { "Draws", "Level", "Money", "OwnedCardBackIds", "PictureUrl", "RequestedMoneyAidToday", "XP" },
                values: new object[] { 3, 13, 850, "[0,2]", "https://pbs.twimg.com/profile_images/592734306725933057/s4-h_LQC.jpg", 2, 806 });

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: "1",
                columns: new[] { "Draws", "Level", "Money", "OwnedCardBackIds", "PictureUrl", "XP" },
                values: new object[] { 1, 43, 2000, "[0,1,2]", "https://d3g9pb5nvr3u7.cloudfront.net/authors/57ea8955d8de1e1602f67ca0/1902081322/256.jpg", 1983 });

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: "2",
                columns: new[] { "Draws", "Level", "Money", "PictureUrl", "PlayedGames", "RequestedMoneyAidToday", "Wins", "XP" },
                values: new object[] { 37, 139, 8500, "https://pbs.twimg.com/profile_images/633661532350623745/8U1sJUc8_400x400.png", 973, 4, 192, 8062 });

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: "3",
                columns: new[] { "Draws", "IsMoneyAidClaimable", "Level", "Money", "OwnedBackgroundIds", "OwnedCardBackIds", "OwnedTitleIds", "PictureUrl", "PlayedGames", "RequestedMoneyAidToday", "Wins", "XP" },
                values: new object[] { 1, true, 4, 3, "[10,8]", "[4,9]", "[1,3]", "https://pbs.twimg.com/profile_images/723902674970750978/p8JWhWxP_400x400.jpg", 6, 3, 2, 12 });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: "0",
                columns: new[] { "Draws", "Level", "Money", "OwnedCardBackIds", "PictureUrl", "RequestedMoneyAidToday", "XP" },
                values: new object[] { 0, 0, 0, null, null, 0, 0 });

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: "1",
                columns: new[] { "Draws", "Level", "Money", "OwnedCardBackIds", "PictureUrl", "XP" },
                values: new object[] { 0, 0, 0, null, null, 0 });

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: "2",
                columns: new[] { "Draws", "Level", "Money", "PictureUrl", "PlayedGames", "RequestedMoneyAidToday", "Wins", "XP" },
                values: new object[] { 0, 0, 0, null, 9, 0, 1, 0 });

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: "3",
                columns: new[] { "Draws", "IsMoneyAidClaimable", "Level", "Money", "OwnedBackgroundIds", "OwnedCardBackIds", "OwnedTitleIds", "PictureUrl", "PlayedGames", "RequestedMoneyAidToday", "Wins", "XP" },
                values: new object[] { 0, false, 0, 0, null, null, null, null, 2, 0, 0, 0 });
        }
    }
}
