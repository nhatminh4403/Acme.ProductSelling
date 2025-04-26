using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Acme.ProductSelling.Migrations
{
    /// <inheritdoc />
    public partial class updatespec : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Products_LaptopSpecifications_LaptopSpecificationId",
                table: "Products");

            migrationBuilder.DropForeignKey(
                name: "FK_Products_MonitorSpecifications_MonitorSpecificationId",
                table: "Products");

            migrationBuilder.DropForeignKey(
                name: "FK_Products_MouseSpecifications_MouseSpecificationId",
                table: "Products");

            migrationBuilder.DropPrimaryKey(
                name: "PK_MouseSpecifications",
                table: "MouseSpecifications");

            migrationBuilder.DropPrimaryKey(
                name: "PK_MonitorSpecifications",
                table: "MonitorSpecifications");

            migrationBuilder.DropPrimaryKey(
                name: "PK_LaptopSpecifications",
                table: "LaptopSpecifications");

            migrationBuilder.RenameTable(
                name: "MouseSpecifications",
                newName: "AppMouseSpecifications");

            migrationBuilder.RenameTable(
                name: "MonitorSpecifications",
                newName: "AppMonitorSpecifications");

            migrationBuilder.RenameTable(
                name: "LaptopSpecifications",
                newName: "AppLaptopSpecifications");

            migrationBuilder.AddPrimaryKey(
                name: "PK_AppMouseSpecifications",
                table: "AppMouseSpecifications",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_AppMonitorSpecifications",
                table: "AppMonitorSpecifications",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_AppLaptopSpecifications",
                table: "AppLaptopSpecifications",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Products_AppLaptopSpecifications_LaptopSpecificationId",
                table: "Products",
                column: "LaptopSpecificationId",
                principalTable: "AppLaptopSpecifications",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Products_AppMonitorSpecifications_MonitorSpecificationId",
                table: "Products",
                column: "MonitorSpecificationId",
                principalTable: "AppMonitorSpecifications",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Products_AppMouseSpecifications_MouseSpecificationId",
                table: "Products",
                column: "MouseSpecificationId",
                principalTable: "AppMouseSpecifications",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Products_AppLaptopSpecifications_LaptopSpecificationId",
                table: "Products");

            migrationBuilder.DropForeignKey(
                name: "FK_Products_AppMonitorSpecifications_MonitorSpecificationId",
                table: "Products");

            migrationBuilder.DropForeignKey(
                name: "FK_Products_AppMouseSpecifications_MouseSpecificationId",
                table: "Products");

            migrationBuilder.DropPrimaryKey(
                name: "PK_AppMouseSpecifications",
                table: "AppMouseSpecifications");

            migrationBuilder.DropPrimaryKey(
                name: "PK_AppMonitorSpecifications",
                table: "AppMonitorSpecifications");

            migrationBuilder.DropPrimaryKey(
                name: "PK_AppLaptopSpecifications",
                table: "AppLaptopSpecifications");

            migrationBuilder.RenameTable(
                name: "AppMouseSpecifications",
                newName: "MouseSpecifications");

            migrationBuilder.RenameTable(
                name: "AppMonitorSpecifications",
                newName: "MonitorSpecifications");

            migrationBuilder.RenameTable(
                name: "AppLaptopSpecifications",
                newName: "LaptopSpecifications");

            migrationBuilder.AddPrimaryKey(
                name: "PK_MouseSpecifications",
                table: "MouseSpecifications",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_MonitorSpecifications",
                table: "MonitorSpecifications",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_LaptopSpecifications",
                table: "LaptopSpecifications",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Products_LaptopSpecifications_LaptopSpecificationId",
                table: "Products",
                column: "LaptopSpecificationId",
                principalTable: "LaptopSpecifications",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Products_MonitorSpecifications_MonitorSpecificationId",
                table: "Products",
                column: "MonitorSpecificationId",
                principalTable: "MonitorSpecifications",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Products_MouseSpecifications_MouseSpecificationId",
                table: "Products",
                column: "MouseSpecificationId",
                principalTable: "MouseSpecifications",
                principalColumn: "Id");
        }
    }
}
