using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Persistence.Database.Migrations
{
    /// <inheritdoc />
    public partial class AddTranslationsAiContextCategoryTagAndStaticResponse : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "StructureScope",
                table: "Questions",
                newName: "StaticResponse");

            migrationBuilder.RenameColumn(
                name: "Content",
                table: "Questions",
                newName: "ContentPl");

            migrationBuilder.RenameColumn(
                name: "Content",
                table: "Answers",
                newName: "ContentPl");

            migrationBuilder.AddColumn<string>(
                name: "AiContext",
                table: "Questions",
                type: "longtext",
                nullable: true)
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddColumn<string>(
                name: "CategoryTag",
                table: "Questions",
                type: "longtext",
                nullable: true)
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddColumn<string>(
                name: "CategoryType",
                table: "Questions",
                type: "longtext",
                nullable: true)
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddColumn<string>(
                name: "ContentDe",
                table: "Questions",
                type: "longtext",
                nullable: true)
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddColumn<string>(
                name: "ContentEn",
                table: "Questions",
                type: "longtext",
                nullable: true)
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddColumn<string>(
                name: "ContentUk",
                table: "Questions",
                type: "longtext",
                nullable: true)
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddColumn<string>(
                name: "ContentDe",
                table: "Answers",
                type: "longtext",
                nullable: true)
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddColumn<string>(
                name: "ContentEn",
                table: "Answers",
                type: "longtext",
                nullable: true)
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddColumn<string>(
                name: "ContentUk",
                table: "Answers",
                type: "longtext",
                nullable: true)
                .Annotation("MySql:CharSet", "utf8mb4");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "AiContext",
                table: "Questions");

            migrationBuilder.DropColumn(
                name: "CategoryTag",
                table: "Questions");

            migrationBuilder.DropColumn(
                name: "CategoryType",
                table: "Questions");

            migrationBuilder.DropColumn(
                name: "ContentDe",
                table: "Questions");

            migrationBuilder.DropColumn(
                name: "ContentEn",
                table: "Questions");

            migrationBuilder.DropColumn(
                name: "ContentUk",
                table: "Questions");

            migrationBuilder.DropColumn(
                name: "ContentDe",
                table: "Answers");

            migrationBuilder.DropColumn(
                name: "ContentEn",
                table: "Answers");

            migrationBuilder.DropColumn(
                name: "ContentUk",
                table: "Answers");

            migrationBuilder.RenameColumn(
                name: "StaticResponse",
                table: "Questions",
                newName: "StructureScope");

            migrationBuilder.RenameColumn(
                name: "ContentPl",
                table: "Questions",
                newName: "Content");

            migrationBuilder.RenameColumn(
                name: "ContentPl",
                table: "Answers",
                newName: "Content");
        }
    }
}
