using Microsoft.EntityFrameworkCore.Migrations;

namespace E_Learning.Migrations
{
    public partial class updatedLikes : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Likes_Courses_CourseId",
                table: "Likes");

            migrationBuilder.AlterColumn<long>(
                name: "CourseId",
                table: "Likes",
                nullable: false,
                oldClrType: typeof(long),
                oldType: "bigint",
                oldNullable: true);

            migrationBuilder.AddColumn<string>(
                name: "UserFullName",
                table: "Likes",
                nullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Likes_Courses_CourseId",
                table: "Likes",
                column: "CourseId",
                principalTable: "Courses",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Likes_Courses_CourseId",
                table: "Likes");

            migrationBuilder.DropColumn(
                name: "UserFullName",
                table: "Likes");

            migrationBuilder.AlterColumn<long>(
                name: "CourseId",
                table: "Likes",
                type: "bigint",
                nullable: true,
                oldClrType: typeof(long));

            migrationBuilder.AddForeignKey(
                name: "FK_Likes_Courses_CourseId",
                table: "Likes",
                column: "CourseId",
                principalTable: "Courses",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
