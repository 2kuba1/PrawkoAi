using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Persistence.Database.Migrations
{
    /// <inheritdoc />
    public partial class RemoveQuestionFromUserAiProgress : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_UserAiProgresses_Questions_QuestionId",
                table: "UserAiProgresses");

            migrationBuilder.DropIndex(
                name: "IX_UserAiProgresses_QuestionId",
                table: "UserAiProgresses");

            migrationBuilder.DropColumn(
                name: "QuestionId",
                table: "UserAiProgresses");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "QuestionId",
                table: "UserAiProgresses",
                type: "char(36)",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"),
                collation: "ascii_general_ci");

            migrationBuilder.CreateIndex(
                name: "IX_UserAiProgresses_QuestionId",
                table: "UserAiProgresses",
                column: "QuestionId");

            migrationBuilder.AddForeignKey(
                name: "FK_UserAiProgresses_Questions_QuestionId",
                table: "UserAiProgresses",
                column: "QuestionId",
                principalTable: "Questions",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
