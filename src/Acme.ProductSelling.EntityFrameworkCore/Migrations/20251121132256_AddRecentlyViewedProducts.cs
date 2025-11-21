using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Acme.ProductSelling.Migrations
{
    /// <inheritdoc />
    public partial class AddRecentlyViewedProducts : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "AppRecentlyViewedProducts",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    UserId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ProductId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ViewedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AppRecentlyViewedProducts", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AppRecentlyViewedProducts_AppProducts_ProductId",
                        column: x => x.ProductId,
                        principalTable: "AppProducts",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_AppRecentlyViewedProducts_ProductId",
                table: "AppRecentlyViewedProducts",
                column: "ProductId");

            migrationBuilder.CreateIndex(
                name: "IX_RecentlyViewed_UserId_ProductId",
                table: "AppRecentlyViewedProducts",
                columns: new[] { "UserId", "ProductId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_RecentlyViewed_UserId_ViewedAt",
                table: "AppRecentlyViewedProducts",
                columns: new[] { "UserId", "ViewedAt" },
                descending: new[] { false, true });

            migrationBuilder.CreateIndex(
                name: "IX_RecentlyViewed_ViewedAt",
                table: "AppRecentlyViewedProducts",
                column: "ViewedAt");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AppRecentlyViewedProducts");
        }
    }
}
