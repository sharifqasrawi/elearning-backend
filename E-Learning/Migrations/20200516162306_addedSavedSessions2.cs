using Microsoft.EntityFrameworkCore.Migrations;

namespace E_Learning.Migrations
{
    public partial class addedSavedSessions2 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_SavedSession_Sessions_SessionId",
                table: "SavedSession");

            migrationBuilder.DropForeignKey(
                name: "FK_SavedSession_AspNetUsers_UserId",
                table: "SavedSession");

            migrationBuilder.DropPrimaryKey(
                name: "PK_SavedSession",
                table: "SavedSession");

            migrationBuilder.RenameTable(
                name: "SavedSession",
                newName: "SavedSessions");

            migrationBuilder.RenameIndex(
                name: "IX_SavedSession_UserId",
                table: "SavedSessions",
                newName: "IX_SavedSessions_UserId");

            migrationBuilder.RenameIndex(
                name: "IX_SavedSession_SessionId",
                table: "SavedSessions",
                newName: "IX_SavedSessions_SessionId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_SavedSessions",
                table: "SavedSessions",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_SavedSessions_Sessions_SessionId",
                table: "SavedSessions",
                column: "SessionId",
                principalTable: "Sessions",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_SavedSessions_AspNetUsers_UserId",
                table: "SavedSessions",
                column: "UserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_SavedSessions_Sessions_SessionId",
                table: "SavedSessions");

            migrationBuilder.DropForeignKey(
                name: "FK_SavedSessions_AspNetUsers_UserId",
                table: "SavedSessions");

            migrationBuilder.DropPrimaryKey(
                name: "PK_SavedSessions",
                table: "SavedSessions");

            migrationBuilder.RenameTable(
                name: "SavedSessions",
                newName: "SavedSession");

            migrationBuilder.RenameIndex(
                name: "IX_SavedSessions_UserId",
                table: "SavedSession",
                newName: "IX_SavedSession_UserId");

            migrationBuilder.RenameIndex(
                name: "IX_SavedSessions_SessionId",
                table: "SavedSession",
                newName: "IX_SavedSession_SessionId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_SavedSession",
                table: "SavedSession",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_SavedSession_Sessions_SessionId",
                table: "SavedSession",
                column: "SessionId",
                principalTable: "Sessions",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_SavedSession_AspNetUsers_UserId",
                table: "SavedSession",
                column: "UserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
