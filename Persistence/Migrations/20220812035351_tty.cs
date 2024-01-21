using Microsoft.EntityFrameworkCore.Migrations;

namespace Persistence.Migrations
{
    public partial class tty : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "isClicked",
                table: "MovieOutOfStocks",
                newName: "IsClicked");

            migrationBuilder.AddColumn<bool>(
                name: "IsOutOfStock",
                table: "MovieOutOfStocks",
                nullable: false,
                defaultValue: false);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsOutOfStock",
                table: "MovieOutOfStocks");

            migrationBuilder.RenameColumn(
                name: "IsClicked",
                table: "MovieOutOfStocks",
                newName: "isClicked");
        }
    }
}
