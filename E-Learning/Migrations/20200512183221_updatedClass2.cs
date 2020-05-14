using Microsoft.EntityFrameworkCore.Migrations;

namespace E_Learning.Migrations
{
    public partial class updatedClass2 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ClassUser_AspNetUsers_ClassId",
                table: "ClassUser");

            migrationBuilder.DropForeignKey(
                name: "FK_ClassUser_Classes_UserId",
                table: "ClassUser");

            migrationBuilder.AddForeignKey(
                name: "FK_ClassUser_Classes_ClassId",
                table: "ClassUser",
                column: "ClassId",
                principalTable: "Classes",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_ClassUser_AspNetUsers_UserId",
                table: "ClassUser",
                column: "UserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ClassUser_Classes_ClassId",
                table: "ClassUser");

            migrationBuilder.DropForeignKey(
                name: "FK_ClassUser_AspNetUsers_UserId",
                table: "ClassUser");

            migrationBuilder.AddForeignKey(
                name: "FK_ClassUser_AspNetUsers_ClassId",
                table: "ClassUser",
                column: "ClassId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_ClassUser_Classes_UserId",
                table: "ClassUser",
                column: "UserId",
                principalTable: "Classes",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
