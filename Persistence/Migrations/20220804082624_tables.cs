using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Persistence.Migrations
{
    public partial class tables : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "MessageTables",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserWhoStartMessageId = table.Column<string>(nullable: true),
                    MessageCreated = table.Column<DateTime>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MessageTables", x => x.Id);
                    table.ForeignKey(
                        name: "FK_MessageTables_AspNetUsers_UserWhoStartMessageId",
                        column: x => x.UserWhoStartMessageId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "MessageUsersTables",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    MessageTableId = table.Column<int>(nullable: false),
                    Message = table.Column<string>(nullable: true),
                    IsRead = table.Column<bool>(nullable: false),
                    UserOneId = table.Column<string>(nullable: true),
                    UserTwoId = table.Column<string>(nullable: true),
                    IsReadDate = table.Column<DateTime>(nullable: true),
                    MessageCreated = table.Column<DateTime>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MessageUsersTables", x => x.Id);
                    table.ForeignKey(
                        name: "FK_MessageUsersTables_MessageTables_MessageTableId",
                        column: x => x.MessageTableId,
                        principalTable: "MessageTables",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_MessageUsersTables_AspNetUsers_UserOneId",
                        column: x => x.UserOneId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_MessageUsersTables_AspNetUsers_UserTwoId",
                        column: x => x.UserTwoId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_MessageTables_UserWhoStartMessageId",
                table: "MessageTables",
                column: "UserWhoStartMessageId");

            migrationBuilder.CreateIndex(
                name: "IX_MessageUsersTables_MessageTableId",
                table: "MessageUsersTables",
                column: "MessageTableId");

            migrationBuilder.CreateIndex(
                name: "IX_MessageUsersTables_UserOneId",
                table: "MessageUsersTables",
                column: "UserOneId");

            migrationBuilder.CreateIndex(
                name: "IX_MessageUsersTables_UserTwoId",
                table: "MessageUsersTables",
                column: "UserTwoId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "MessageUsersTables");

            migrationBuilder.DropTable(
                name: "MessageTables");
        }
    }
}
