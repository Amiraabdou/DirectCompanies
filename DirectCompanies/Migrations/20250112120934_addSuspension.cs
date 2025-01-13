using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DirectCompanies.Migrations
{
    /// <inheritdoc />
    public partial class addSuspension : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Status",
                table: "OutBoxEvents");

            migrationBuilder.RenameColumn(
                name: "SentToErp",
                table: "Employees",
                newName: "IsTemporarySuspension");

            migrationBuilder.AddColumn<bool>(
                name: "IsPermanentSuspension",
                table: "Employees",
                type: "INTEGER",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<DateTime>(
                name: "SuspensionTo",
                table: "Employees",
                type: "TEXT",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsPermanentSuspension",
                table: "Employees");

            migrationBuilder.DropColumn(
                name: "SuspensionTo",
                table: "Employees");

            migrationBuilder.RenameColumn(
                name: "IsTemporarySuspension",
                table: "Employees",
                newName: "SentToErp");

            migrationBuilder.AddColumn<string>(
                name: "Status",
                table: "OutBoxEvents",
                type: "TEXT",
                nullable: true);
        }
    }
}
