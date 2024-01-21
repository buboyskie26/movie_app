using Microsoft.EntityFrameworkCore.Migrations;

namespace Persistence.Migrations
{
    public partial class rtyy : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ShoppingVouchers_ShoppingCartItems_ShoppingCartItemId",
                table: "ShoppingVouchers");

            migrationBuilder.AddForeignKey(
                name: "FK_ShoppingVouchers_ShoppingCartItems_ShoppingCartItemId",
                table: "ShoppingVouchers",
                column: "ShoppingCartItemId",
                principalTable: "ShoppingCartItems",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ShoppingVouchers_ShoppingCartItems_ShoppingCartItemId",
                table: "ShoppingVouchers");

            migrationBuilder.AddForeignKey(
                name: "FK_ShoppingVouchers_ShoppingCartItems_ShoppingCartItemId",
                table: "ShoppingVouchers",
                column: "ShoppingCartItemId",
                principalTable: "ShoppingCartItems",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
