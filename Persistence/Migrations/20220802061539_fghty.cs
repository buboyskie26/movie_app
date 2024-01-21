using Microsoft.EntityFrameworkCore.Migrations;

namespace Persistence.Migrations
{
    public partial class fghty : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "ShoppingVoucherUserid",
                table: "ShoppingVouchers",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_ShoppingVouchers_ShoppingVoucherUserid",
                table: "ShoppingVouchers",
                column: "ShoppingVoucherUserid");

            migrationBuilder.AddForeignKey(
                name: "FK_ShoppingVouchers_AspNetUsers_ShoppingVoucherUserid",
                table: "ShoppingVouchers",
                column: "ShoppingVoucherUserid",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ShoppingVouchers_AspNetUsers_ShoppingVoucherUserid",
                table: "ShoppingVouchers");

            migrationBuilder.DropIndex(
                name: "IX_ShoppingVouchers_ShoppingVoucherUserid",
                table: "ShoppingVouchers");

            migrationBuilder.DropColumn(
                name: "ShoppingVoucherUserid",
                table: "ShoppingVouchers");
        }
    }
}
