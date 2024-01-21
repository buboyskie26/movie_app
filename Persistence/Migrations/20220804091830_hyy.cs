using Microsoft.EntityFrameworkCore.Migrations;

namespace Persistence.Migrations
{
    public partial class hyy : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "UserHeHadMessageId",
                table: "MessageTables",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_MessageTables_UserHeHadMessageId",
                table: "MessageTables",
                column: "UserHeHadMessageId");

            migrationBuilder.AddForeignKey(
                name: "FK_MessageTables_AspNetUsers_UserHeHadMessageId",
                table: "MessageTables",
                column: "UserHeHadMessageId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_MessageTables_AspNetUsers_UserHeHadMessageId",
                table: "MessageTables");

            migrationBuilder.DropIndex(
                name: "IX_MessageTables_UserHeHadMessageId",
                table: "MessageTables");

            migrationBuilder.DropColumn(
                name: "UserHeHadMessageId",
                table: "MessageTables");
        }
    }
}
