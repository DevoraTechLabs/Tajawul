using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace Tajawul.Migrations
{
    /// <inheritdoc />
    public partial class seedRole : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "AspNetRoles",
                columns: new[] { "Id", "ConcurrencyStamp", "Name", "NormalizedName" },
                values: new object[,]
                {
                    { "251467ac-f694-48c3-922a-46b27ffbbe03", null, "PlaceOwner", "PLACEOWNER" },
                    { "704a2a66-bc99-4bbe-9ebc-5b74f5df1362", null, "User", "USER" },
                    { "897cb339-f935-4616-ac91-204fbdb49a03", null, "BusinessManager", "BUSINESSMANAGER" },
                    { "9c1358a4-5151-4888-ba77-5a71174fabd4", null, "Person", "PERSON" }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "251467ac-f694-48c3-922a-46b27ffbbe03");

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "704a2a66-bc99-4bbe-9ebc-5b74f5df1362");

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "897cb339-f935-4616-ac91-204fbdb49a03");

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "9c1358a4-5151-4888-ba77-5a71174fabd4");
        }
    }
}
