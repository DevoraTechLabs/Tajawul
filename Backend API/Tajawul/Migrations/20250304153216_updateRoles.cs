using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Tajawul.Migrations
{
    /// <inheritdoc />
    public partial class updateRoles : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "8c1683a7-f52e-4f82-9b88-513ab45fc1c8",
                columns: new[] { "Name", "NormalizedName" },
                values: new object[] { "CompletedSocialInfo", "COMPLETEDSOCIALINFO" });

            migrationBuilder.InsertData(
                table: "AspNetRoles",
                columns: new[] { "Id", "ConcurrencyStamp", "Name", "NormalizedName" },
                values: new object[] { "1e84b668-1a0f-4040-a602-3949f371793e", null, "CompletedInterestInfo", "COMPLETEDINTERESTINFO" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "1e84b668-1a0f-4040-a602-3949f371793e");

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "8c1683a7-f52e-4f82-9b88-513ab45fc1c8",
                columns: new[] { "Name", "NormalizedName" },
                values: new object[] { "CompletedStepOne", "COMPLETEDSTEPONE" });
        }
    }
}
