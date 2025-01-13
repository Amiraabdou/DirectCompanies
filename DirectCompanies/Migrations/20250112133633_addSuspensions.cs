using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DirectCompanies.Migrations
{
    /// <inheritdoc />
    public partial class addSuspensions : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "SuspensionTo",
                table: "Employees",
                newName: "SuspendToDate");

            migrationBuilder.AddColumn<DateTime>(
                name: "SuspendFromDate",
                table: "Employees",
                type: "TEXT",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "SuspendFromDate",
                table: "Employees");

            migrationBuilder.RenameColumn(
                name: "SuspendToDate",
                table: "Employees",
                newName: "SuspensionTo");
        }
    }
}
