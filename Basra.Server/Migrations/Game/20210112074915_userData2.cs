using Microsoft.EntityFrameworkCore.Migrations;

namespace Basra.Server.Migrations.Game
{
    public partial class userData2 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Id",
                table: "User",
                newName: "IdentityUserId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "IdentityUserId",
                table: "User",
                newName: "Id");
        }
    }
}
