using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ScrapingWashes.Migrations
{
    /// <inheritdoc />
    public partial class updateEdition : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Quantity",
                table: "Editions");

            migrationBuilder.DropColumn(
                name: "WASHESEdition",
                table: "Editions");

            migrationBuilder.AddColumn<DateTime>(
                name: "Date",
                table: "Editions",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<string>(
                name: "Location",
                table: "Editions",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Proceedings",
                table: "Editions",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Date",
                table: "Editions");

            migrationBuilder.DropColumn(
                name: "Location",
                table: "Editions");

            migrationBuilder.DropColumn(
                name: "Proceedings",
                table: "Editions");

            migrationBuilder.AddColumn<int>(
                name: "Quantity",
                table: "Editions",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "WASHESEdition",
                table: "Editions",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }
    }
}
