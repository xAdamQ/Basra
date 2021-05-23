using Microsoft.EntityFrameworkCore.Migrations;

namespace Basra.Server.Migrations
{
    public partial class addowneditemslists : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsMoneyAidProcessing",
                table: "Users");

            migrationBuilder.AddColumn<string>(
                name: "OwnedBackgroundIds",
                table: "Users",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "OwnedCardBackIds",
                table: "Users",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "OwnedTitleIds",
                table: "Users",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: "0",
                columns: new[] { "OwnedBackgroundIds", "OwnedTitleIds" },
                values: new object[] { "[1,3]", "[2,4]" });

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: "1",
                columns: new[] { "OwnedBackgroundIds", "OwnedTitleIds" },
                values: new object[] { "[0,9]", "[11,6]" });

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: "2",
                columns: new[] { "OwnedBackgroundIds", "OwnedCardBackIds", "OwnedTitleIds" },
                values: new object[] { "[10,8]", "[4,9]", "[1,3]" });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "OwnedBackgroundIds",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "OwnedCardBackIds",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "OwnedTitleIds",
                table: "Users");

            migrationBuilder.AddColumn<bool>(
                name: "IsMoneyAidProcessing",
                table: "Users",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }
    }
}
