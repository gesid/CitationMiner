using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ScrapingWashes.Migrations
{
    /// <inheritdoc />
    public partial class addAuthorPaper : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Authors_Papers_PaperId",
                table: "Authors");

            migrationBuilder.DropIndex(
                name: "IX_Authors_PaperId",
                table: "Authors");

            migrationBuilder.CreateTable(
                name: "AuthorPaper",
                columns: table => new
                {
                    AuthorsAuthorId = table.Column<int>(type: "int", nullable: false),
                    PapersPaperId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AuthorPaper", x => new { x.AuthorsAuthorId, x.PapersPaperId });
                    table.ForeignKey(
                        name: "FK_AuthorPaper_Authors_AuthorsAuthorId",
                        column: x => x.AuthorsAuthorId,
                        principalTable: "Authors",
                        principalColumn: "AuthorId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_AuthorPaper_Papers_PapersPaperId",
                        column: x => x.PapersPaperId,
                        principalTable: "Papers",
                        principalColumn: "PaperId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_AuthorPaper_PapersPaperId",
                table: "AuthorPaper",
                column: "PapersPaperId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AuthorPaper");

            migrationBuilder.CreateIndex(
                name: "IX_Authors_PaperId",
                table: "Authors",
                column: "PaperId");

            migrationBuilder.AddForeignKey(
                name: "FK_Authors_Papers_PaperId",
                table: "Authors",
                column: "PaperId",
                principalTable: "Papers",
                principalColumn: "PaperId",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
