using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Acme.ProductSelling.Migrations
{
    /// <inheritdoc />
    public partial class Added_ECommerce_Properties_To_AppUser : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ContactPhoneNumber",
                table: "AbpUsers");

            migrationBuilder.AddColumn<DateTime>(
                name: "DateOfBirth",
                table: "AbpUsers",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Discriminator",
                table: "AbpUsers",
                type: "nvarchar(13)",
                maxLength: 13,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<int>(
                name: "Gender",
                table: "AbpUsers",
                type: "int",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "DateOfBirth",
                table: "AbpUsers");

            migrationBuilder.DropColumn(
                name: "Discriminator",
                table: "AbpUsers");

            migrationBuilder.DropColumn(
                name: "Gender",
                table: "AbpUsers");

            migrationBuilder.AddColumn<string>(
                name: "ContactPhoneNumber",
                table: "AbpUsers",
                type: "nvarchar(20)",
                maxLength: 20,
                nullable: true);
        }
    }
}
