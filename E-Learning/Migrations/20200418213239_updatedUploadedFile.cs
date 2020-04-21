using Microsoft.EntityFrameworkCore.Migrations;

namespace E_Learning.Migrations
{
    public partial class updatedUploadedFile : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "ModifiedFileName",
                table: "UploadedFiles",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "OriginalFileName",
                table: "UploadedFiles",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "UploadPath",
                table: "UploadedFiles",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ModifiedFileName",
                table: "UploadedFiles");

            migrationBuilder.DropColumn(
                name: "OriginalFileName",
                table: "UploadedFiles");

            migrationBuilder.DropColumn(
                name: "UploadPath",
                table: "UploadedFiles");
        }
    }
}
