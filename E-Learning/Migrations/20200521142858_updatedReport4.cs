using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace E_Learning.Migrations
{
    public partial class updatedReport4 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsReplySeen",
                table: "Reports",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "ReplyDateTime",
                table: "Reports",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ReplyMessage",
                table: "Reports",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsReplySeen",
                table: "Reports");

            migrationBuilder.DropColumn(
                name: "ReplyDateTime",
                table: "Reports");

            migrationBuilder.DropColumn(
                name: "ReplyMessage",
                table: "Reports");
        }
    }
}
