using Microsoft.EntityFrameworkCore.Migrations;

namespace E_Learning.Migrations
{
    public partial class updatedSessionContent : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Note",
                table: "SessionContents",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Note",
                table: "SessionContents");
        }
    }
}
