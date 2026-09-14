using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Tajawul.Migrations
{
    /// <inheritdoc />
    public partial class remove_user_businessManager_tables : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "business_managers");

            migrationBuilder.DropTable(
                name: "users");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "business_managers",
                columns: table => new
                {
                    manager_id = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Id = table.Column<string>(type: "nvarchar(450)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__business__5A6073FC19907B65", x => x.manager_id);
                    table.ForeignKey(
                        name: "FK__business___perso__5BE2A6F2",
                        column: x => x.Id,
                        principalTable: "persons",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "users",
                columns: table => new
                {
                    user_id = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Id = table.Column<string>(type: "nvarchar(450)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__users__B9BE370FA5584F82", x => x.user_id);
                    table.ForeignKey(
                        name: "FK__users__person_id__5165187F",
                        column: x => x.Id,
                        principalTable: "persons",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_business_managers_Id",
                table: "business_managers",
                column: "Id");

            migrationBuilder.CreateIndex(
                name: "IX_users_Id",
                table: "users",
                column: "Id");
        }
    }
}
