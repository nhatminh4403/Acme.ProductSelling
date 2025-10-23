using Microsoft.EntityFrameworkCore.Migrations;
using System;

#nullable disable

namespace Acme.ProductSelling.Migrations
{
    /// <inheritdoc />
    public partial class AddOrderHistory : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Status",
                table: "AppOrders",
                newName: "OrderStatus");

            migrationBuilder.CreateTable(
                name: "AppOrderHistories",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    OrderId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    OldStatus = table.Column<int>(type: "int", nullable: false),
                    NewStatus = table.Column<int>(type: "int", nullable: false),
                    OldPaymentStatus = table.Column<int>(type: "int", nullable: false),
                    NewPaymentStatus = table.Column<int>(type: "int", nullable: false),
                    ChangeDescription = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    ChangedBy = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: false),
                    CreationTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatorId = table.Column<Guid>(type: "uniqueidentifier", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AppOrderHistories", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AppOrderHistories_AppOrders_OrderId",
                        column: x => x.OrderId,
                        principalTable: "AppOrders",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_AppOrderHistories_CreationTime",
                table: "AppOrderHistories",
                column: "CreationTime");

            migrationBuilder.CreateIndex(
                name: "IX_AppOrderHistories_OrderId",
                table: "AppOrderHistories",
                column: "OrderId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AppOrderHistories");

            migrationBuilder.RenameColumn(
                name: "OrderStatus",
                table: "AppOrders",
                newName: "Status");
        }
    }
}
