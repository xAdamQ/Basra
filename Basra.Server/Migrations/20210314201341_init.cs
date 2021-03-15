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
                    Id = table.Column<string>(type: "varchar(255)", nullable: false),
                    Fbid = table.Column<string>(type: "longtext", nullable: true),
                    PlayedGames = table.Column<int>(type: "int", nullable: false),
                    Wins = table.Column<int>(type: "int", nullable: false),
                    Draws = table.Column<int>(type: "int", nullable: false),
                    Money = table.Column<int>(type: "int", nullable: false),
                    Name = table.Column<string>(type: "longtext", nullable: true),
                    Email = table.Column<string>(type: "longtext", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Users", x => x.Id);
                });

            migrationBuilder.InsertData(
                table: "Users",
                columns: new[] { "Id", "Draws", "Email", "Fbid", "Money", "Name", "PlayedGames", "Wins" },
                values: new object[,]
                {
                    { "0", 0, null, "0", 0, "hany", 3, 4 },
                    { "1", 0, null, "1", 0, "samy", 7, 11 },
                    { "2", 0, null, "2", 0, "anni", 9, 1 },
                    { "3", 0, null, "3", 0, "ali", 2, 0 }
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Users");
        }
    }
}
