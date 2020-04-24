using Microsoft.EntityFrameworkCore.Migrations;

namespace E_Learning.Migrations
{
    public partial class updatedCourses4 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<long>(
                name: "CourseId",
                table: "Tags",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Tags_CourseId",
                table: "Tags",
                column: "CourseId");

            migrationBuilder.AddForeignKey(
                name: "FK_Tags_Courses_CourseId",
                table: "Tags",
                column: "CourseId",
                principalTable: "Courses",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Tags_Courses_CourseId",
                table: "Tags");

            migrationBuilder.DropIndex(
                name: "IX_Tags_CourseId",
                table: "Tags");

            migrationBuilder.DropColumn(
                name: "CourseId",
                table: "Tags");
        }
    }
}
