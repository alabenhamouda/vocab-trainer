using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace VocabTrainer.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class InitDatabase : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "VocabEntries",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Term = table.Column<string>(type: "text", nullable: false),
                    Definition = table.Column<string>(type: "text", nullable: true),
                    EnglishTranslation = table.Column<string>(type: "text", nullable: true),
                    ImageUrl = table.Column<string>(type: "text", nullable: true),
                    EntryType = table.Column<string>(
                        type: "character varying(13)",
                        maxLength: 13,
                        nullable: false
                    ),
                    Gender = table.Column<int>(type: "integer", nullable: true),
                    PluralForm = table.Column<string>(type: "text", nullable: true),
                    IsSingularOnly = table.Column<bool>(type: "boolean", nullable: true),
                    IsPluralOnly = table.Column<bool>(type: "boolean", nullable: true),
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_VocabEntries", x => x.Id);
                }
            );
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(name: "VocabEntries");
        }
    }
}
