using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ScrapingWashes.Migrations
{
    /// <inheritdoc />
    public partial class updatepk : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Papers_Editions_EditionId",
                table: "Papers");

            migrationBuilder.RenameColumn(
                name: "TypePaper",
                table: "Papers",
                newName: "Type");

            migrationBuilder.AlterColumn<int>(
                name: "EditionId",
                table: "Papers",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Papers_Editions_EditionId",
                table: "Papers",
                column: "EditionId",
                principalTable: "Editions",
                principalColumn: "EditionId",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Papers_Editions_EditionId",
                table: "Papers");

            migrationBuilder.RenameColumn(
                name: "Type",
                table: "Papers",
                newName: "TypePaper");

            migrationBuilder.AlterColumn<int>(
                name: "EditionId",
                table: "Papers",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AddForeignKey(
                name: "FK_Papers_Editions_EditionId",
                table: "Papers",
                column: "EditionId",
                principalTable: "Editions",
                principalColumn: "EditionId");
        }
    }
}
