using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddGinIndexOnQuestions : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("CREATE EXTENSION IF NOT EXISTS pg_trgm;");
            
            migrationBuilder.CreateIndex(
                name: "IX_Questions_CategoryTag",
                table: "Questions",
                column: "CategoryTag");

            migrationBuilder.CreateIndex(
                name: "IX_Questions_ContentDe_Trgm",
                table: "Questions",
                column: "ContentDe")
                .Annotation("Npgsql:IndexMethod", "gin")
                .Annotation("Npgsql:IndexOperators", new[] { "gin_trgm_ops" });

            migrationBuilder.CreateIndex(
                name: "IX_Questions_ContentEn_Trgm",
                table: "Questions",
                column: "ContentEn")
                .Annotation("Npgsql:IndexMethod", "gin")
                .Annotation("Npgsql:IndexOperators", new[] { "gin_trgm_ops" });

            migrationBuilder.CreateIndex(
                name: "IX_Questions_ContentPl_Trgm",
                table: "Questions",
                column: "ContentPl")
                .Annotation("Npgsql:IndexMethod", "gin")
                .Annotation("Npgsql:IndexOperators", new[] { "gin_trgm_ops" });

            migrationBuilder.CreateIndex(
                name: "IX_Questions_ContentUa_Trgm",
                table: "Questions",
                column: "ContentUa")
                .Annotation("Npgsql:IndexMethod", "gin")
                .Annotation("Npgsql:IndexOperators", new[] { "gin_trgm_ops" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Questions_CategoryTag",
                table: "Questions");

            migrationBuilder.DropIndex(
                name: "IX_Questions_ContentDe_Trgm",
                table: "Questions");

            migrationBuilder.DropIndex(
                name: "IX_Questions_ContentEn_Trgm",
                table: "Questions");

            migrationBuilder.DropIndex(
                name: "IX_Questions_ContentPl_Trgm",
                table: "Questions");

            migrationBuilder.DropIndex(
                name: "IX_Questions_ContentUa_Trgm",
                table: "Questions");
        }
    }
}
