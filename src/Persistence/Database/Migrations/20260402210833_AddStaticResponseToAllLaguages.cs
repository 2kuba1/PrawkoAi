using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Persistence.Database.Migrations
{
    /// <inheritdoc />
    public partial class AddStaticResponseToAllLaguages : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "StaticResponse",
                table: "Questions");

            migrationBuilder.AddColumn<string>(
                name: "StaticResponseDe",
                table: "Questions",
                type: "longtext",
                nullable: true)
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddColumn<string>(
                name: "StaticResponseEn",
                table: "Questions",
                type: "longtext",
                nullable: true)
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddColumn<string>(
                name: "StaticResponsePl",
                table: "Questions",
                type: "longtext",
                nullable: true)
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddColumn<string>(
                name: "StaticResponseUa",
                table: "Questions",
                type: "longtext",
                nullable: true)
                .Annotation("MySql:CharSet", "utf8mb4");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "StaticResponseDe",
                table: "Questions");

            migrationBuilder.DropColumn(
                name: "StaticResponseEn",
                table: "Questions");

            migrationBuilder.DropColumn(
                name: "StaticResponsePl",
                table: "Questions");

            migrationBuilder.DropColumn(
                name: "StaticResponseUa",
                table: "Questions");

            migrationBuilder.AddColumn<string>(
                name: "StaticResponse",
                table: "Questions",
                type: "longtext",
                nullable: true)
                .Annotation("MySql:CharSet", "utf8mb4");
        }
    }
}
