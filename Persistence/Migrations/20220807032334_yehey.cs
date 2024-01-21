using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Persistence.Migrations
{
    public partial class yehey : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "MovieNotifications",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    MovieId = table.Column<int>(nullable: false),
                    Creation = table.Column<DateTime>(nullable: false),
                    ProductImage = table.Column<string>(nullable: true),
                    Header = table.Column<string>(nullable: true),
                    MessageBody = table.Column<string>(nullable: true),
                    ReceivingUserId = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MovieNotifications", x => x.Id);
                    table.ForeignKey(
                        name: "FK_MovieNotifications_Movies_MovieId",
                        column: x => x.MovieId,
                        principalTable: "Movies",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_MovieNotifications_AspNetUsers_ReceivingUserId",
                        column: x => x.ReceivingUserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_MovieNotifications_MovieId",
                table: "MovieNotifications",
                column: "MovieId");

            migrationBuilder.CreateIndex(
                name: "IX_MovieNotifications_ReceivingUserId",
                table: "MovieNotifications",
                column: "ReceivingUserId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "MovieNotifications");
        }
    }
}
