using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Basra.Server.Migrations
{
    public partial class init : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Users",
                columns: table => new
                {
                    Id = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Fbid = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PlayedRoomsCount = table.Column<int>(type: "int", nullable: false),
                    WonRoomsCount = table.Column<int>(type: "int", nullable: false),
                    Draws = table.Column<int>(type: "int", nullable: false),
                    Money = table.Column<int>(type: "int", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PictureUrl = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    EatenCardsCount = table.Column<int>(type: "int", nullable: false),
                    WinStreak = table.Column<int>(type: "int", nullable: false),
                    BasraCount = table.Column<int>(type: "int", nullable: false),
                    BigBasraCount = table.Column<int>(type: "int", nullable: false),
                    TotalEarnedMoney = table.Column<int>(type: "int", nullable: false),
                    Level = table.Column<int>(type: "int", nullable: false),
                    XP = table.Column<int>(type: "int", nullable: false),
                    RequestedMoneyAidToday = table.Column<int>(type: "int", nullable: false),
                    LastMoneyAimRequestTime = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsMoneyAidClaimable = table.Column<bool>(type: "bit", nullable: false),
                    OwnedBackgroundIds = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    OwnedCardBackIds = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    OwnedTitleIds = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    SelectedTitleId = table.Column<int>(type: "int", nullable: false),
                    SelectedCardback = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Users", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "UserRelation",
                columns: table => new
                {
                    UserId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    OtherUserId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    RelationType = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserRelation", x => new { x.UserId, x.OtherUserId });
                    table.ForeignKey(
                        name: "FK_UserRelation_Users_OtherUserId",
                        column: x => x.OtherUserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_UserRelation_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.InsertData(
                table: "Users",
                columns: new[] { "Id", "BasraCount", "BigBasraCount", "Draws", "EatenCardsCount", "Fbid", "IsMoneyAidClaimable", "LastMoneyAimRequestTime", "Level", "Money", "Name", "OwnedBackgroundIds", "OwnedCardBackIds", "OwnedTitleIds", "PictureUrl", "PlayedRoomsCount", "RequestedMoneyAidToday", "SelectedCardback", "SelectedTitleId", "TotalEarnedMoney", "WinStreak", "WonRoomsCount", "XP" },
                values: new object[,]
                {
                    { "0", 0, 0, 3, 0, "0", false, null, 13, 22250, "hany", "[1,3]", "[0,2]", "[2,4]", "https://pbs.twimg.com/profile_images/592734306725933057/s4-h_LQC.jpg", 3, 2, 2, 0, 0, 0, 4, 806 },
                    { "1", 0, 0, 1, 0, "1", false, null, 43, 89000, "samy", "[0,9]", "[0,1,2]", "[11,6]", "https://d3g9pb5nvr3u7.cloudfront.net/authors/57ea8955d8de1e1602f67ca0/1902081322/256.jpg", 7, 0, 1, 0, 0, 0, 11, 1983 },
                    { "2", 0, 0, 37, 0, "2", false, null, 139, 8500, "anni", "[10,8]", "[4,9]", "[1,3]", "https://pbs.twimg.com/profile_images/633661532350623745/8U1sJUc8_400x400.png", 973, 4, 4, 0, 0, 0, 192, 8062 },
                    { "3", 0, 0, 1, 0, "3", true, null, 4, 3, "ali", "[10,8]", "[2,4,8]", "[1,3]", "https://pbs.twimg.com/profile_images/723902674970750978/p8JWhWxP_400x400.jpg", 6, 3, 2, 0, 0, 0, 2, 12 }
                });

            migrationBuilder.CreateIndex(
                name: "IX_UserRelation_OtherUserId",
                table: "UserRelation",
                column: "OtherUserId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "UserRelation");

            migrationBuilder.DropTable(
                name: "Users");
        }
    }
}
