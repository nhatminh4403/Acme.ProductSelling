using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Acme.ProductSelling.Migrations
{
    /// <inheritdoc />
    public partial class updateforeachspecs : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsWireless",
                table: "MouseSpecifications");

            migrationBuilder.DropColumn(
                name: "IncludedFans",
                table: "AppCaseSpecifications");

            migrationBuilder.AddColumn<string>(
                name: "BacklightColor",
                table: "MouseSpecifications",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Color",
                table: "MouseSpecifications",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Connectivity",
                table: "MouseSpecifications",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<int>(
                name: "PollingRate",
                table: "MouseSpecifications",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "SensorType",
                table: "MouseSpecifications",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<int>(
                name: "Weight",
                table: "MouseSpecifications",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "Brightness",
                table: "MonitorSpecifications",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "ColorGamut",
                table: "MonitorSpecifications",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "PanelType",
                table: "MonitorSpecifications",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<int>(
                name: "ResponseTimeMs",
                table: "MonitorSpecifications",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<bool>(
                name: "VesaMount",
                table: "MonitorSpecifications",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AlterColumn<int>(
                name: "DriverSize",
                table: "AppHeadsetSpecifications",
                type: "int",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AddColumn<string>(
                name: "Color",
                table: "AppHeadsetSpecifications",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Frequency",
                table: "AppHeadsetSpecifications",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<int>(
                name: "Impedance",
                table: "AppHeadsetSpecifications",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "MicrophoneType",
                table: "AppHeadsetSpecifications",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<int>(
                name: "Sensitivity",
                table: "AppHeadsetSpecifications",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "Color",
                table: "AppCpuCoolerSpecifications",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "LedLighting",
                table: "AppCpuCoolerSpecifications",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<int>(
                name: "NoiseLevel",
                table: "AppCpuCoolerSpecifications",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "TdpSupport",
                table: "AppCpuCoolerSpecifications",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "CoolingSupport",
                table: "AppCaseSpecifications",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "DriveBays",
                table: "AppCaseSpecifications",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "FanSupport",
                table: "AppCaseSpecifications",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "FrontPanelPorts",
                table: "AppCaseSpecifications",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<float>(
                name: "MaxPsuLength",
                table: "AppCaseSpecifications",
                type: "real",
                nullable: false,
                defaultValue: 0f);

            migrationBuilder.AddColumn<string>(
                name: "RadiatorSupport",
                table: "AppCaseSpecifications",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "BacklightColor",
                table: "MouseSpecifications");

            migrationBuilder.DropColumn(
                name: "Color",
                table: "MouseSpecifications");

            migrationBuilder.DropColumn(
                name: "Connectivity",
                table: "MouseSpecifications");

            migrationBuilder.DropColumn(
                name: "PollingRate",
                table: "MouseSpecifications");

            migrationBuilder.DropColumn(
                name: "SensorType",
                table: "MouseSpecifications");

            migrationBuilder.DropColumn(
                name: "Weight",
                table: "MouseSpecifications");

            migrationBuilder.DropColumn(
                name: "Brightness",
                table: "MonitorSpecifications");

            migrationBuilder.DropColumn(
                name: "ColorGamut",
                table: "MonitorSpecifications");

            migrationBuilder.DropColumn(
                name: "PanelType",
                table: "MonitorSpecifications");

            migrationBuilder.DropColumn(
                name: "ResponseTimeMs",
                table: "MonitorSpecifications");

            migrationBuilder.DropColumn(
                name: "VesaMount",
                table: "MonitorSpecifications");

            migrationBuilder.DropColumn(
                name: "Color",
                table: "AppHeadsetSpecifications");

            migrationBuilder.DropColumn(
                name: "Frequency",
                table: "AppHeadsetSpecifications");

            migrationBuilder.DropColumn(
                name: "Impedance",
                table: "AppHeadsetSpecifications");

            migrationBuilder.DropColumn(
                name: "MicrophoneType",
                table: "AppHeadsetSpecifications");

            migrationBuilder.DropColumn(
                name: "Sensitivity",
                table: "AppHeadsetSpecifications");

            migrationBuilder.DropColumn(
                name: "Color",
                table: "AppCpuCoolerSpecifications");

            migrationBuilder.DropColumn(
                name: "LedLighting",
                table: "AppCpuCoolerSpecifications");

            migrationBuilder.DropColumn(
                name: "NoiseLevel",
                table: "AppCpuCoolerSpecifications");

            migrationBuilder.DropColumn(
                name: "TdpSupport",
                table: "AppCpuCoolerSpecifications");

            migrationBuilder.DropColumn(
                name: "CoolingSupport",
                table: "AppCaseSpecifications");

            migrationBuilder.DropColumn(
                name: "DriveBays",
                table: "AppCaseSpecifications");

            migrationBuilder.DropColumn(
                name: "FanSupport",
                table: "AppCaseSpecifications");

            migrationBuilder.DropColumn(
                name: "FrontPanelPorts",
                table: "AppCaseSpecifications");

            migrationBuilder.DropColumn(
                name: "MaxPsuLength",
                table: "AppCaseSpecifications");

            migrationBuilder.DropColumn(
                name: "RadiatorSupport",
                table: "AppCaseSpecifications");

            migrationBuilder.AddColumn<bool>(
                name: "IsWireless",
                table: "MouseSpecifications",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AlterColumn<string>(
                name: "DriverSize",
                table: "AppHeadsetSpecifications",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AddColumn<int>(
                name: "IncludedFans",
                table: "AppCaseSpecifications",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }
    }
}
