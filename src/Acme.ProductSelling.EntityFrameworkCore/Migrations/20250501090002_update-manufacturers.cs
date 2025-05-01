using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Acme.ProductSelling.Migrations
{
    /// <inheritdoc />
    public partial class updatemanufacturers : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Phone",
                table: "Manufacturers",
                newName: "Description");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Description",
                table: "Manufacturers",
                newName: "Phone");
        }
    }
}
