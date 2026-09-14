using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Tajawul.Migrations
{
    /// <inheritdoc />
    public partial class removeFields : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "bio",
                table: "users");

            migrationBuilder.DropColumn(
                name: "completed_survey",
                table: "users");

            migrationBuilder.DropColumn(
                name: "gender",
                table: "users");

            migrationBuilder.DropColumn(
                name: "is_top_traveler",
                table: "users");

            migrationBuilder.DropColumn(
                name: "social_media_links",
                table: "users");

            migrationBuilder.DropColumn(
                name: "birth_date",
                table: "persons");

            migrationBuilder.DropColumn(
                name: "city",
                table: "persons");

            migrationBuilder.DropColumn(
                name: "country",
                table: "persons");

            migrationBuilder.DropColumn(
                name: "location",
                table: "persons");

            migrationBuilder.DropColumn(
                name: "postal_code",
                table: "persons");

            migrationBuilder.DropColumn(
                name: "profile_picture_URL",
                table: "persons");

            migrationBuilder.DropColumn(
                name: "street",
                table: "persons");

            migrationBuilder.RenameColumn(
                name: "phone_number",
                table: "persons",
                newName: "PhoneNumber");

            migrationBuilder.AlterColumn<string>(
                name: "PhoneNumber",
                table: "persons",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(30)",
                oldMaxLength: 30,
                oldNullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "PhoneNumber",
                table: "persons",
                newName: "phone_number");

            migrationBuilder.AddColumn<string>(
                name: "bio",
                table: "users",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "completed_survey",
                table: "users",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "gender",
                table: "users",
                type: "bit",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "is_top_traveler",
                table: "users",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<string>(
                name: "social_media_links",
                table: "users",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "phone_number",
                table: "persons",
                type: "nvarchar(30)",
                maxLength: 30,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AddColumn<DateOnly>(
                name: "birth_date",
                table: "persons",
                type: "date",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "city",
                table: "persons",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "country",
                table: "persons",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "location",
                table: "persons",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "postal_code",
                table: "persons",
                type: "nvarchar(20)",
                maxLength: 20,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "profile_picture_URL",
                table: "persons",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "street",
                table: "persons",
                type: "nvarchar(255)",
                maxLength: 255,
                nullable: true);
        }
    }
}
