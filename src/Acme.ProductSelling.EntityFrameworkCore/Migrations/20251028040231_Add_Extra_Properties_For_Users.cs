using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Acme.ProductSelling.Migrations
{
    /// <inheritdoc />
    public partial class Add_Extra_Properties_For_Users : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "ContactPhoneNumber",
                table: "AbpUsers",
                type: "nvarchar(20)",
                maxLength: 20,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ShippingAddress",
                table: "AbpUsers",
                type: "nvarchar(256)",
                maxLength: 256,
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ContactPhoneNumber",
                table: "AbpUsers");

            migrationBuilder.DropColumn(
                name: "ShippingAddress",
                table: "AbpUsers");
        }
    }
}
