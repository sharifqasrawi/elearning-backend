using Microsoft.EntityFrameworkCore.Migrations;

namespace E_Learning.Migrations
{
    public partial class updatedFR : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Name_FR",
                table: "Tags",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Slug_FR",
                table: "Sessions",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Title_FR",
                table: "Sessions",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Content_FR",
                table: "SessionContents",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Name_FR",
                table: "Sections",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Slug_FR",
                table: "Sections",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Description_FR",
                table: "Quizzes",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Slug_FR",
                table: "Quizzes",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "title_FR",
                table: "Quizzes",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Slug_FR",
                table: "Questions",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Text_FR",
                table: "Questions",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Description_FR",
                table: "Courses",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Prerequisites_FR",
                table: "Courses",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Slug_FR",
                table: "Courses",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Title_FR",
                table: "Courses",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Name_FR",
                table: "Classes",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Name_FR",
                table: "Tags");

            migrationBuilder.DropColumn(
                name: "Slug_FR",
                table: "Sessions");

            migrationBuilder.DropColumn(
                name: "Title_FR",
                table: "Sessions");

            migrationBuilder.DropColumn(
                name: "Content_FR",
                table: "SessionContents");

            migrationBuilder.DropColumn(
                name: "Name_FR",
                table: "Sections");

            migrationBuilder.DropColumn(
                name: "Slug_FR",
                table: "Sections");

            migrationBuilder.DropColumn(
                name: "Description_FR",
                table: "Quizzes");

            migrationBuilder.DropColumn(
                name: "Slug_FR",
                table: "Quizzes");

            migrationBuilder.DropColumn(
                name: "title_FR",
                table: "Quizzes");

            migrationBuilder.DropColumn(
                name: "Slug_FR",
                table: "Questions");

            migrationBuilder.DropColumn(
                name: "Text_FR",
                table: "Questions");

            migrationBuilder.DropColumn(
                name: "Description_FR",
                table: "Courses");

            migrationBuilder.DropColumn(
                name: "Prerequisites_FR",
                table: "Courses");

            migrationBuilder.DropColumn(
                name: "Slug_FR",
                table: "Courses");

            migrationBuilder.DropColumn(
                name: "Title_FR",
                table: "Courses");

            migrationBuilder.DropColumn(
                name: "Name_FR",
                table: "Classes");
        }
    }
}
