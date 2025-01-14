using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DirectCompanies.Migrations
{
    /// <inheritdoc />
    public partial class deleteIsPermanentSuspension : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsPermanentSuspension",
                table: "Employees");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsPermanentSuspension",
                table: "Employees",
                type: "INTEGER",
                nullable: false,
                defaultValue: false);
        }
    }
}
