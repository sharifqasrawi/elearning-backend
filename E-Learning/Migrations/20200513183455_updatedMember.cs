using Microsoft.EntityFrameworkCore.Migrations;

namespace E_Learning.Migrations
{
    public partial class updatedMember : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<long>(
                name: "SessionId",
                table: "SessionContents",
                nullable: false,
                oldClrType: typeof(long),
                oldType: "bigint",
                oldNullable: true);

            migrationBuilder.AddColumn<string>(
                name: "CurrentSessionSlug",
                table: "ClassUser",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CurrentSessionSlug",
                table: "ClassUser");

            migrationBuilder.AlterColumn<long>(
                name: "SessionId",
                table: "SessionContents",
                type: "bigint",
                nullable: true,
                oldClrType: typeof(long));
        }
    }
}
