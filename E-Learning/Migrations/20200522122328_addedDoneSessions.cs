using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace E_Learning.Migrations
{
    public partial class addedDoneSessions : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "DoneSessions",
                columns: table => new
                {
                    Id = table.Column<long>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    SessionId = table.Column<long>(nullable: true),
                    UserID = table.Column<string>(nullable: true),
                    DoneDateTime = table.Column<DateTime>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DoneSessions", x => x.Id);
                    table.ForeignKey(
                        name: "FK_DoneSessions_Sessions_SessionId",
                        column: x => x.SessionId,
                        principalTable: "Sessions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_DoneSessions_AspNetUsers_UserID",
                        column: x => x.UserID,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_DoneSessions_SessionId",
                table: "DoneSessions",
                column: "SessionId");

            migrationBuilder.CreateIndex(
                name: "IX_DoneSessions_UserID",
                table: "DoneSessions",
                column: "UserID");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "DoneSessions");
        }
    }
}
