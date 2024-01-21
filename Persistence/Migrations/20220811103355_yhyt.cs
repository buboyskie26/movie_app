using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Persistence.Migrations
{
    public partial class yhyt : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "MovieOutOfStocks",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    MovieId = table.Column<int>(nullable: false),
                    UserAttemptToCartOutOfStockId = table.Column<string>(nullable: true),
                    DateCreation = table.Column<DateTime>(nullable: false),
                    isClicked = table.Column<bool>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MovieOutOfStocks", x => x.Id);
                    table.ForeignKey(
                        name: "FK_MovieOutOfStocks_Movies_MovieId",
                        column: x => x.MovieId,
                        principalTable: "Movies",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_MovieOutOfStocks_AspNetUsers_UserAttemptToCartOutOfStockId",
                        column: x => x.UserAttemptToCartOutOfStockId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "MovieOutOfStockNotifications",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    MovieOutOfStockId = table.Column<int>(nullable: false),
                    Creation = table.Column<DateTime>(nullable: false),
                    ProductImage = table.Column<string>(nullable: true),
                    Header = table.Column<string>(nullable: true),
                    MessageBody = table.Column<string>(nullable: true),
                    ReceivingUserId = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MovieOutOfStockNotifications", x => x.Id);
                    table.ForeignKey(
                        name: "FK_MovieOutOfStockNotifications_MovieOutOfStocks_MovieOutOfStockId",
                        column: x => x.MovieOutOfStockId,
                        principalTable: "MovieOutOfStocks",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_MovieOutOfStockNotifications_AspNetUsers_ReceivingUserId",
                        column: x => x.ReceivingUserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_MovieOutOfStockNotifications_MovieOutOfStockId",
                table: "MovieOutOfStockNotifications",
                column: "MovieOutOfStockId");

            migrationBuilder.CreateIndex(
                name: "IX_MovieOutOfStockNotifications_ReceivingUserId",
                table: "MovieOutOfStockNotifications",
                column: "ReceivingUserId");

            migrationBuilder.CreateIndex(
                name: "IX_MovieOutOfStocks_MovieId",
                table: "MovieOutOfStocks",
                column: "MovieId");

            migrationBuilder.CreateIndex(
                name: "IX_MovieOutOfStocks_UserAttemptToCartOutOfStockId",
                table: "MovieOutOfStocks",
                column: "UserAttemptToCartOutOfStockId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "MovieOutOfStockNotifications");

            migrationBuilder.DropTable(
                name: "MovieOutOfStocks");
        }
    }
}
