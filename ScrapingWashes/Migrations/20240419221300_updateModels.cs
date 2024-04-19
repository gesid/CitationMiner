using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ScrapingWashes.Migrations
{
    /// <inheritdoc />
    public partial class updateModels : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Authors_Papers_PaperId",
                table: "Authors");

            migrationBuilder.RenameColumn(
                name: "OtenDate",
                table: "Papers",
                newName: "ObtenDate");

            migrationBuilder.AddColumn<string>(
                name: "Summary",
                table: "Papers",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<int>(
                name: "Year",
                table: "Papers",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AlterColumn<int>(
                name: "PaperId",
                table: "Authors",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Authors_Papers_PaperId",
                table: "Authors",
                column: "PaperId",
                principalTable: "Papers",
                principalColumn: "PaperId",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Authors_Papers_PaperId",
                table: "Authors");

            migrationBuilder.DropColumn(
                name: "Summary",
                table: "Papers");

            migrationBuilder.DropColumn(
                name: "Year",
                table: "Papers");

            migrationBuilder.RenameColumn(
                name: "ObtenDate",
                table: "Papers",
                newName: "OtenDate");

            migrationBuilder.AlterColumn<int>(
                name: "PaperId",
                table: "Authors",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AddForeignKey(
                name: "FK_Authors_Papers_PaperId",
                table: "Authors",
                column: "PaperId",
                principalTable: "Papers",
                principalColumn: "PaperId");
        }
    }
}
