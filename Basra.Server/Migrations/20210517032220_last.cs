using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Basra.Server.Migrations
{
    public partial class last : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Users",
                columns: table => new
                {
                    Id = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Fbid = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PlayedGames = table.Column<int>(type: "int", nullable: false),
                    Wins = table.Column<int>(type: "int", nullable: false),
                    Draws = table.Column<int>(type: "int", nullable: false),
                    Money = table.Column<int>(type: "int", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Level = table.Column<int>(type: "int", nullable: false),
                    XP = table.Column<int>(type: "int", nullable: false),
                    RequestedMoneyAidToday = table.Column<int>(type: "int", nullable: false),
                    IsMoneyAidClaimable = table.Column<bool>(type: "bit", nullable: false),
                    IsMoneyAimProcessing = table.Column<bool>(type: "bit", nullable: false),
                    LastMoneyAimRequestTime = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Users", x => x.Id);
                });

            migrationBuilder.InsertData(
                table: "Users",
                columns: new[] { "Id", "Draws", "Fbid", "IsMoneyAidClaimable", "IsMoneyAimProcessing", "LastMoneyAimRequestTime", "Level", "Money", "Name", "PlayedGames", "RequestedMoneyAidToday", "Wins", "XP" },
                values: new object[,]
                {
                    { "0", 0, "0", false, false, new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 0, 0, "hany", 3, 0, 4, 0 },
                    { "1", 0, "1", false, false, new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 0, 0, "samy", 7, 0, 11, 0 },
                    { "2", 0, "2", false, false, new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 0, 0, "anni", 9, 0, 1, 0 },
                    { "3", 0, "3", false, false, new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 0, 0, "ali", 2, 0, 0, 0 }
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Users");
        }
    }
}
