using Microsoft.EntityFrameworkCore.Migrations;
using System;

#nullable disable

namespace Acme.ProductSelling.Migrations
{
    /// <inheritdoc />
    public partial class Reconfigured_Product_And_Specifications : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Products_AppCaseSpecifications_CaseSpecificationId",
                table: "Products");

            migrationBuilder.DropForeignKey(
                name: "FK_Products_AppCpuCoolerSpecifications_CpuCoolerSpecificationId",
                table: "Products");

            migrationBuilder.DropForeignKey(
                name: "FK_Products_AppCpuSpecifications_CpuSpecificationId",
                table: "Products");

            migrationBuilder.DropForeignKey(
                name: "FK_Products_AppGpuSpecifications_GpuSpecificationId",
                table: "Products");

            migrationBuilder.DropForeignKey(
                name: "FK_Products_AppHeadsetSpecifications_HeadsetSpecificationId",
                table: "Products");

            migrationBuilder.DropForeignKey(
                name: "FK_Products_AppKeyboardSpecifications_KeyboardSpecificationId",
                table: "Products");

            migrationBuilder.DropForeignKey(
                name: "FK_Products_AppLaptopSpecifications_LaptopSpecificationId",
                table: "Products");

            migrationBuilder.DropForeignKey(
                name: "FK_Products_AppMonitorSpecifications_MonitorSpecificationId",
                table: "Products");

            migrationBuilder.DropForeignKey(
                name: "FK_Products_AppMotherboardSpecifications_MotherboardSpecificationId",
                table: "Products");

            migrationBuilder.DropForeignKey(
                name: "FK_Products_AppMouseSpecifications_MouseSpecificationId",
                table: "Products");

            migrationBuilder.DropForeignKey(
                name: "FK_Products_AppPsuSpecifications_PsuSpecificationId",
                table: "Products");

            migrationBuilder.DropForeignKey(
                name: "FK_Products_AppRamSpecifications_RamSpecificationId",
                table: "Products");

            migrationBuilder.DropForeignKey(
                name: "FK_Products_AppStorageSpecifications_StorageSpecificationId",
                table: "Products");

            migrationBuilder.DropForeignKey(
                name: "FK_Products_Categories_CategoryId",
                table: "Products");

            migrationBuilder.DropForeignKey(
                name: "FK_Products_Manufacturers_ManufacturerId",
                table: "Products");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Products",
                table: "Products");

            migrationBuilder.DropIndex(
                name: "IX_Products_CaseSpecificationId",
                table: "Products");

            migrationBuilder.DropIndex(
                name: "IX_Products_CpuCoolerSpecificationId",
                table: "Products");

            migrationBuilder.DropIndex(
                name: "IX_Products_CpuSpecificationId",
                table: "Products");

            migrationBuilder.DropIndex(
                name: "IX_Products_GpuSpecificationId",
                table: "Products");

            migrationBuilder.DropIndex(
                name: "IX_Products_HeadsetSpecificationId",
                table: "Products");

            migrationBuilder.DropIndex(
                name: "IX_Products_KeyboardSpecificationId",
                table: "Products");

            migrationBuilder.DropIndex(
                name: "IX_Products_LaptopSpecificationId",
                table: "Products");

            migrationBuilder.DropIndex(
                name: "IX_Products_MonitorSpecificationId",
                table: "Products");

            migrationBuilder.DropIndex(
                name: "IX_Products_MotherboardSpecificationId",
                table: "Products");

            migrationBuilder.DropIndex(
                name: "IX_Products_MouseSpecificationId",
                table: "Products");

            migrationBuilder.DropIndex(
                name: "IX_Products_PsuSpecificationId",
                table: "Products");

            migrationBuilder.DropIndex(
                name: "IX_Products_RamSpecificationId",
                table: "Products");

            migrationBuilder.DropIndex(
                name: "IX_Products_StorageSpecificationId",
                table: "Products");

            migrationBuilder.DropIndex(
                name: "IX_Products_UrlSlug",
                table: "Products");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Manufacturers",
                table: "Manufacturers");

            migrationBuilder.DropIndex(
                name: "IX_Manufacturers_UrlSlug",
                table: "Manufacturers");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Categories",
                table: "Categories");

            migrationBuilder.DropIndex(
                name: "IX_Categories_UrlSlug",
                table: "Categories");

            migrationBuilder.DropPrimaryKey(
                name: "PK_AppGpuSpecifications",
                table: "AppGpuSpecifications");

            migrationBuilder.DropColumn(
                name: "FormFactor",
                table: "AppStorageSpecifications");

            migrationBuilder.DropColumn(
                name: "RamType",
                table: "AppRamSpecifications");

            migrationBuilder.DropColumn(
                name: "FormFactor",
                table: "AppPsuSpecifications");

            migrationBuilder.DropColumn(
                name: "Chipset",
                table: "AppMotherboardSpecifications");

            migrationBuilder.DropColumn(
                name: "FormFactor",
                table: "AppMotherboardSpecifications");

            migrationBuilder.DropColumn(
                name: "Socket",
                table: "AppMotherboardSpecifications");

            migrationBuilder.DropColumn(
                name: "SupportedRamType",
                table: "AppMotherboardSpecifications");

            migrationBuilder.DropColumn(
                name: "PanelType",
                table: "AppMonitorSpecifications");

            migrationBuilder.DropColumn(
                name: "SwitchType",
                table: "AppKeyboardSpecifications");

            migrationBuilder.DropColumn(
                name: "Socket",
                table: "AppCpuSpecifications");

            migrationBuilder.DropColumn(
                name: "SupportedSockets",
                table: "AppCpuCoolerSpecifications");

            migrationBuilder.DropColumn(
                name: "Material",
                table: "AppCaseSpecifications");

            migrationBuilder.DropColumn(
                name: "SupportedMbFormFactor",
                table: "AppCaseSpecifications");

            migrationBuilder.DropColumn(
                name: "CaseSpecificationId",
                table: "Products");

            migrationBuilder.DropColumn(
                name: "CpuCoolerSpecificationId",
                table: "Products");

            migrationBuilder.DropColumn(
                name: "CpuSpecificationId",
                table: "Products");

            migrationBuilder.DropColumn(
                name: "GpuSpecificationId",
                table: "Products");

            migrationBuilder.DropColumn(
                name: "HeadsetSpecificationId",
                table: "Products");

            migrationBuilder.DropColumn(
                name: "KeyboardSpecificationId",
                table: "Products");

            migrationBuilder.DropColumn(
                name: "LaptopSpecificationId",
                table: "Products");

            migrationBuilder.DropColumn(
                name: "MonitorSpecificationId",
                table: "Products");

            migrationBuilder.DropColumn(
                name: "MotherboardSpecificationId",
                table: "Products");

            migrationBuilder.DropColumn(
                name: "MouseSpecificationId",
                table: "Products");

            migrationBuilder.DropColumn(
                name: "PsuSpecificationId",
                table: "Products");

            migrationBuilder.DropColumn(
                name: "RamSpecificationId",
                table: "Products");

            migrationBuilder.DropColumn(
                name: "StorageSpecificationId",
                table: "Products");

            migrationBuilder.RenameTable(
                name: "Products",
                newName: "AppProducts");

            migrationBuilder.RenameTable(
                name: "Manufacturers",
                newName: "AppManufacturers");

            migrationBuilder.RenameTable(
                name: "Categories",
                newName: "AppCategories");

            migrationBuilder.RenameTable(
                name: "AppGpuSpecifications",
                newName: "AppAppGpuSpecifications");

            migrationBuilder.RenameIndex(
                name: "IX_Products_ManufacturerId",
                table: "AppProducts",
                newName: "IX_AppProducts_ManufacturerId");

            migrationBuilder.RenameIndex(
                name: "IX_Products_CategoryId",
                table: "AppProducts",
                newName: "IX_AppProducts_CategoryId");

            migrationBuilder.AddColumn<Guid>(
                name: "FormFactorId",
                table: "AppStorageSpecifications",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<Guid>(
                name: "ProductId",
                table: "AppStorageSpecifications",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<Guid>(
                name: "ProductId",
                table: "AppRamSpecifications",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<Guid>(
                name: "RamTypeId",
                table: "AppRamSpecifications",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AlterColumn<int>(
                name: "Modularity",
                table: "AppPsuSpecifications",
                type: "int",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AddColumn<Guid>(
                name: "FormFactorId",
                table: "AppPsuSpecifications",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<Guid>(
                name: "ProductId",
                table: "AppPsuSpecifications",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<string>(
                name: "PaymentMethod",
                table: "AppOrders",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<int>(
                name: "PaymentStatus",
                table: "AppOrders",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AlterColumn<int>(
                name: "Connectivity",
                table: "AppMouseSpecifications",
                type: "int",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AddColumn<Guid>(
                name: "ProductId",
                table: "AppMouseSpecifications",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<Guid>(
                name: "ChipsetId",
                table: "AppMotherboardSpecifications",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<Guid>(
                name: "FormFactorId",
                table: "AppMotherboardSpecifications",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<Guid>(
                name: "ProductId",
                table: "AppMotherboardSpecifications",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<Guid>(
                name: "RamTypeId",
                table: "AppMotherboardSpecifications",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<Guid>(
                name: "SocketId",
                table: "AppMotherboardSpecifications",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<Guid>(
                name: "PanelTypeId",
                table: "AppMonitorSpecifications",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<Guid>(
                name: "ProductId",
                table: "AppMonitorSpecifications",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<Guid>(
                name: "ProductId",
                table: "AppLaptopSpecifications",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AlterColumn<int>(
                name: "Layout",
                table: "AppKeyboardSpecifications",
                type: "int",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AlterColumn<int>(
                name: "Connectivity",
                table: "AppKeyboardSpecifications",
                type: "int",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AddColumn<Guid>(
                name: "ProductId",
                table: "AppKeyboardSpecifications",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<Guid>(
                name: "SwitchTypeId",
                table: "AppKeyboardSpecifications",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AlterColumn<int>(
                name: "Connectivity",
                table: "AppHeadsetSpecifications",
                type: "int",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AddColumn<Guid>(
                name: "ProductId",
                table: "AppHeadsetSpecifications",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<Guid>(
                name: "ProductId",
                table: "AppCpuSpecifications",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<Guid>(
                name: "SocketId",
                table: "AppCpuSpecifications",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AlterColumn<float>(
                name: "Height",
                table: "AppCpuCoolerSpecifications",
                type: "real",
                nullable: true,
                oldClrType: typeof(decimal),
                oldType: "decimal(18,2)",
                oldNullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "ProductId",
                table: "AppCpuCoolerSpecifications",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AlterColumn<float>(
                name: "MaxGpuLength",
                table: "AppCaseSpecifications",
                type: "real",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "decimal(18,2)");

            migrationBuilder.AlterColumn<float>(
                name: "MaxCpuCoolerHeight",
                table: "AppCaseSpecifications",
                type: "real",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "decimal(18,2)");

            migrationBuilder.AddColumn<Guid>(
                name: "FormFactorId",
                table: "AppCaseSpecifications",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<Guid>(
                name: "ProductId",
                table: "AppCaseSpecifications",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<string>(
                name: "ApplicationName",
                table: "AbpBackgroundJobs",
                type: "nvarchar(96)",
                maxLength: 96,
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "ProductId",
                table: "AppAppGpuSpecifications",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddPrimaryKey(
                name: "PK_AppProducts",
                table: "AppProducts",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_AppManufacturers",
                table: "AppManufacturers",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_AppCategories",
                table: "AppCategories",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_AppAppGpuSpecifications",
                table: "AppAppGpuSpecifications",
                column: "Id");

            migrationBuilder.CreateTable(
                name: "AbpAuditLogExcelFiles",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    TenantId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    FileName = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    CreationTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatorId = table.Column<Guid>(type: "uniqueidentifier", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AbpAuditLogExcelFiles", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "AppBlogs",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Title = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Content = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    PublishedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Author = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    UrlSlug = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    AuthorId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    MainImageUrl = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    MainImageId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    ExtraProperties = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ConcurrencyStamp = table.Column<string>(type: "nvarchar(40)", maxLength: 40, nullable: false),
                    CreationTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatorId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    LastModificationTime = table.Column<DateTime>(type: "datetime2", nullable: true),
                    LastModifierId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false, defaultValue: false),
                    DeleterId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    DeletionTime = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AppBlogs", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "AppChipsets",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    CreationTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatorId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    LastModificationTime = table.Column<DateTime>(type: "datetime2", nullable: true),
                    LastModifierId = table.Column<Guid>(type: "uniqueidentifier", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AppChipsets", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "AppComments",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    UserId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CommentedOn = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Content = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    EntityType = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    EntityId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ParentId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    ExtraProperties = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ConcurrencyStamp = table.Column<string>(type: "nvarchar(40)", maxLength: 40, nullable: false),
                    CreationTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatorId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    LastModificationTime = table.Column<DateTime>(type: "datetime2", nullable: true),
                    LastModifierId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false, defaultValue: false),
                    DeleterId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    DeletionTime = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AppComments", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "AppFormFactors",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    CreationTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatorId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    LastModificationTime = table.Column<DateTime>(type: "datetime2", nullable: true),
                    LastModifierId = table.Column<Guid>(type: "uniqueidentifier", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AppFormFactors", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "AppLikes",
                columns: table => new
                {
                    CommentId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    UserId = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AppLikes", x => new { x.CommentId, x.UserId });
                });

            migrationBuilder.CreateTable(
                name: "AppMaterials",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    CreationTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatorId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    LastModificationTime = table.Column<DateTime>(type: "datetime2", nullable: true),
                    LastModifierId = table.Column<Guid>(type: "uniqueidentifier", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AppMaterials", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "AppPanelTypes",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    CreationTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatorId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    LastModificationTime = table.Column<DateTime>(type: "datetime2", nullable: true),
                    LastModifierId = table.Column<Guid>(type: "uniqueidentifier", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AppPanelTypes", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "AppRamTypes",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    CreationTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatorId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    LastModificationTime = table.Column<DateTime>(type: "datetime2", nullable: true),
                    LastModifierId = table.Column<Guid>(type: "uniqueidentifier", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AppRamTypes", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "AppSockets",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    CreationTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatorId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    LastModificationTime = table.Column<DateTime>(type: "datetime2", nullable: true),
                    LastModifierId = table.Column<Guid>(type: "uniqueidentifier", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AppSockets", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "AppSwitchTypes",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    CreationTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatorId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    LastModificationTime = table.Column<DateTime>(type: "datetime2", nullable: true),
                    LastModifierId = table.Column<Guid>(type: "uniqueidentifier", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AppSwitchTypes", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "AppCaseMaterials",
                columns: table => new
                {
                    CaseSpecificationId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    MaterialId = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AppCaseMaterials", x => new { x.CaseSpecificationId, x.MaterialId });
                    table.ForeignKey(
                        name: "FK_AppCaseMaterials_AppCaseSpecifications_CaseSpecificationId",
                        column: x => x.CaseSpecificationId,
                        principalTable: "AppCaseSpecifications",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_AppCaseMaterials_AppMaterials_MaterialId",
                        column: x => x.MaterialId,
                        principalTable: "AppMaterials",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AppCpuCoolerSocketSupports",
                columns: table => new
                {
                    CpuCoolerSpecificationId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    SocketId = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AppCpuCoolerSocketSupports", x => new { x.CpuCoolerSpecificationId, x.SocketId });
                    table.ForeignKey(
                        name: "FK_AppCpuCoolerSocketSupports_AppCpuCoolerSpecifications_CpuCoolerSpecificationId",
                        column: x => x.CpuCoolerSpecificationId,
                        principalTable: "AppCpuCoolerSpecifications",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_AppCpuCoolerSocketSupports_AppSockets_SocketId",
                        column: x => x.SocketId,
                        principalTable: "AppSockets",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_AppStorageSpecifications_FormFactorId",
                table: "AppStorageSpecifications",
                column: "FormFactorId");

            migrationBuilder.CreateIndex(
                name: "IX_AppStorageSpecifications_ProductId",
                table: "AppStorageSpecifications",
                column: "ProductId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_AppRamSpecifications_ProductId",
                table: "AppRamSpecifications",
                column: "ProductId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_AppRamSpecifications_RamTypeId",
                table: "AppRamSpecifications",
                column: "RamTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_AppPsuSpecifications_FormFactorId",
                table: "AppPsuSpecifications",
                column: "FormFactorId");

            migrationBuilder.CreateIndex(
                name: "IX_AppPsuSpecifications_ProductId",
                table: "AppPsuSpecifications",
                column: "ProductId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_AppMouseSpecifications_ProductId",
                table: "AppMouseSpecifications",
                column: "ProductId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_AppMotherboardSpecifications_ChipsetId",
                table: "AppMotherboardSpecifications",
                column: "ChipsetId");

            migrationBuilder.CreateIndex(
                name: "IX_AppMotherboardSpecifications_FormFactorId",
                table: "AppMotherboardSpecifications",
                column: "FormFactorId");

            migrationBuilder.CreateIndex(
                name: "IX_AppMotherboardSpecifications_ProductId",
                table: "AppMotherboardSpecifications",
                column: "ProductId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_AppMotherboardSpecifications_RamTypeId",
                table: "AppMotherboardSpecifications",
                column: "RamTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_AppMotherboardSpecifications_SocketId",
                table: "AppMotherboardSpecifications",
                column: "SocketId");

            migrationBuilder.CreateIndex(
                name: "IX_AppMonitorSpecifications_PanelTypeId",
                table: "AppMonitorSpecifications",
                column: "PanelTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_AppMonitorSpecifications_ProductId",
                table: "AppMonitorSpecifications",
                column: "ProductId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_AppLaptopSpecifications_ProductId",
                table: "AppLaptopSpecifications",
                column: "ProductId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_AppKeyboardSpecifications_ProductId",
                table: "AppKeyboardSpecifications",
                column: "ProductId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_AppKeyboardSpecifications_SwitchTypeId",
                table: "AppKeyboardSpecifications",
                column: "SwitchTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_AppHeadsetSpecifications_ProductId",
                table: "AppHeadsetSpecifications",
                column: "ProductId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_AppCpuSpecifications_ProductId",
                table: "AppCpuSpecifications",
                column: "ProductId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_AppCpuSpecifications_SocketId",
                table: "AppCpuSpecifications",
                column: "SocketId");

            migrationBuilder.CreateIndex(
                name: "IX_AppCpuCoolerSpecifications_ProductId",
                table: "AppCpuCoolerSpecifications",
                column: "ProductId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_AppCaseSpecifications_FormFactorId",
                table: "AppCaseSpecifications",
                column: "FormFactorId");

            migrationBuilder.CreateIndex(
                name: "IX_AppCaseSpecifications_ProductId",
                table: "AppCaseSpecifications",
                column: "ProductId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_AppProducts_UrlSlug",
                table: "AppProducts",
                column: "UrlSlug",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_AppManufacturers_UrlSlug",
                table: "AppManufacturers",
                column: "UrlSlug",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_AppCategories_UrlSlug",
                table: "AppCategories",
                column: "UrlSlug",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_AppAppGpuSpecifications_ProductId",
                table: "AppAppGpuSpecifications",
                column: "ProductId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_AppCaseMaterials_MaterialId",
                table: "AppCaseMaterials",
                column: "MaterialId");

            migrationBuilder.CreateIndex(
                name: "IX_AppComments_EntityType_EntityId",
                table: "AppComments",
                columns: new[] { "EntityType", "EntityId" });

            migrationBuilder.CreateIndex(
                name: "IX_AppCpuCoolerSocketSupports_SocketId",
                table: "AppCpuCoolerSocketSupports",
                column: "SocketId");

            migrationBuilder.CreateIndex(
                name: "IX_AppLikes_CommentId_UserId",
                table: "AppLikes",
                columns: new[] { "CommentId", "UserId" });

            migrationBuilder.AddForeignKey(
                name: "FK_AppAppGpuSpecifications_AppProducts_ProductId",
                table: "AppAppGpuSpecifications",
                column: "ProductId",
                principalTable: "AppProducts",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_AppCaseSpecifications_AppFormFactors_FormFactorId",
                table: "AppCaseSpecifications",
                column: "FormFactorId",
                principalTable: "AppFormFactors",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_AppCaseSpecifications_AppProducts_ProductId",
                table: "AppCaseSpecifications",
                column: "ProductId",
                principalTable: "AppProducts",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_AppCpuCoolerSpecifications_AppProducts_ProductId",
                table: "AppCpuCoolerSpecifications",
                column: "ProductId",
                principalTable: "AppProducts",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_AppCpuSpecifications_AppProducts_ProductId",
                table: "AppCpuSpecifications",
                column: "ProductId",
                principalTable: "AppProducts",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_AppCpuSpecifications_AppSockets_SocketId",
                table: "AppCpuSpecifications",
                column: "SocketId",
                principalTable: "AppSockets",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_AppHeadsetSpecifications_AppProducts_ProductId",
                table: "AppHeadsetSpecifications",
                column: "ProductId",
                principalTable: "AppProducts",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_AppKeyboardSpecifications_AppProducts_ProductId",
                table: "AppKeyboardSpecifications",
                column: "ProductId",
                principalTable: "AppProducts",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_AppKeyboardSpecifications_AppSwitchTypes_SwitchTypeId",
                table: "AppKeyboardSpecifications",
                column: "SwitchTypeId",
                principalTable: "AppSwitchTypes",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_AppLaptopSpecifications_AppProducts_ProductId",
                table: "AppLaptopSpecifications",
                column: "ProductId",
                principalTable: "AppProducts",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_AppMonitorSpecifications_AppPanelTypes_PanelTypeId",
                table: "AppMonitorSpecifications",
                column: "PanelTypeId",
                principalTable: "AppPanelTypes",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_AppMonitorSpecifications_AppProducts_ProductId",
                table: "AppMonitorSpecifications",
                column: "ProductId",
                principalTable: "AppProducts",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_AppMotherboardSpecifications_AppChipsets_ChipsetId",
                table: "AppMotherboardSpecifications",
                column: "ChipsetId",
                principalTable: "AppChipsets",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_AppMotherboardSpecifications_AppFormFactors_FormFactorId",
                table: "AppMotherboardSpecifications",
                column: "FormFactorId",
                principalTable: "AppFormFactors",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_AppMotherboardSpecifications_AppProducts_ProductId",
                table: "AppMotherboardSpecifications",
                column: "ProductId",
                principalTable: "AppProducts",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_AppMotherboardSpecifications_AppRamTypes_RamTypeId",
                table: "AppMotherboardSpecifications",
                column: "RamTypeId",
                principalTable: "AppRamTypes",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_AppMotherboardSpecifications_AppSockets_SocketId",
                table: "AppMotherboardSpecifications",
                column: "SocketId",
                principalTable: "AppSockets",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_AppMouseSpecifications_AppProducts_ProductId",
                table: "AppMouseSpecifications",
                column: "ProductId",
                principalTable: "AppProducts",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_AppProducts_AppCategories_CategoryId",
                table: "AppProducts",
                column: "CategoryId",
                principalTable: "AppCategories",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_AppProducts_AppManufacturers_ManufacturerId",
                table: "AppProducts",
                column: "ManufacturerId",
                principalTable: "AppManufacturers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_AppPsuSpecifications_AppFormFactors_FormFactorId",
                table: "AppPsuSpecifications",
                column: "FormFactorId",
                principalTable: "AppFormFactors",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_AppPsuSpecifications_AppProducts_ProductId",
                table: "AppPsuSpecifications",
                column: "ProductId",
                principalTable: "AppProducts",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_AppRamSpecifications_AppProducts_ProductId",
                table: "AppRamSpecifications",
                column: "ProductId",
                principalTable: "AppProducts",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_AppRamSpecifications_AppRamTypes_RamTypeId",
                table: "AppRamSpecifications",
                column: "RamTypeId",
                principalTable: "AppRamTypes",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_AppStorageSpecifications_AppFormFactors_FormFactorId",
                table: "AppStorageSpecifications",
                column: "FormFactorId",
                principalTable: "AppFormFactors",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_AppStorageSpecifications_AppProducts_ProductId",
                table: "AppStorageSpecifications",
                column: "ProductId",
                principalTable: "AppProducts",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AppAppGpuSpecifications_AppProducts_ProductId",
                table: "AppAppGpuSpecifications");

            migrationBuilder.DropForeignKey(
                name: "FK_AppCaseSpecifications_AppFormFactors_FormFactorId",
                table: "AppCaseSpecifications");

            migrationBuilder.DropForeignKey(
                name: "FK_AppCaseSpecifications_AppProducts_ProductId",
                table: "AppCaseSpecifications");

            migrationBuilder.DropForeignKey(
                name: "FK_AppCpuCoolerSpecifications_AppProducts_ProductId",
                table: "AppCpuCoolerSpecifications");

            migrationBuilder.DropForeignKey(
                name: "FK_AppCpuSpecifications_AppProducts_ProductId",
                table: "AppCpuSpecifications");

            migrationBuilder.DropForeignKey(
                name: "FK_AppCpuSpecifications_AppSockets_SocketId",
                table: "AppCpuSpecifications");

            migrationBuilder.DropForeignKey(
                name: "FK_AppHeadsetSpecifications_AppProducts_ProductId",
                table: "AppHeadsetSpecifications");

            migrationBuilder.DropForeignKey(
                name: "FK_AppKeyboardSpecifications_AppProducts_ProductId",
                table: "AppKeyboardSpecifications");

            migrationBuilder.DropForeignKey(
                name: "FK_AppKeyboardSpecifications_AppSwitchTypes_SwitchTypeId",
                table: "AppKeyboardSpecifications");

            migrationBuilder.DropForeignKey(
                name: "FK_AppLaptopSpecifications_AppProducts_ProductId",
                table: "AppLaptopSpecifications");

            migrationBuilder.DropForeignKey(
                name: "FK_AppMonitorSpecifications_AppPanelTypes_PanelTypeId",
                table: "AppMonitorSpecifications");

            migrationBuilder.DropForeignKey(
                name: "FK_AppMonitorSpecifications_AppProducts_ProductId",
                table: "AppMonitorSpecifications");

            migrationBuilder.DropForeignKey(
                name: "FK_AppMotherboardSpecifications_AppChipsets_ChipsetId",
                table: "AppMotherboardSpecifications");

            migrationBuilder.DropForeignKey(
                name: "FK_AppMotherboardSpecifications_AppFormFactors_FormFactorId",
                table: "AppMotherboardSpecifications");

            migrationBuilder.DropForeignKey(
                name: "FK_AppMotherboardSpecifications_AppProducts_ProductId",
                table: "AppMotherboardSpecifications");

            migrationBuilder.DropForeignKey(
                name: "FK_AppMotherboardSpecifications_AppRamTypes_RamTypeId",
                table: "AppMotherboardSpecifications");

            migrationBuilder.DropForeignKey(
                name: "FK_AppMotherboardSpecifications_AppSockets_SocketId",
                table: "AppMotherboardSpecifications");

            migrationBuilder.DropForeignKey(
                name: "FK_AppMouseSpecifications_AppProducts_ProductId",
                table: "AppMouseSpecifications");

            migrationBuilder.DropForeignKey(
                name: "FK_AppProducts_AppCategories_CategoryId",
                table: "AppProducts");

            migrationBuilder.DropForeignKey(
                name: "FK_AppProducts_AppManufacturers_ManufacturerId",
                table: "AppProducts");

            migrationBuilder.DropForeignKey(
                name: "FK_AppPsuSpecifications_AppFormFactors_FormFactorId",
                table: "AppPsuSpecifications");

            migrationBuilder.DropForeignKey(
                name: "FK_AppPsuSpecifications_AppProducts_ProductId",
                table: "AppPsuSpecifications");

            migrationBuilder.DropForeignKey(
                name: "FK_AppRamSpecifications_AppProducts_ProductId",
                table: "AppRamSpecifications");

            migrationBuilder.DropForeignKey(
                name: "FK_AppRamSpecifications_AppRamTypes_RamTypeId",
                table: "AppRamSpecifications");

            migrationBuilder.DropForeignKey(
                name: "FK_AppStorageSpecifications_AppFormFactors_FormFactorId",
                table: "AppStorageSpecifications");

            migrationBuilder.DropForeignKey(
                name: "FK_AppStorageSpecifications_AppProducts_ProductId",
                table: "AppStorageSpecifications");

            migrationBuilder.DropTable(
                name: "AbpAuditLogExcelFiles");

            migrationBuilder.DropTable(
                name: "AppBlogs");

            migrationBuilder.DropTable(
                name: "AppCaseMaterials");

            migrationBuilder.DropTable(
                name: "AppChipsets");

            migrationBuilder.DropTable(
                name: "AppComments");

            migrationBuilder.DropTable(
                name: "AppCpuCoolerSocketSupports");

            migrationBuilder.DropTable(
                name: "AppFormFactors");

            migrationBuilder.DropTable(
                name: "AppLikes");

            migrationBuilder.DropTable(
                name: "AppPanelTypes");

            migrationBuilder.DropTable(
                name: "AppRamTypes");

            migrationBuilder.DropTable(
                name: "AppSwitchTypes");

            migrationBuilder.DropTable(
                name: "AppMaterials");

            migrationBuilder.DropTable(
                name: "AppSockets");

            migrationBuilder.DropIndex(
                name: "IX_AppStorageSpecifications_FormFactorId",
                table: "AppStorageSpecifications");

            migrationBuilder.DropIndex(
                name: "IX_AppStorageSpecifications_ProductId",
                table: "AppStorageSpecifications");

            migrationBuilder.DropIndex(
                name: "IX_AppRamSpecifications_ProductId",
                table: "AppRamSpecifications");

            migrationBuilder.DropIndex(
                name: "IX_AppRamSpecifications_RamTypeId",
                table: "AppRamSpecifications");

            migrationBuilder.DropIndex(
                name: "IX_AppPsuSpecifications_FormFactorId",
                table: "AppPsuSpecifications");

            migrationBuilder.DropIndex(
                name: "IX_AppPsuSpecifications_ProductId",
                table: "AppPsuSpecifications");

            migrationBuilder.DropIndex(
                name: "IX_AppMouseSpecifications_ProductId",
                table: "AppMouseSpecifications");

            migrationBuilder.DropIndex(
                name: "IX_AppMotherboardSpecifications_ChipsetId",
                table: "AppMotherboardSpecifications");

            migrationBuilder.DropIndex(
                name: "IX_AppMotherboardSpecifications_FormFactorId",
                table: "AppMotherboardSpecifications");

            migrationBuilder.DropIndex(
                name: "IX_AppMotherboardSpecifications_ProductId",
                table: "AppMotherboardSpecifications");

            migrationBuilder.DropIndex(
                name: "IX_AppMotherboardSpecifications_RamTypeId",
                table: "AppMotherboardSpecifications");

            migrationBuilder.DropIndex(
                name: "IX_AppMotherboardSpecifications_SocketId",
                table: "AppMotherboardSpecifications");

            migrationBuilder.DropIndex(
                name: "IX_AppMonitorSpecifications_PanelTypeId",
                table: "AppMonitorSpecifications");

            migrationBuilder.DropIndex(
                name: "IX_AppMonitorSpecifications_ProductId",
                table: "AppMonitorSpecifications");

            migrationBuilder.DropIndex(
                name: "IX_AppLaptopSpecifications_ProductId",
                table: "AppLaptopSpecifications");

            migrationBuilder.DropIndex(
                name: "IX_AppKeyboardSpecifications_ProductId",
                table: "AppKeyboardSpecifications");

            migrationBuilder.DropIndex(
                name: "IX_AppKeyboardSpecifications_SwitchTypeId",
                table: "AppKeyboardSpecifications");

            migrationBuilder.DropIndex(
                name: "IX_AppHeadsetSpecifications_ProductId",
                table: "AppHeadsetSpecifications");

            migrationBuilder.DropIndex(
                name: "IX_AppCpuSpecifications_ProductId",
                table: "AppCpuSpecifications");

            migrationBuilder.DropIndex(
                name: "IX_AppCpuSpecifications_SocketId",
                table: "AppCpuSpecifications");

            migrationBuilder.DropIndex(
                name: "IX_AppCpuCoolerSpecifications_ProductId",
                table: "AppCpuCoolerSpecifications");

            migrationBuilder.DropIndex(
                name: "IX_AppCaseSpecifications_FormFactorId",
                table: "AppCaseSpecifications");

            migrationBuilder.DropIndex(
                name: "IX_AppCaseSpecifications_ProductId",
                table: "AppCaseSpecifications");

            migrationBuilder.DropPrimaryKey(
                name: "PK_AppProducts",
                table: "AppProducts");

            migrationBuilder.DropIndex(
                name: "IX_AppProducts_UrlSlug",
                table: "AppProducts");

            migrationBuilder.DropPrimaryKey(
                name: "PK_AppManufacturers",
                table: "AppManufacturers");

            migrationBuilder.DropIndex(
                name: "IX_AppManufacturers_UrlSlug",
                table: "AppManufacturers");

            migrationBuilder.DropPrimaryKey(
                name: "PK_AppCategories",
                table: "AppCategories");

            migrationBuilder.DropIndex(
                name: "IX_AppCategories_UrlSlug",
                table: "AppCategories");

            migrationBuilder.DropPrimaryKey(
                name: "PK_AppAppGpuSpecifications",
                table: "AppAppGpuSpecifications");

            migrationBuilder.DropIndex(
                name: "IX_AppAppGpuSpecifications_ProductId",
                table: "AppAppGpuSpecifications");

            migrationBuilder.DropColumn(
                name: "FormFactorId",
                table: "AppStorageSpecifications");

            migrationBuilder.DropColumn(
                name: "ProductId",
                table: "AppStorageSpecifications");

            migrationBuilder.DropColumn(
                name: "ProductId",
                table: "AppRamSpecifications");

            migrationBuilder.DropColumn(
                name: "RamTypeId",
                table: "AppRamSpecifications");

            migrationBuilder.DropColumn(
                name: "FormFactorId",
                table: "AppPsuSpecifications");

            migrationBuilder.DropColumn(
                name: "ProductId",
                table: "AppPsuSpecifications");

            migrationBuilder.DropColumn(
                name: "PaymentMethod",
                table: "AppOrders");

            migrationBuilder.DropColumn(
                name: "PaymentStatus",
                table: "AppOrders");

            migrationBuilder.DropColumn(
                name: "ProductId",
                table: "AppMouseSpecifications");

            migrationBuilder.DropColumn(
                name: "ChipsetId",
                table: "AppMotherboardSpecifications");

            migrationBuilder.DropColumn(
                name: "FormFactorId",
                table: "AppMotherboardSpecifications");

            migrationBuilder.DropColumn(
                name: "ProductId",
                table: "AppMotherboardSpecifications");

            migrationBuilder.DropColumn(
                name: "RamTypeId",
                table: "AppMotherboardSpecifications");

            migrationBuilder.DropColumn(
                name: "SocketId",
                table: "AppMotherboardSpecifications");

            migrationBuilder.DropColumn(
                name: "PanelTypeId",
                table: "AppMonitorSpecifications");

            migrationBuilder.DropColumn(
                name: "ProductId",
                table: "AppMonitorSpecifications");

            migrationBuilder.DropColumn(
                name: "ProductId",
                table: "AppLaptopSpecifications");

            migrationBuilder.DropColumn(
                name: "ProductId",
                table: "AppKeyboardSpecifications");

            migrationBuilder.DropColumn(
                name: "SwitchTypeId",
                table: "AppKeyboardSpecifications");

            migrationBuilder.DropColumn(
                name: "ProductId",
                table: "AppHeadsetSpecifications");

            migrationBuilder.DropColumn(
                name: "ProductId",
                table: "AppCpuSpecifications");

            migrationBuilder.DropColumn(
                name: "SocketId",
                table: "AppCpuSpecifications");

            migrationBuilder.DropColumn(
                name: "ProductId",
                table: "AppCpuCoolerSpecifications");

            migrationBuilder.DropColumn(
                name: "FormFactorId",
                table: "AppCaseSpecifications");

            migrationBuilder.DropColumn(
                name: "ProductId",
                table: "AppCaseSpecifications");

            migrationBuilder.DropColumn(
                name: "ApplicationName",
                table: "AbpBackgroundJobs");

            migrationBuilder.DropColumn(
                name: "ProductId",
                table: "AppAppGpuSpecifications");

            migrationBuilder.RenameTable(
                name: "AppProducts",
                newName: "Products");

            migrationBuilder.RenameTable(
                name: "AppManufacturers",
                newName: "Manufacturers");

            migrationBuilder.RenameTable(
                name: "AppCategories",
                newName: "Categories");

            migrationBuilder.RenameTable(
                name: "AppAppGpuSpecifications",
                newName: "AppGpuSpecifications");

            migrationBuilder.RenameIndex(
                name: "IX_AppProducts_ManufacturerId",
                table: "Products",
                newName: "IX_Products_ManufacturerId");

            migrationBuilder.RenameIndex(
                name: "IX_AppProducts_CategoryId",
                table: "Products",
                newName: "IX_Products_CategoryId");

            migrationBuilder.AddColumn<string>(
                name: "FormFactor",
                table: "AppStorageSpecifications",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "RamType",
                table: "AppRamSpecifications",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AlterColumn<string>(
                name: "Modularity",
                table: "AppPsuSpecifications",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AddColumn<string>(
                name: "FormFactor",
                table: "AppPsuSpecifications",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AlterColumn<string>(
                name: "Connectivity",
                table: "AppMouseSpecifications",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AddColumn<string>(
                name: "Chipset",
                table: "AppMotherboardSpecifications",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "FormFactor",
                table: "AppMotherboardSpecifications",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Socket",
                table: "AppMotherboardSpecifications",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "SupportedRamType",
                table: "AppMotherboardSpecifications",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "PanelType",
                table: "AppMonitorSpecifications",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AlterColumn<string>(
                name: "Layout",
                table: "AppKeyboardSpecifications",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AlterColumn<string>(
                name: "Connectivity",
                table: "AppKeyboardSpecifications",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AddColumn<string>(
                name: "SwitchType",
                table: "AppKeyboardSpecifications",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AlterColumn<string>(
                name: "Connectivity",
                table: "AppHeadsetSpecifications",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AddColumn<string>(
                name: "Socket",
                table: "AppCpuSpecifications",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AlterColumn<decimal>(
                name: "Height",
                table: "AppCpuCoolerSpecifications",
                type: "decimal(18,2)",
                nullable: true,
                oldClrType: typeof(float),
                oldType: "real",
                oldNullable: true);

            migrationBuilder.AddColumn<string>(
                name: "SupportedSockets",
                table: "AppCpuCoolerSpecifications",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AlterColumn<decimal>(
                name: "MaxGpuLength",
                table: "AppCaseSpecifications",
                type: "decimal(18,2)",
                nullable: false,
                oldClrType: typeof(float),
                oldType: "real");

            migrationBuilder.AlterColumn<decimal>(
                name: "MaxCpuCoolerHeight",
                table: "AppCaseSpecifications",
                type: "decimal(18,2)",
                nullable: false,
                oldClrType: typeof(float),
                oldType: "real");

            migrationBuilder.AddColumn<string>(
                name: "Material",
                table: "AppCaseSpecifications",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "SupportedMbFormFactor",
                table: "AppCaseSpecifications",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<Guid>(
                name: "CaseSpecificationId",
                table: "Products",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "CpuCoolerSpecificationId",
                table: "Products",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "CpuSpecificationId",
                table: "Products",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "GpuSpecificationId",
                table: "Products",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "HeadsetSpecificationId",
                table: "Products",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "KeyboardSpecificationId",
                table: "Products",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "LaptopSpecificationId",
                table: "Products",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "MonitorSpecificationId",
                table: "Products",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "MotherboardSpecificationId",
                table: "Products",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "MouseSpecificationId",
                table: "Products",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "PsuSpecificationId",
                table: "Products",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "RamSpecificationId",
                table: "Products",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "StorageSpecificationId",
                table: "Products",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AddPrimaryKey(
                name: "PK_Products",
                table: "Products",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Manufacturers",
                table: "Manufacturers",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Categories",
                table: "Categories",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_AppGpuSpecifications",
                table: "AppGpuSpecifications",
                column: "Id");

            migrationBuilder.CreateIndex(
                name: "IX_Products_CaseSpecificationId",
                table: "Products",
                column: "CaseSpecificationId",
                unique: true,
                filter: "[CaseSpecificationId] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_Products_CpuCoolerSpecificationId",
                table: "Products",
                column: "CpuCoolerSpecificationId",
                unique: true,
                filter: "[CpuCoolerSpecificationId] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_Products_CpuSpecificationId",
                table: "Products",
                column: "CpuSpecificationId",
                unique: true,
                filter: "[CpuSpecificationId] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_Products_GpuSpecificationId",
                table: "Products",
                column: "GpuSpecificationId",
                unique: true,
                filter: "[GpuSpecificationId] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_Products_HeadsetSpecificationId",
                table: "Products",
                column: "HeadsetSpecificationId",
                unique: true,
                filter: "[HeadsetSpecificationId] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_Products_KeyboardSpecificationId",
                table: "Products",
                column: "KeyboardSpecificationId",
                unique: true,
                filter: "[KeyboardSpecificationId] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_Products_LaptopSpecificationId",
                table: "Products",
                column: "LaptopSpecificationId",
                unique: true,
                filter: "[LaptopSpecificationId] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_Products_MonitorSpecificationId",
                table: "Products",
                column: "MonitorSpecificationId",
                unique: true,
                filter: "[MonitorSpecificationId] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_Products_MotherboardSpecificationId",
                table: "Products",
                column: "MotherboardSpecificationId",
                unique: true,
                filter: "[MotherboardSpecificationId] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_Products_MouseSpecificationId",
                table: "Products",
                column: "MouseSpecificationId",
                unique: true,
                filter: "[MouseSpecificationId] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_Products_PsuSpecificationId",
                table: "Products",
                column: "PsuSpecificationId",
                unique: true,
                filter: "[PsuSpecificationId] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_Products_RamSpecificationId",
                table: "Products",
                column: "RamSpecificationId",
                unique: true,
                filter: "[RamSpecificationId] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_Products_StorageSpecificationId",
                table: "Products",
                column: "StorageSpecificationId",
                unique: true,
                filter: "[StorageSpecificationId] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_Products_UrlSlug",
                table: "Products",
                column: "UrlSlug");

            migrationBuilder.CreateIndex(
                name: "IX_Manufacturers_UrlSlug",
                table: "Manufacturers",
                column: "UrlSlug");

            migrationBuilder.CreateIndex(
                name: "IX_Categories_UrlSlug",
                table: "Categories",
                column: "UrlSlug");

            migrationBuilder.AddForeignKey(
                name: "FK_Products_AppCaseSpecifications_CaseSpecificationId",
                table: "Products",
                column: "CaseSpecificationId",
                principalTable: "AppCaseSpecifications",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Products_AppCpuCoolerSpecifications_CpuCoolerSpecificationId",
                table: "Products",
                column: "CpuCoolerSpecificationId",
                principalTable: "AppCpuCoolerSpecifications",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Products_AppCpuSpecifications_CpuSpecificationId",
                table: "Products",
                column: "CpuSpecificationId",
                principalTable: "AppCpuSpecifications",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Products_AppGpuSpecifications_GpuSpecificationId",
                table: "Products",
                column: "GpuSpecificationId",
                principalTable: "AppGpuSpecifications",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Products_AppHeadsetSpecifications_HeadsetSpecificationId",
                table: "Products",
                column: "HeadsetSpecificationId",
                principalTable: "AppHeadsetSpecifications",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Products_AppKeyboardSpecifications_KeyboardSpecificationId",
                table: "Products",
                column: "KeyboardSpecificationId",
                principalTable: "AppKeyboardSpecifications",
                principalColumn: "Id");

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
                name: "FK_Products_AppMotherboardSpecifications_MotherboardSpecificationId",
                table: "Products",
                column: "MotherboardSpecificationId",
                principalTable: "AppMotherboardSpecifications",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Products_AppMouseSpecifications_MouseSpecificationId",
                table: "Products",
                column: "MouseSpecificationId",
                principalTable: "AppMouseSpecifications",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Products_AppPsuSpecifications_PsuSpecificationId",
                table: "Products",
                column: "PsuSpecificationId",
                principalTable: "AppPsuSpecifications",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Products_AppRamSpecifications_RamSpecificationId",
                table: "Products",
                column: "RamSpecificationId",
                principalTable: "AppRamSpecifications",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Products_AppStorageSpecifications_StorageSpecificationId",
                table: "Products",
                column: "StorageSpecificationId",
                principalTable: "AppStorageSpecifications",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Products_Categories_CategoryId",
                table: "Products",
                column: "CategoryId",
                principalTable: "Categories",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Products_Manufacturers_ManufacturerId",
                table: "Products",
                column: "ManufacturerId",
                principalTable: "Manufacturers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
