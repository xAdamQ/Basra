using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Basra.Server.Migrations
{
    public partial class init2 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Rooms",
                columns: table => new
                {
                    Id = table.Column<string>(type: "varchar(255)", nullable: false),
                    Genre = table.Column<int>(type: "int", nullable: false),
                    CurrentTurn = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Rooms", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Users",
                columns: table => new
                {
                    Id = table.Column<string>(type: "varchar(255)", nullable: false),
                    Fbid = table.Column<string>(type: "longtext", nullable: true),
                    PlayedGames = table.Column<int>(type: "int", nullable: false),
                    Wins = table.Column<int>(type: "int", nullable: false),
                    Draws = table.Column<int>(type: "int", nullable: false),
                    Money = table.Column<int>(type: "int", nullable: false),
                    Name = table.Column<string>(type: "longtext", nullable: true),
                    Email = table.Column<string>(type: "longtext", nullable: true),
                    IsActive = table.Column<bool>(type: "tinyint(1)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Users", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "PendingRooms",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    Genre = table.Column<int>(type: "int", nullable: false),
                    UserCount = table.Column<int>(type: "int", nullable: false),
                    EnteredUsers = table.Column<int>(type: "int", nullable: false),
                    RoomId = table.Column<string>(type: "varchar(255)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PendingRooms", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PendingRooms_Rooms_RoomId",
                        column: x => x.RoomId,
                        principalTable: "Rooms",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "RoomUsers",
                columns: table => new
                {
                    Id = table.Column<string>(type: "varchar(255)", nullable: false),
                    ActiveRoomId = table.Column<string>(type: "varchar(255)", nullable: true),
                    Score = table.Column<int>(type: "int", nullable: false),
                    BasraCount = table.Column<int>(type: "int", nullable: false),
                    BigBasraCount = table.Column<int>(type: "int", nullable: false),
                    EatenCardsCount = table.Column<int>(type: "int", nullable: false),
                    IsReady = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    ConnectionId = table.Column<string>(type: "longtext", nullable: true),
                    UserId = table.Column<string>(type: "varchar(255)", nullable: true),
                    RoomId = table.Column<string>(type: "varchar(255)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RoomUsers", x => x.Id);
                    table.ForeignKey(
                        name: "FK_RoomUsers_Rooms_ActiveRoomId",
                        column: x => x.ActiveRoomId,
                        principalTable: "Rooms",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_RoomUsers_Rooms_RoomId",
                        column: x => x.RoomId,
                        principalTable: "Rooms",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_RoomUsers_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.InsertData(
                table: "Rooms",
                columns: new[] { "Id", "CurrentTurn", "Genre" },
                values: new object[,]
                {
                    { "0", 0, 0 },
                    { "1", 0, 0 }
                });

            migrationBuilder.InsertData(
                table: "Users",
                columns: new[] { "Id", "Draws", "Email", "Fbid", "IsActive", "Money", "Name", "PlayedGames", "Wins" },
                values: new object[,]
                {
                    { "0", 0, null, "0", false, 0, "hany", 3, 4 },
                    { "1", 0, null, "1", false, 0, "samy", 7, 11 },
                    { "2", 0, null, "2", false, 0, "anni", 9, 1 },
                    { "3", 0, null, "3", false, 0, "ali", 2, 0 }
                });

            migrationBuilder.InsertData(
                table: "RoomUsers",
                columns: new[] { "Id", "ActiveRoomId", "BasraCount", "BigBasraCount", "ConnectionId", "EatenCardsCount", "IsReady", "RoomId", "Score", "UserId" },
                values: new object[] { "1", null, 0, 0, null, 0, false, "1", 0, "0" });

            migrationBuilder.InsertData(
                table: "RoomUsers",
                columns: new[] { "Id", "ActiveRoomId", "BasraCount", "BigBasraCount", "ConnectionId", "EatenCardsCount", "IsReady", "RoomId", "Score", "UserId" },
                values: new object[] { "0", null, 0, 0, null, 0, false, "0", 0, "3" });

            migrationBuilder.CreateIndex(
                name: "IX_PendingRooms_RoomId",
                table: "PendingRooms",
                column: "RoomId");

            migrationBuilder.CreateIndex(
                name: "IX_RoomUsers_ActiveRoomId",
                table: "RoomUsers",
                column: "ActiveRoomId");

            migrationBuilder.CreateIndex(
                name: "IX_RoomUsers_RoomId",
                table: "RoomUsers",
                column: "RoomId");

            migrationBuilder.CreateIndex(
                name: "IX_RoomUsers_UserId",
                table: "RoomUsers",
                column: "UserId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "PendingRooms");

            migrationBuilder.DropTable(
                name: "RoomUsers");

            migrationBuilder.DropTable(
                name: "Rooms");

            migrationBuilder.DropTable(
                name: "Users");
        }
    }
}
