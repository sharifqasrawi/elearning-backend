using Microsoft.EntityFrameworkCore.Migrations;

namespace E_Learning.Migrations
{
    public partial class addedVisits2 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<long>(
                name: "DayVisitsCount",
                table: "Visits",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "DayVisitsCount",
                table: "Visits");
        }
    }
}
