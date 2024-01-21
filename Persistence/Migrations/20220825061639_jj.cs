using Microsoft.EntityFrameworkCore.Migrations;

namespace Persistence.Migrations
{
    public partial class jj : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "MovieRefId",
                table: "MessageUsersTables");

            migrationBuilder.AddColumn<int>(
                name: "MovieRefId",
                table: "MessageTables",
                nullable: false,
                defaultValue: 0);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "MovieRefId",
                table: "MessageTables");

            migrationBuilder.AddColumn<int>(
                name: "MovieRefId",
                table: "MessageUsersTables",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }
    }
}
