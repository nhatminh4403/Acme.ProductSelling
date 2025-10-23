using Microsoft.EntityFrameworkCore.Migrations;
using System;

#nullable disable

namespace Acme.ProductSelling.Migrations
{
    /// <inheritdoc />
    public partial class Update_Storage : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AppStorageSpecifications_AppFormFactors_FormFactorId",
                table: "AppStorageSpecifications");

            migrationBuilder.DropIndex(
                name: "IX_AppStorageSpecifications_FormFactorId",
                table: "AppStorageSpecifications");

            migrationBuilder.DropColumn(
                name: "FormFactorId",
                table: "AppStorageSpecifications");

            migrationBuilder.AddColumn<int>(
                name: "StorageFormFactor",
                table: "AppStorageSpecifications",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "RamFormFactor",
                table: "AppRamSpecifications",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "StorageFormFactor",
                table: "AppStorageSpecifications");

            migrationBuilder.DropColumn(
                name: "RamFormFactor",
                table: "AppRamSpecifications");

            migrationBuilder.AddColumn<Guid>(
                name: "FormFactorId",
                table: "AppStorageSpecifications",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.CreateIndex(
                name: "IX_AppStorageSpecifications_FormFactorId",
                table: "AppStorageSpecifications",
                column: "FormFactorId");

            migrationBuilder.AddForeignKey(
                name: "FK_AppStorageSpecifications_AppFormFactors_FormFactorId",
                table: "AppStorageSpecifications",
                column: "FormFactorId",
                principalTable: "AppFormFactors",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
