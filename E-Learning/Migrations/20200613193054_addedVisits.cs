using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace E_Learning.Migrations
{
    public partial class addedVisits : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
         
            migrationBuilder.CreateTable(
                name: "Visits",
                columns: table => new
                {
                    Id = table.Column<long>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    IPAddress = table.Column<string>(nullable: true),
                    BrowserInfo = table.Column<string>(nullable: true),
                    Continent_Code = table.Column<string>(nullable: true),
                    Continent_Name = table.Column<string>(nullable: true),
                    Country_Code = table.Column<string>(nullable: true),
                    Country_Name = table.Column<string>(nullable: true),
                    Region_Code = table.Column<string>(nullable: true),
                    Region_Name = table.Column<string>(nullable: true),
                    City = table.Column<string>(nullable: true),
                    Zip = table.Column<string>(nullable: true),
                    Latitude = table.Column<string>(nullable: true),
                    Longitude = table.Column<string>(nullable: true),
                    DateTime = table.Column<DateTime>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Visits", x => x.Id);
                });

         
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Visits");

          
        }
    }
}
