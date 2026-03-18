using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace VocabTrainer.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddIsClassifiedToVocabEntry : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsClassified",
                table: "VocabEntries",
                type: "boolean",
                nullable: false,
                defaultValue: false
            );
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(name: "IsClassified", table: "VocabEntries");
        }
    }
}
