using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Acme.ProductSelling.Migrations
{
    /// <inheritdoc />
    public partial class tempremoveslug : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Manufacturers_UrlSlug",
                table: "Manufacturers");

            migrationBuilder.DropIndex(
                name: "IX_Categories_UrlSlug",
                table: "Categories");

            migrationBuilder.DropColumn(
                name: "UrlSlug",
                table: "Manufacturers");

            migrationBuilder.DropColumn(
                name: "UrlSlug",
                table: "Categories");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "UrlSlug",
                table: "Manufacturers",
                type: "nvarchar(450)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "UrlSlug",
                table: "Categories",
                type: "nvarchar(450)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.CreateIndex(
                name: "IX_Manufacturers_UrlSlug",
                table: "Manufacturers",
                column: "UrlSlug");

            migrationBuilder.CreateIndex(
                name: "IX_Categories_UrlSlug",
                table: "Categories",
                column: "UrlSlug");
        }
    }
}
