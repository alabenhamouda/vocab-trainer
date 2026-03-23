using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace VocabTrainer.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddReviewEntity : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Reviews",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    DeckId = table.Column<Guid>(type: "uuid", nullable: false),
                    VocabEntryId = table.Column<Guid>(type: "uuid", nullable: false),
                    ConfidenceLevel = table.Column<int>(type: "integer", nullable: false),
                    LastReviewedAt = table.Column<DateTime>(
                        type: "timestamp with time zone",
                        nullable: false
                    ),
                    NextReviewAt = table.Column<DateTime>(
                        type: "timestamp with time zone",
                        nullable: false
                    ),
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Reviews", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Reviews_Decks_DeckId",
                        column: x => x.DeckId,
                        principalTable: "Decks",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade
                    );
                    table.ForeignKey(
                        name: "FK_Reviews_VocabEntries_VocabEntryId",
                        column: x => x.VocabEntryId,
                        principalTable: "VocabEntries",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade
                    );
                }
            );

            migrationBuilder.CreateIndex(
                name: "IX_Reviews_DeckId_VocabEntryId",
                table: "Reviews",
                columns: new[] { "DeckId", "VocabEntryId" },
                unique: true
            );

            migrationBuilder.CreateIndex(
                name: "IX_Reviews_VocabEntryId",
                table: "Reviews",
                column: "VocabEntryId"
            );
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(name: "Reviews");
        }
    }
}
