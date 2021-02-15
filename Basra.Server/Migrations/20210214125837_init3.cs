using Microsoft.EntityFrameworkCore.Migrations;

namespace Basra.Server.Migrations
{
    public partial class init3 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "PendingRooms",
                columns: new[] { "Id", "EnteredUsers", "Genre", "RoomId", "UserCount" },
                values: new object[] { 1, 0, 0, "1", 0 });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "PendingRooms",
                keyColumn: "Id",
                keyValue: 1);
        }
    }
}
