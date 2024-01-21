using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Persistence.Migrations
{
    public partial class rtyrty : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "MessageNotifications",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    MessageTableId = table.Column<int>(nullable: false),
                    Creation = table.Column<DateTime>(nullable: false),
                    Header = table.Column<string>(nullable: true),
                    MessageBody = table.Column<string>(nullable: true),
                    ReceivingUserId = table.Column<string>(nullable: true),
                    UserWhomMakeId = table.Column<string>(nullable: true),
                    IsRead = table.Column<bool>(nullable: false),
                    ReadTime = table.Column<DateTime>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MessageNotifications", x => x.Id);
                    table.ForeignKey(
                        name: "FK_MessageNotifications_MessageTables_MessageTableId",
                        column: x => x.MessageTableId,
                        principalTable: "MessageTables",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_MessageNotifications_AspNetUsers_ReceivingUserId",
                        column: x => x.ReceivingUserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_MessageNotifications_AspNetUsers_UserWhomMakeId",
                        column: x => x.UserWhomMakeId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_MessageNotifications_MessageTableId",
                table: "MessageNotifications",
                column: "MessageTableId");

            migrationBuilder.CreateIndex(
                name: "IX_MessageNotifications_ReceivingUserId",
                table: "MessageNotifications",
                column: "ReceivingUserId");

            migrationBuilder.CreateIndex(
                name: "IX_MessageNotifications_UserWhomMakeId",
                table: "MessageNotifications",
                column: "UserWhomMakeId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "MessageNotifications");
        }
    }
}
