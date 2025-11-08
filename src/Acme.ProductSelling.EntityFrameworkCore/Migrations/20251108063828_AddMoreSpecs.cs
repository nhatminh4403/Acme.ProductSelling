using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Acme.ProductSelling.Migrations
{
    /// <inheritdoc />
    public partial class AddMoreSpecs : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_ChatbotTrainingData",
                table: "ChatbotTrainingData");

            migrationBuilder.DropPrimaryKey(
                name: "PK_ChatbotMessages",
                table: "ChatbotMessages");

            migrationBuilder.RenameTable(
                name: "ChatbotTrainingData",
                newName: "AppChatbotTrainingData");

            migrationBuilder.RenameTable(
                name: "ChatbotMessages",
                newName: "AppChatbotMessages");

            migrationBuilder.AddPrimaryKey(
                name: "PK_AppChatbotTrainingData",
                table: "AppChatbotTrainingData",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_AppChatbotMessages",
                table: "AppChatbotMessages",
                column: "Id");

            migrationBuilder.CreateTable(
                name: "AppCableSpecifications",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CableType = table.Column<int>(type: "int", nullable: false),
                    Length = table.Column<float>(type: "real", nullable: false),
                    MaxPower = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    DataTransferSpeed = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Connector1 = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Connector2 = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    IsBraided = table.Column<bool>(type: "bit", nullable: false),
                    Color = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Warranty = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ProductId = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AppCableSpecifications", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AppCableSpecifications_AppProducts_ProductId",
                        column: x => x.ProductId,
                        principalTable: "AppProducts",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AppCaseFanSpecifications",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    FanSize = table.Column<int>(type: "int", nullable: false),
                    MaxRpm = table.Column<int>(type: "int", nullable: false),
                    NoiseLevel = table.Column<float>(type: "real", nullable: false),
                    Airflow = table.Column<float>(type: "real", nullable: false),
                    StaticPressure = table.Column<float>(type: "real", nullable: false),
                    Connector = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    BearingType = table.Column<int>(type: "int", nullable: false),
                    HasRgb = table.Column<bool>(type: "bit", nullable: false),
                    Color = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ProductId = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AppCaseFanSpecifications", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AppCaseFanSpecifications_AppProducts_ProductId",
                        column: x => x.ProductId,
                        principalTable: "AppProducts",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AppChairSpecifications",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ChairType = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Material = table.Column<int>(type: "int", nullable: false),
                    MaxWeight = table.Column<int>(type: "int", nullable: false),
                    ArmrestType = table.Column<int>(type: "int", nullable: false),
                    BackrestAdjustment = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    SeatHeight = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    HasLumbarSupport = table.Column<bool>(type: "bit", nullable: false),
                    HasHeadrest = table.Column<bool>(type: "bit", nullable: false),
                    BaseType = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    WheelType = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Color = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ProductId = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AppChairSpecifications", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AppChairSpecifications_AppProducts_ProductId",
                        column: x => x.ProductId,
                        principalTable: "AppProducts",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AppChargerSpecifications",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ChargerType = table.Column<int>(type: "int", nullable: false),
                    TotalWattage = table.Column<int>(type: "int", nullable: false),
                    PortCount = table.Column<int>(type: "int", nullable: false),
                    UsbCPorts = table.Column<int>(type: "int", nullable: false),
                    UsbAPorts = table.Column<int>(type: "int", nullable: false),
                    MaxOutputPerPort = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    FastChargingProtocols = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CableIncluded = table.Column<bool>(type: "bit", nullable: false),
                    HasFoldablePlug = table.Column<bool>(type: "bit", nullable: false),
                    Technology = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Color = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ProductId = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AppChargerSpecifications", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AppChargerSpecifications_AppProducts_ProductId",
                        column: x => x.ProductId,
                        principalTable: "AppProducts",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AppConsoleSpecifications",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Processor = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Graphics = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    RAM = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Storage = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    OpticalDrive = table.Column<int>(type: "int", nullable: false),
                    MaxResolution = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    MaxFrameRate = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    HDRSupport = table.Column<bool>(type: "bit", nullable: false),
                    Connectivity = table.Column<int>(type: "int", nullable: false),
                    HasEthernet = table.Column<bool>(type: "bit", nullable: false),
                    WifiVersion = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    BluetoothVersion = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ProductId = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AppConsoleSpecifications", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AppConsoleSpecifications_AppProducts_ProductId",
                        column: x => x.ProductId,
                        principalTable: "AppProducts",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AppDeskSpecifications",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Width = table.Column<int>(type: "int", nullable: false),
                    Depth = table.Column<int>(type: "int", nullable: false),
                    Height = table.Column<float>(type: "real", nullable: false),
                    Material = table.Column<int>(type: "int", nullable: false),
                    MaxWeight = table.Column<int>(type: "int", nullable: false),
                    IsHeightAdjustable = table.Column<bool>(type: "bit", nullable: false),
                    HasCableManagement = table.Column<bool>(type: "bit", nullable: false),
                    HasCupHolder = table.Column<bool>(type: "bit", nullable: false),
                    HasHeadphoneHook = table.Column<bool>(type: "bit", nullable: false),
                    SurfaceType = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Color = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ProductId = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AppDeskSpecifications", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AppDeskSpecifications_AppProducts_ProductId",
                        column: x => x.ProductId,
                        principalTable: "AppProducts",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AppHandheldSpecifications",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Processor = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Graphics = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    RAM = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Storage = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Display = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    BatteryLife = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Weight = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    OperatingSystem = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Connectivity = table.Column<int>(type: "int", nullable: false),
                    WifiVersion = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    BluetoothVersion = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ProductId = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AppHandheldSpecifications", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AppHandheldSpecifications_AppProducts_ProductId",
                        column: x => x.ProductId,
                        principalTable: "AppProducts",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AppHubSpecifications",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    HubType = table.Column<int>(type: "int", nullable: false),
                    PortCount = table.Column<int>(type: "int", nullable: false),
                    UsbAPorts = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    UsbCPorts = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    HdmiPorts = table.Column<int>(type: "int", nullable: false),
                    DisplayPorts = table.Column<int>(type: "int", nullable: false),
                    EthernetPort = table.Column<bool>(type: "bit", nullable: false),
                    SdCardReader = table.Column<bool>(type: "bit", nullable: false),
                    AudioJack = table.Column<bool>(type: "bit", nullable: false),
                    MaxDisplays = table.Column<int>(type: "int", nullable: false),
                    MaxResolution = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    PowerDelivery = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Color = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ProductId = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AppHubSpecifications", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AppHubSpecifications_AppProducts_ProductId",
                        column: x => x.ProductId,
                        principalTable: "AppProducts",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AppMemoryCardSpecifications",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Capacity = table.Column<int>(type: "int", nullable: false),
                    CardType = table.Column<int>(type: "int", nullable: false),
                    SpeedClass = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ReadSpeed = table.Column<int>(type: "int", nullable: false),
                    WriteSpeed = table.Column<int>(type: "int", nullable: false),
                    Warranty = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Waterproof = table.Column<bool>(type: "bit", nullable: false),
                    Shockproof = table.Column<bool>(type: "bit", nullable: false),
                    ProductId = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AppMemoryCardSpecifications", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AppMemoryCardSpecifications_AppProducts_ProductId",
                        column: x => x.ProductId,
                        principalTable: "AppProducts",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AppMicrophoneSpecifications",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    MicrophoneType = table.Column<int>(type: "int", nullable: false),
                    PolarPattern = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Frequency = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    SampleRate = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Sensitivity = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Connectivity = table.Column<int>(type: "int", nullable: false),
                    Connection = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    HasShockMount = table.Column<bool>(type: "bit", nullable: false),
                    HasPopFilter = table.Column<bool>(type: "bit", nullable: false),
                    HasRgb = table.Column<bool>(type: "bit", nullable: false),
                    Color = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ProductId = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AppMicrophoneSpecifications", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AppMicrophoneSpecifications_AppProducts_ProductId",
                        column: x => x.ProductId,
                        principalTable: "AppProducts",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AppMousePadSpecifications",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Width = table.Column<int>(type: "int", nullable: false),
                    Height = table.Column<int>(type: "int", nullable: false),
                    Thickness = table.Column<float>(type: "real", nullable: false),
                    Material = table.Column<int>(type: "int", nullable: false),
                    SurfaceType = table.Column<int>(type: "int", nullable: false),
                    BaseType = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    HasRgb = table.Column<bool>(type: "bit", nullable: false),
                    IsWashable = table.Column<bool>(type: "bit", nullable: false),
                    Color = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ProductId = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AppMousePadSpecifications", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AppMousePadSpecifications_AppProducts_ProductId",
                        column: x => x.ProductId,
                        principalTable: "AppProducts",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AppNetworkHardwareSpecifications",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    DeviceType = table.Column<int>(type: "int", nullable: false),
                    WifiStandard = table.Column<int>(type: "int", nullable: false),
                    MaxSpeed = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Frequency = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    EthernetPorts = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    AntennaCount = table.Column<int>(type: "int", nullable: false),
                    HasUsb = table.Column<bool>(type: "bit", nullable: false),
                    SecurityProtocol = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Coverage = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ProductId = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AppNetworkHardwareSpecifications", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AppNetworkHardwareSpecifications_AppProducts_ProductId",
                        column: x => x.ProductId,
                        principalTable: "AppProducts",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AppPowerBankSpecifications",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Capacity = table.Column<int>(type: "int", nullable: false),
                    TotalWattage = table.Column<int>(type: "int", nullable: false),
                    PortCount = table.Column<int>(type: "int", nullable: false),
                    UsbCPorts = table.Column<int>(type: "int", nullable: false),
                    UsbAPorts = table.Column<int>(type: "int", nullable: false),
                    InputPorts = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    MaxOutputPerPort = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    FastChargingProtocols = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    RechargingTime = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    HasDisplay = table.Column<bool>(type: "bit", nullable: false),
                    Weight = table.Column<int>(type: "int", nullable: false),
                    Color = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ProductId = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AppPowerBankSpecifications", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AppPowerBankSpecifications_AppProducts_ProductId",
                        column: x => x.ProductId,
                        principalTable: "AppProducts",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AppSoftwareSpecifications",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    SoftwareType = table.Column<int>(type: "int", nullable: false),
                    LicenseType = table.Column<int>(type: "int", nullable: false),
                    Platform = table.Column<int>(type: "int", nullable: false),
                    Version = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Language = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    DeliveryMethod = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    SystemRequirements = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    IsSubscription = table.Column<bool>(type: "bit", nullable: false),
                    ProductId = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AppSoftwareSpecifications", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AppSoftwareSpecifications_AppProducts_ProductId",
                        column: x => x.ProductId,
                        principalTable: "AppProducts",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AppSpeakerSpecifications",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    SpeakerType = table.Column<int>(type: "int", nullable: false),
                    TotalWattage = table.Column<int>(type: "int", nullable: false),
                    Frequency = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Connectivity = table.Column<int>(type: "int", nullable: false),
                    InputPorts = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    HasBluetooth = table.Column<bool>(type: "bit", nullable: false),
                    HasRemote = table.Column<bool>(type: "bit", nullable: false),
                    Color = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ProductId = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AppSpeakerSpecifications", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AppSpeakerSpecifications_AppProducts_ProductId",
                        column: x => x.ProductId,
                        principalTable: "AppProducts",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AppWebcamSpecifications",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Resolution = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    FrameRate = table.Column<int>(type: "int", nullable: false),
                    FocusType = table.Column<int>(type: "int", nullable: false),
                    FieldOfView = table.Column<int>(type: "int", nullable: false),
                    Connectivity = table.Column<int>(type: "int", nullable: false),
                    Connection = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    HasMicrophone = table.Column<bool>(type: "bit", nullable: false),
                    HasPrivacyShutter = table.Column<bool>(type: "bit", nullable: false),
                    MountType = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Color = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ProductId = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AppWebcamSpecifications", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AppWebcamSpecifications_AppProducts_ProductId",
                        column: x => x.ProductId,
                        principalTable: "AppProducts",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_AppCableSpecifications_ProductId",
                table: "AppCableSpecifications",
                column: "ProductId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_AppCaseFanSpecifications_ProductId",
                table: "AppCaseFanSpecifications",
                column: "ProductId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_AppChairSpecifications_ProductId",
                table: "AppChairSpecifications",
                column: "ProductId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_AppChargerSpecifications_ProductId",
                table: "AppChargerSpecifications",
                column: "ProductId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_AppConsoleSpecifications_ProductId",
                table: "AppConsoleSpecifications",
                column: "ProductId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_AppDeskSpecifications_ProductId",
                table: "AppDeskSpecifications",
                column: "ProductId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_AppHandheldSpecifications_ProductId",
                table: "AppHandheldSpecifications",
                column: "ProductId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_AppHubSpecifications_ProductId",
                table: "AppHubSpecifications",
                column: "ProductId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_AppMemoryCardSpecifications_ProductId",
                table: "AppMemoryCardSpecifications",
                column: "ProductId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_AppMicrophoneSpecifications_ProductId",
                table: "AppMicrophoneSpecifications",
                column: "ProductId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_AppMousePadSpecifications_ProductId",
                table: "AppMousePadSpecifications",
                column: "ProductId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_AppNetworkHardwareSpecifications_ProductId",
                table: "AppNetworkHardwareSpecifications",
                column: "ProductId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_AppPowerBankSpecifications_ProductId",
                table: "AppPowerBankSpecifications",
                column: "ProductId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_AppSoftwareSpecifications_ProductId",
                table: "AppSoftwareSpecifications",
                column: "ProductId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_AppSpeakerSpecifications_ProductId",
                table: "AppSpeakerSpecifications",
                column: "ProductId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_AppWebcamSpecifications_ProductId",
                table: "AppWebcamSpecifications",
                column: "ProductId",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AppCableSpecifications");

            migrationBuilder.DropTable(
                name: "AppCaseFanSpecifications");

            migrationBuilder.DropTable(
                name: "AppChairSpecifications");

            migrationBuilder.DropTable(
                name: "AppChargerSpecifications");

            migrationBuilder.DropTable(
                name: "AppConsoleSpecifications");

            migrationBuilder.DropTable(
                name: "AppDeskSpecifications");

            migrationBuilder.DropTable(
                name: "AppHandheldSpecifications");

            migrationBuilder.DropTable(
                name: "AppHubSpecifications");

            migrationBuilder.DropTable(
                name: "AppMemoryCardSpecifications");

            migrationBuilder.DropTable(
                name: "AppMicrophoneSpecifications");

            migrationBuilder.DropTable(
                name: "AppMousePadSpecifications");

            migrationBuilder.DropTable(
                name: "AppNetworkHardwareSpecifications");

            migrationBuilder.DropTable(
                name: "AppPowerBankSpecifications");

            migrationBuilder.DropTable(
                name: "AppSoftwareSpecifications");

            migrationBuilder.DropTable(
                name: "AppSpeakerSpecifications");

            migrationBuilder.DropTable(
                name: "AppWebcamSpecifications");

            migrationBuilder.DropPrimaryKey(
                name: "PK_AppChatbotTrainingData",
                table: "AppChatbotTrainingData");

            migrationBuilder.DropPrimaryKey(
                name: "PK_AppChatbotMessages",
                table: "AppChatbotMessages");

            migrationBuilder.RenameTable(
                name: "AppChatbotTrainingData",
                newName: "ChatbotTrainingData");

            migrationBuilder.RenameTable(
                name: "AppChatbotMessages",
                newName: "ChatbotMessages");

            migrationBuilder.AddPrimaryKey(
                name: "PK_ChatbotTrainingData",
                table: "ChatbotTrainingData",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_ChatbotMessages",
                table: "ChatbotMessages",
                column: "Id");
        }
    }
}
