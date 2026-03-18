using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace VocabTrainer.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddExampleToVocabEntry : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Example",
                table: "VocabEntries",
                type: "text",
                nullable: true
            );
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(name: "Example", table: "VocabEntries");
        }
    }
}
