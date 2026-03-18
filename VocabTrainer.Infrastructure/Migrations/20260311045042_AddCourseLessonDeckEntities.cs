using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace VocabTrainer.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddCourseLessonDeckEntities : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "LessonId",
                table: "VocabEntries",
                type: "uuid",
                nullable: true
            );

            migrationBuilder.CreateTable(
                name: "Courses",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Title = table.Column<string>(type: "text", nullable: false),
                    Description = table.Column<string>(type: "text", nullable: true),
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Courses", x => x.Id);
                }
            );

            migrationBuilder.CreateTable(
                name: "Decks",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Title = table.Column<string>(type: "text", nullable: false),
                    Description = table.Column<string>(type: "text", nullable: true),
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Decks", x => x.Id);
                }
            );

            migrationBuilder.CreateTable(
                name: "Lessons",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Title = table.Column<string>(type: "text", nullable: false),
                    Order = table.Column<int>(type: "integer", nullable: false),
                    CourseId = table.Column<Guid>(type: "uuid", nullable: false),
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Lessons", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Lessons_Courses_CourseId",
                        column: x => x.CourseId,
                        principalTable: "Courses",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade
                    );
                }
            );

            migrationBuilder.CreateTable(
                name: "DeckEntries",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    DeckId = table.Column<Guid>(type: "uuid", nullable: false),
                    VocabEntryId = table.Column<Guid>(type: "uuid", nullable: false),
                    AddedAt = table.Column<DateTime>(
                        type: "timestamp with time zone",
                        nullable: false
                    ),
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DeckEntries", x => x.Id);
                    table.ForeignKey(
                        name: "FK_DeckEntries_Decks_DeckId",
                        column: x => x.DeckId,
                        principalTable: "Decks",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade
                    );
                    table.ForeignKey(
                        name: "FK_DeckEntries_VocabEntries_VocabEntryId",
                        column: x => x.VocabEntryId,
                        principalTable: "VocabEntries",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade
                    );
                }
            );

            migrationBuilder.CreateTable(
                name: "DeckLessons",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    DeckId = table.Column<Guid>(type: "uuid", nullable: false),
                    LessonId = table.Column<Guid>(type: "uuid", nullable: false),
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DeckLessons", x => x.Id);
                    table.ForeignKey(
                        name: "FK_DeckLessons_Decks_DeckId",
                        column: x => x.DeckId,
                        principalTable: "Decks",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade
                    );
                    table.ForeignKey(
                        name: "FK_DeckLessons_Lessons_LessonId",
                        column: x => x.LessonId,
                        principalTable: "Lessons",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade
                    );
                }
            );

            migrationBuilder.CreateIndex(
                name: "IX_VocabEntries_LessonId",
                table: "VocabEntries",
                column: "LessonId"
            );

            migrationBuilder.CreateIndex(
                name: "IX_DeckEntries_DeckId_VocabEntryId",
                table: "DeckEntries",
                columns: new[] { "DeckId", "VocabEntryId" },
                unique: true
            );

            migrationBuilder.CreateIndex(
                name: "IX_DeckEntries_VocabEntryId",
                table: "DeckEntries",
                column: "VocabEntryId"
            );

            migrationBuilder.CreateIndex(
                name: "IX_DeckLessons_DeckId_LessonId",
                table: "DeckLessons",
                columns: new[] { "DeckId", "LessonId" },
                unique: true
            );

            migrationBuilder.CreateIndex(
                name: "IX_DeckLessons_LessonId",
                table: "DeckLessons",
                column: "LessonId"
            );

            migrationBuilder.CreateIndex(
                name: "IX_Lessons_CourseId_Order",
                table: "Lessons",
                columns: new[] { "CourseId", "Order" },
                unique: true
            );

            migrationBuilder.AddForeignKey(
                name: "FK_VocabEntries_Lessons_LessonId",
                table: "VocabEntries",
                column: "LessonId",
                principalTable: "Lessons",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull
            );
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_VocabEntries_Lessons_LessonId",
                table: "VocabEntries"
            );

            migrationBuilder.DropTable(name: "DeckEntries");

            migrationBuilder.DropTable(name: "DeckLessons");

            migrationBuilder.DropTable(name: "Decks");

            migrationBuilder.DropTable(name: "Lessons");

            migrationBuilder.DropTable(name: "Courses");

            migrationBuilder.DropIndex(name: "IX_VocabEntries_LessonId", table: "VocabEntries");

            migrationBuilder.DropColumn(name: "LessonId", table: "VocabEntries");
        }
    }
}
