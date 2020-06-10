using Microsoft.EntityFrameworkCore.Migrations;

namespace E_Learning.Migrations
{
    public partial class updatedCategoryFR : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Slug_FR",
                table: "Categories",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Title_FR",
                table: "Categories",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Slug_FR",
                table: "Categories");

            migrationBuilder.DropColumn(
                name: "Title_FR",
                table: "Categories");
        }
    }
}
