using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Acme.ProductSelling.Migrations
{
    /// <inheritdoc />
    public partial class addproductdetails : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
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

            migrationBuilder.CreateTable(
                name: "AppCaseSpecifications",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    SupportedMbFormFactor = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Material = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Color = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    MaxGpuLength = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    MaxCpuCoolerHeight = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    IncludedFans = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AppCaseSpecifications", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "AppCpuCoolerSpecifications",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CoolerType = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    SupportedSockets = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    FanSize = table.Column<int>(type: "int", nullable: false),
                    RadiatorSize = table.Column<int>(type: "int", nullable: true),
                    Height = table.Column<decimal>(type: "decimal(18,2)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AppCpuCoolerSpecifications", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "AppCpuSpecifications",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Socket = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CoreCount = table.Column<int>(type: "int", nullable: false),
                    ThreadCount = table.Column<int>(type: "int", nullable: false),
                    BaseClock = table.Column<float>(type: "real", nullable: false),
                    BoostClock = table.Column<float>(type: "real", nullable: false),
                    L3Cache = table.Column<int>(type: "int", nullable: false),
                    Tdp = table.Column<int>(type: "int", nullable: false),
                    HasIntegratedGraphics = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AppCpuSpecifications", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "AppGpuSpecifications",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Chipset = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    MemorySize = table.Column<int>(type: "int", nullable: false),
                    MemoryType = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    BoostClock = table.Column<float>(type: "real", nullable: false),
                    Interface = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    RecommendedPsu = table.Column<int>(type: "int", nullable: false),
                    Length = table.Column<decimal>(type: "decimal(18,2)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AppGpuSpecifications", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "AppHeadsetSpecifications",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Connectivity = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    HasMicrophone = table.Column<bool>(type: "bit", nullable: false),
                    IsSurroundSound = table.Column<bool>(type: "bit", nullable: false),
                    IsNoiseCancelling = table.Column<bool>(type: "bit", nullable: false),
                    DriverSize = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AppHeadsetSpecifications", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "AppKeyboardSpecifications",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    KeyboardType = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    SwitchType = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Layout = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Connectivity = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Backlight = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AppKeyboardSpecifications", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "AppMotherboardSpecifications",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Socket = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Chipset = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    FormFactor = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    RamSlots = table.Column<int>(type: "int", nullable: false),
                    MaxRam = table.Column<int>(type: "int", nullable: false),
                    SupportedRamType = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    M2Slots = table.Column<int>(type: "int", nullable: false),
                    SataPorts = table.Column<int>(type: "int", nullable: false),
                    HasWifi = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AppMotherboardSpecifications", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "AppPsuSpecifications",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Wattage = table.Column<int>(type: "int", nullable: false),
                    EfficiencyRating = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Modularity = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    FormFactor = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AppPsuSpecifications", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "AppRamSpecifications",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    RamType = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Capacity = table.Column<int>(type: "int", nullable: false),
                    Speed = table.Column<int>(type: "int", nullable: false),
                    ModuleCount = table.Column<int>(type: "int", nullable: false),
                    Timing = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Voltage = table.Column<float>(type: "real", nullable: false),
                    HasRGB = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AppRamSpecifications", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "AppStorageSpecifications",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    StorageType = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Interface = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Capacity = table.Column<int>(type: "int", nullable: false),
                    ReadSpeed = table.Column<int>(type: "int", nullable: false),
                    WriteSpeed = table.Column<int>(type: "int", nullable: false),
                    FormFactor = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Rpm = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AppStorageSpecifications", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "LaptopSpecifications",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CPU = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    RAM = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Storage = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Display = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    GraphicsCard = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    OperatingSystem = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    BatteryLife = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Weight = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Warranty = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LaptopSpecifications", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "MonitorSpecifications",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    RefreshRate = table.Column<int>(type: "int", nullable: false),
                    Resolution = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ScreenSize = table.Column<float>(type: "real", nullable: false),
                    ResponseTime = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MonitorSpecifications", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "MouseSpecifications",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Dpi = table.Column<int>(type: "int", nullable: false),
                    ButtonCount = table.Column<int>(type: "int", nullable: false),
                    IsWireless = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MouseSpecifications", x => x.Id);
                });

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
                name: "FK_Products_AppMotherboardSpecifications_MotherboardSpecificationId",
                table: "Products",
                column: "MotherboardSpecificationId",
                principalTable: "AppMotherboardSpecifications",
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

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
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
                name: "FK_Products_AppMotherboardSpecifications_MotherboardSpecificationId",
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
                name: "FK_Products_LaptopSpecifications_LaptopSpecificationId",
                table: "Products");

            migrationBuilder.DropForeignKey(
                name: "FK_Products_MonitorSpecifications_MonitorSpecificationId",
                table: "Products");

            migrationBuilder.DropForeignKey(
                name: "FK_Products_MouseSpecifications_MouseSpecificationId",
                table: "Products");

            migrationBuilder.DropTable(
                name: "AppCaseSpecifications");

            migrationBuilder.DropTable(
                name: "AppCpuCoolerSpecifications");

            migrationBuilder.DropTable(
                name: "AppCpuSpecifications");

            migrationBuilder.DropTable(
                name: "AppGpuSpecifications");

            migrationBuilder.DropTable(
                name: "AppHeadsetSpecifications");

            migrationBuilder.DropTable(
                name: "AppKeyboardSpecifications");

            migrationBuilder.DropTable(
                name: "AppMotherboardSpecifications");

            migrationBuilder.DropTable(
                name: "AppPsuSpecifications");

            migrationBuilder.DropTable(
                name: "AppRamSpecifications");

            migrationBuilder.DropTable(
                name: "AppStorageSpecifications");

            migrationBuilder.DropTable(
                name: "LaptopSpecifications");

            migrationBuilder.DropTable(
                name: "MonitorSpecifications");

            migrationBuilder.DropTable(
                name: "MouseSpecifications");

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
        }
    }
}
