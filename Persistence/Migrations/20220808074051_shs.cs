using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Persistence.Migrations
{
    public partial class shs : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "DiscountedShop",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Quantity = table.Column<int>(nullable: false),
                    MinimumSpend = table.Column<int>(nullable: false),
                    VendorId = table.Column<string>(nullable: true),
                    Creation = table.Column<DateTime>(nullable: false),
                    Expire = table.Column<DateTime>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DiscountedShop", x => x.Id);
                    table.ForeignKey(
                        name: "FK_DiscountedShop_AspNetUsers_VendorId",
                        column: x => x.VendorId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "DiscountShop_Cart",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ShoppingCartItemId = table.Column<int>(nullable: false),
                    DiscountedShopId = table.Column<int>(nullable: false),
                    ShoppingVoucherUserid = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DiscountShop_Cart", x => x.Id);
                    table.ForeignKey(
                        name: "FK_DiscountShop_Cart_DiscountedShop_DiscountedShopId",
                        column: x => x.DiscountedShopId,
                        principalTable: "DiscountedShop",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_DiscountShop_Cart_ShoppingCartItems_ShoppingCartItemId",
                        column: x => x.ShoppingCartItemId,
                        principalTable: "ShoppingCartItems",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_DiscountShop_Cart_AspNetUsers_ShoppingVoucherUserid",
                        column: x => x.ShoppingVoucherUserid,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_DiscountedShop_VendorId",
                table: "DiscountedShop",
                column: "VendorId");

            migrationBuilder.CreateIndex(
                name: "IX_DiscountShop_Cart_DiscountedShopId",
                table: "DiscountShop_Cart",
                column: "DiscountedShopId");

            migrationBuilder.CreateIndex(
                name: "IX_DiscountShop_Cart_ShoppingCartItemId",
                table: "DiscountShop_Cart",
                column: "ShoppingCartItemId");

            migrationBuilder.CreateIndex(
                name: "IX_DiscountShop_Cart_ShoppingVoucherUserid",
                table: "DiscountShop_Cart",
                column: "ShoppingVoucherUserid");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "DiscountShop_Cart");

            migrationBuilder.DropTable(
                name: "DiscountedShop");
        }
    }
}
