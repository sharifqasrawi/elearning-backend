using Microsoft.EntityFrameworkCore.Migrations;

namespace E_Learning.Migrations
{
    public partial class addedSlug : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Slug_EN",
                table: "Sessions",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Slug_EN",
                table: "Sections",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Slug_EN",
                table: "Courses",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Slug_EN",
                table: "Sessions");

            migrationBuilder.DropColumn(
                name: "Slug_EN",
                table: "Sections");

            migrationBuilder.DropColumn(
                name: "Slug_EN",
                table: "Courses");
        }
    }
}
