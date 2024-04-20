using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ScrapingWashes.Migrations
{
    /// <inheritdoc />
    public partial class updatestate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "StateOfBrazil",
                table: "Authors",
                newName: "State");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "State",
                table: "Authors",
                newName: "StateOfBrazil");
        }
    }
}
