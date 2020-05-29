using Microsoft.EntityFrameworkCore.Migrations;

namespace E_Learning.Migrations
{
    public partial class updatedUserQuiz3 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_UserQuizzes_Questions_CurrentQuestionId",
                table: "UserQuizzes");

            migrationBuilder.DropIndex(
                name: "IX_UserQuizzes_CurrentQuestionId",
                table: "UserQuizzes");

            migrationBuilder.DropColumn(
                name: "CurrentQuestionId",
                table: "UserQuizzes");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<long>(
                name: "CurrentQuestionId",
                table: "UserQuizzes",
                type: "bigint",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_UserQuizzes_CurrentQuestionId",
                table: "UserQuizzes",
                column: "CurrentQuestionId");

            migrationBuilder.AddForeignKey(
                name: "FK_UserQuizzes_Questions_CurrentQuestionId",
                table: "UserQuizzes",
                column: "CurrentQuestionId",
                principalTable: "Questions",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
