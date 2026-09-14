using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Tajawul.Migrations
{
    /// <inheritdoc />
    public partial class AddRefreshTokenColumn : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "RefreshToken",
                table: "Persons", // Ensure the correct table name
                type: "nvarchar(max)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "RefreshToken",
                table: "Persons"); // Ensure the correct table name
        }
    }
}
