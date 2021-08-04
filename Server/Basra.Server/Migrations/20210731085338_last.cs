using Microsoft.EntityFrameworkCore.Migrations;

namespace Basra.Server.Migrations
{
    public partial class last : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsMoneyAidClaimable",
                table: "Users");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsMoneyAidClaimable",
                table: "Users",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: "3",
                column: "IsMoneyAidClaimable",
                value: true);
        }
    }
}
