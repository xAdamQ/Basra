using Microsoft.EntityFrameworkCore.Migrations;

namespace Basra.Server.Migrations.Game
{
    public partial class userData4 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Disconncted",
                table: "User");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "Disconncted",
                table: "User",
                type: "tinyint(1)",
                nullable: true);
        }
    }
}
