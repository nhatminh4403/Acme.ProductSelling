using Microsoft.EntityFrameworkCore.Migrations;
using System;

#nullable disable

namespace Acme.ProductSelling.Migrations
{
    /// <inheritdoc />
    public partial class Initial : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
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
                name: "AbpAuditLogs",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ApplicationName = table.Column<string>(type: "nvarchar(96)", maxLength: 96, nullable: true),
                    UserId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    UserName = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    TenantId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    TenantName = table.Column<string>(type: "nvarchar(64)", maxLength: 64, nullable: true),
                    ImpersonatorUserId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    ImpersonatorUserName = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    ImpersonatorTenantId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    ImpersonatorTenantName = table.Column<string>(type: "nvarchar(64)", maxLength: 64, nullable: true),
                    ExecutionTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ExecutionDuration = table.Column<int>(type: "int", nullable: false),
                    ClientIpAddress = table.Column<string>(type: "nvarchar(64)", maxLength: 64, nullable: true),
                    ClientName = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: true),
                    ClientId = table.Column<string>(type: "nvarchar(64)", maxLength: 64, nullable: true),
                    CorrelationId = table.Column<string>(type: "nvarchar(64)", maxLength: 64, nullable: true),
                    BrowserInfo = table.Column<string>(type: "nvarchar(512)", maxLength: 512, nullable: true),
                    HttpMethod = table.Column<string>(type: "nvarchar(16)", maxLength: 16, nullable: true),
                    Url = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    Exceptions = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Comments = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    HttpStatusCode = table.Column<int>(type: "int", nullable: true),
                    ExtraProperties = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ConcurrencyStamp = table.Column<string>(type: "nvarchar(40)", maxLength: 40, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AbpAuditLogs", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "AbpBackgroundJobs",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ApplicationName = table.Column<string>(type: "nvarchar(96)", maxLength: 96, nullable: true),
                    JobName = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: false),
                    JobArgs = table.Column<string>(type: "nvarchar(max)", maxLength: 1048576, nullable: false),
                    TryCount = table.Column<short>(type: "smallint", nullable: false, defaultValue: (short)0),
                    CreationTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    NextTryTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    LastTryTime = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsAbandoned = table.Column<bool>(type: "bit", nullable: false, defaultValue: false),
                    Priority = table.Column<byte>(type: "tinyint", nullable: false, defaultValue: (byte)15),
                    ExtraProperties = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ConcurrencyStamp = table.Column<string>(type: "nvarchar(40)", maxLength: 40, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AbpBackgroundJobs", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "AbpBlobContainers",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    TenantId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    Name = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: false),
                    ExtraProperties = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ConcurrencyStamp = table.Column<string>(type: "nvarchar(40)", maxLength: 40, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AbpBlobContainers", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "AbpClaimTypes",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: false),
                    Required = table.Column<bool>(type: "bit", nullable: false),
                    IsStatic = table.Column<bool>(type: "bit", nullable: false),
                    Regex = table.Column<string>(type: "nvarchar(512)", maxLength: 512, nullable: true),
                    RegexDescription = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: true),
                    Description = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    ValueType = table.Column<int>(type: "int", nullable: false),
                    CreationTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ExtraProperties = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ConcurrencyStamp = table.Column<string>(type: "nvarchar(40)", maxLength: 40, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AbpClaimTypes", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "AbpFeatureGroups",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: false),
                    DisplayName = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: false),
                    ExtraProperties = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AbpFeatureGroups", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "AbpFeatures",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    GroupName = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: false),
                    Name = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: false),
                    ParentName = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: true),
                    DisplayName = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: false),
                    Description = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    DefaultValue = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    IsVisibleToClients = table.Column<bool>(type: "bit", nullable: false),
                    IsAvailableToHost = table.Column<bool>(type: "bit", nullable: false),
                    AllowedProviders = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    ValueType = table.Column<string>(type: "nvarchar(2048)", maxLength: 2048, nullable: true),
                    ExtraProperties = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AbpFeatures", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "AbpFeatureValues",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: false),
                    Value = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: false),
                    ProviderName = table.Column<string>(type: "nvarchar(64)", maxLength: 64, nullable: true),
                    ProviderKey = table.Column<string>(type: "nvarchar(64)", maxLength: 64, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AbpFeatureValues", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "AbpLinkUsers",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    SourceUserId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    SourceTenantId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    TargetUserId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    TargetTenantId = table.Column<Guid>(type: "uniqueidentifier", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AbpLinkUsers", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "AbpOrganizationUnits",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    TenantId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    ParentId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    Code = table.Column<string>(type: "nvarchar(95)", maxLength: 95, nullable: false),
                    DisplayName = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: false),
                    EntityVersion = table.Column<int>(type: "int", nullable: false),
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
                    table.PrimaryKey("PK_AbpOrganizationUnits", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AbpOrganizationUnits_AbpOrganizationUnits_ParentId",
                        column: x => x.ParentId,
                        principalTable: "AbpOrganizationUnits",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "AbpPermissionGrants",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    TenantId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    Name = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: false),
                    ProviderName = table.Column<string>(type: "nvarchar(64)", maxLength: 64, nullable: false),
                    ProviderKey = table.Column<string>(type: "nvarchar(64)", maxLength: 64, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AbpPermissionGrants", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "AbpPermissionGroups",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: false),
                    DisplayName = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: false),
                    ExtraProperties = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AbpPermissionGroups", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "AbpPermissions",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    GroupName = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: false),
                    Name = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: false),
                    ParentName = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: true),
                    DisplayName = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: false),
                    IsEnabled = table.Column<bool>(type: "bit", nullable: false),
                    MultiTenancySide = table.Column<byte>(type: "tinyint", nullable: false),
                    Providers = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: true),
                    StateCheckers = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    ExtraProperties = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AbpPermissions", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "AbpRoles",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    TenantId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    Name = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: false),
                    NormalizedName = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: false),
                    IsDefault = table.Column<bool>(type: "bit", nullable: false),
                    IsStatic = table.Column<bool>(type: "bit", nullable: false),
                    IsPublic = table.Column<bool>(type: "bit", nullable: false),
                    EntityVersion = table.Column<int>(type: "int", nullable: false),
                    CreationTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ExtraProperties = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ConcurrencyStamp = table.Column<string>(type: "nvarchar(40)", maxLength: 40, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AbpRoles", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "AbpSecurityLogs",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    TenantId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    ApplicationName = table.Column<string>(type: "nvarchar(96)", maxLength: 96, nullable: true),
                    Identity = table.Column<string>(type: "nvarchar(96)", maxLength: 96, nullable: true),
                    Action = table.Column<string>(type: "nvarchar(96)", maxLength: 96, nullable: true),
                    UserId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    UserName = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    TenantName = table.Column<string>(type: "nvarchar(64)", maxLength: 64, nullable: true),
                    ClientId = table.Column<string>(type: "nvarchar(64)", maxLength: 64, nullable: true),
                    CorrelationId = table.Column<string>(type: "nvarchar(64)", maxLength: 64, nullable: true),
                    ClientIpAddress = table.Column<string>(type: "nvarchar(64)", maxLength: 64, nullable: true),
                    BrowserInfo = table.Column<string>(type: "nvarchar(512)", maxLength: 512, nullable: true),
                    CreationTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ExtraProperties = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ConcurrencyStamp = table.Column<string>(type: "nvarchar(40)", maxLength: 40, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AbpSecurityLogs", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "AbpSessions",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    SessionId = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: false),
                    Device = table.Column<string>(type: "nvarchar(64)", maxLength: 64, nullable: false),
                    DeviceInfo = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    TenantId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    UserId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ClientId = table.Column<string>(type: "nvarchar(64)", maxLength: 64, nullable: true),
                    IpAddresses = table.Column<string>(type: "nvarchar(2048)", maxLength: 2048, nullable: true),
                    SignedIn = table.Column<DateTime>(type: "datetime2", nullable: false),
                    LastAccessed = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ExtraProperties = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AbpSessions", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "AbpSettingDefinitions",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: false),
                    DisplayName = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: false),
                    Description = table.Column<string>(type: "nvarchar(512)", maxLength: 512, nullable: true),
                    DefaultValue = table.Column<string>(type: "nvarchar(2048)", maxLength: 2048, nullable: true),
                    IsVisibleToClients = table.Column<bool>(type: "bit", nullable: false),
                    Providers = table.Column<string>(type: "nvarchar(1024)", maxLength: 1024, nullable: true),
                    IsInherited = table.Column<bool>(type: "bit", nullable: false),
                    IsEncrypted = table.Column<bool>(type: "bit", nullable: false),
                    ExtraProperties = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AbpSettingDefinitions", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "AbpSettings",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: false),
                    Value = table.Column<string>(type: "nvarchar(2048)", maxLength: 2048, nullable: false),
                    ProviderName = table.Column<string>(type: "nvarchar(64)", maxLength: 64, nullable: true),
                    ProviderKey = table.Column<string>(type: "nvarchar(64)", maxLength: 64, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AbpSettings", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "AbpTenants",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(64)", maxLength: 64, nullable: false),
                    NormalizedName = table.Column<string>(type: "nvarchar(64)", maxLength: 64, nullable: false),
                    EntityVersion = table.Column<int>(type: "int", nullable: false),
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
                    table.PrimaryKey("PK_AbpTenants", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "AbpUserDelegations",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    TenantId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    SourceUserId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    TargetUserId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    StartTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    EndTime = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AbpUserDelegations", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "AbpUsers",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    TenantId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    UserName = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: false),
                    NormalizedUserName = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: false),
                    Name = table.Column<string>(type: "nvarchar(64)", maxLength: 64, nullable: true),
                    Surname = table.Column<string>(type: "nvarchar(64)", maxLength: 64, nullable: true),
                    Email = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: false),
                    NormalizedEmail = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: false),
                    EmailConfirmed = table.Column<bool>(type: "bit", nullable: false, defaultValue: false),
                    PasswordHash = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    SecurityStamp = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: false),
                    IsExternal = table.Column<bool>(type: "bit", nullable: false, defaultValue: false),
                    PhoneNumber = table.Column<string>(type: "nvarchar(16)", maxLength: 16, nullable: true),
                    PhoneNumberConfirmed = table.Column<bool>(type: "bit", nullable: false, defaultValue: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    TwoFactorEnabled = table.Column<bool>(type: "bit", nullable: false, defaultValue: false),
                    LockoutEnd = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true),
                    LockoutEnabled = table.Column<bool>(type: "bit", nullable: false, defaultValue: false),
                    AccessFailedCount = table.Column<int>(type: "int", nullable: false, defaultValue: 0),
                    ShouldChangePasswordOnNextLogin = table.Column<bool>(type: "bit", nullable: false),
                    EntityVersion = table.Column<int>(type: "int", nullable: false),
                    LastPasswordChangeTime = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true),
                    Discriminator = table.Column<string>(type: "nvarchar(13)", maxLength: 13, nullable: false),
                    AssignedStoreId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
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
                    table.PrimaryKey("PK_AbpUsers", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "AppCarts",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    UserId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ExtraProperties = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ConcurrencyStamp = table.Column<string>(type: "nvarchar(40)", maxLength: 40, nullable: false),
                    CreationTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatorId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    LastModificationTime = table.Column<DateTime>(type: "datetime2", nullable: true),
                    LastModifierId = table.Column<Guid>(type: "uniqueidentifier", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AppCarts", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "AppCategories",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    SpecificationType = table.Column<int>(type: "int", nullable: false),
                    CategoryGroup = table.Column<int>(type: "int", nullable: false),
                    DisplayOrder = table.Column<int>(type: "int", nullable: false),
                    IconCssClass = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    UrlSlug = table.Column<string>(type: "nvarchar(450)", nullable: false),
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
                    table.PrimaryKey("PK_AppCategories", x => x.Id);
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
                name: "AppManufacturers",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ContactInfo = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    UrlSlug = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    ManufacturerImage = table.Column<string>(type: "nvarchar(max)", nullable: false),
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
                    table.PrimaryKey("PK_AppManufacturers", x => x.Id);
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
                name: "AppOrders",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    OrderNumber = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: false),
                    OrderDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CustomerId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    CustomerName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    CustomerPhone = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: false),
                    TotalAmount = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    PaymentMethod = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    OrderStatus = table.Column<int>(type: "int", nullable: false),
                    PaymentStatus = table.Column<int>(type: "int", nullable: false),
                    OrderType = table.Column<int>(type: "int", nullable: false),
                    StoreId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    SellerId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    SellerName = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: true),
                    CashierId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    CashierName = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: true),
                    FulfillerId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    FulfillerName = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: true),
                    CompletedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    FulfilledAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ShippingAddress = table.Column<string>(type: "nvarchar(max)", nullable: true),
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
                    table.PrimaryKey("PK_AppOrders", x => x.Id);
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
                name: "AppStores",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Code = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    Address = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    City = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    PhoneNumber = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Email = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    ManagerName = table.Column<string>(type: "nvarchar(max)", nullable: false),
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
                    table.PrimaryKey("PK_AppStores", x => x.Id);
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
                name: "OpenIddictApplications",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ApplicationType = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    ClientId = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    ClientSecret = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ClientType = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    ConsentType = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    DisplayName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    DisplayNames = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    JsonWebKeySet = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Permissions = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PostLogoutRedirectUris = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Properties = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    RedirectUris = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Requirements = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Settings = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    FrontChannelLogoutUri = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ClientUri = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    LogoUri = table.Column<string>(type: "nvarchar(max)", nullable: true),
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
                    table.PrimaryKey("PK_OpenIddictApplications", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "OpenIddictScopes",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Descriptions = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    DisplayName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    DisplayNames = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Name = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    Properties = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Resources = table.Column<string>(type: "nvarchar(max)", nullable: true),
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
                    table.PrimaryKey("PK_OpenIddictScopes", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "AbpAuditLogActions",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    TenantId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    AuditLogId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ServiceName = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    MethodName = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: true),
                    Parameters = table.Column<string>(type: "nvarchar(2000)", maxLength: 2000, nullable: true),
                    ExecutionTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ExecutionDuration = table.Column<int>(type: "int", nullable: false),
                    ExtraProperties = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AbpAuditLogActions", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AbpAuditLogActions_AbpAuditLogs_AuditLogId",
                        column: x => x.AuditLogId,
                        principalTable: "AbpAuditLogs",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AbpEntityChanges",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    AuditLogId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    TenantId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    ChangeTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ChangeType = table.Column<byte>(type: "tinyint", nullable: false),
                    EntityTenantId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    EntityId = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: true),
                    EntityTypeFullName = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: false),
                    ExtraProperties = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AbpEntityChanges", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AbpEntityChanges_AbpAuditLogs_AuditLogId",
                        column: x => x.AuditLogId,
                        principalTable: "AbpAuditLogs",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AbpBlobs",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ContainerId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    TenantId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    Name = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: false),
                    Content = table.Column<byte[]>(type: "varbinary(max)", maxLength: 2147483647, nullable: true),
                    ExtraProperties = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ConcurrencyStamp = table.Column<string>(type: "nvarchar(40)", maxLength: 40, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AbpBlobs", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AbpBlobs_AbpBlobContainers_ContainerId",
                        column: x => x.ContainerId,
                        principalTable: "AbpBlobContainers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AbpOrganizationUnitRoles",
                columns: table => new
                {
                    RoleId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    OrganizationUnitId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    TenantId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    CreationTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatorId = table.Column<Guid>(type: "uniqueidentifier", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AbpOrganizationUnitRoles", x => new { x.OrganizationUnitId, x.RoleId });
                    table.ForeignKey(
                        name: "FK_AbpOrganizationUnitRoles_AbpOrganizationUnits_OrganizationUnitId",
                        column: x => x.OrganizationUnitId,
                        principalTable: "AbpOrganizationUnits",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_AbpOrganizationUnitRoles_AbpRoles_RoleId",
                        column: x => x.RoleId,
                        principalTable: "AbpRoles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AbpRoleClaims",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    RoleId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    TenantId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    ClaimType = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: false),
                    ClaimValue = table.Column<string>(type: "nvarchar(1024)", maxLength: 1024, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AbpRoleClaims", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AbpRoleClaims_AbpRoles_RoleId",
                        column: x => x.RoleId,
                        principalTable: "AbpRoles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AbpTenantConnectionStrings",
                columns: table => new
                {
                    TenantId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(64)", maxLength: 64, nullable: false),
                    Value = table.Column<string>(type: "nvarchar(1024)", maxLength: 1024, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AbpTenantConnectionStrings", x => new { x.TenantId, x.Name });
                    table.ForeignKey(
                        name: "FK_AbpTenantConnectionStrings_AbpTenants_TenantId",
                        column: x => x.TenantId,
                        principalTable: "AbpTenants",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AbpUserClaims",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    UserId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    TenantId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    ClaimType = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: false),
                    ClaimValue = table.Column<string>(type: "nvarchar(1024)", maxLength: 1024, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AbpUserClaims", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AbpUserClaims_AbpUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AbpUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AbpUserLogins",
                columns: table => new
                {
                    UserId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    LoginProvider = table.Column<string>(type: "nvarchar(64)", maxLength: 64, nullable: false),
                    TenantId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    ProviderKey = table.Column<string>(type: "nvarchar(196)", maxLength: 196, nullable: false),
                    ProviderDisplayName = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AbpUserLogins", x => new { x.UserId, x.LoginProvider });
                    table.ForeignKey(
                        name: "FK_AbpUserLogins_AbpUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AbpUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AbpUserOrganizationUnits",
                columns: table => new
                {
                    UserId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    OrganizationUnitId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    TenantId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    CreationTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatorId = table.Column<Guid>(type: "uniqueidentifier", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AbpUserOrganizationUnits", x => new { x.OrganizationUnitId, x.UserId });
                    table.ForeignKey(
                        name: "FK_AbpUserOrganizationUnits_AbpOrganizationUnits_OrganizationUnitId",
                        column: x => x.OrganizationUnitId,
                        principalTable: "AbpOrganizationUnits",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_AbpUserOrganizationUnits_AbpUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AbpUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AbpUserRoles",
                columns: table => new
                {
                    UserId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    RoleId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    TenantId = table.Column<Guid>(type: "uniqueidentifier", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AbpUserRoles", x => new { x.UserId, x.RoleId });
                    table.ForeignKey(
                        name: "FK_AbpUserRoles_AbpRoles_RoleId",
                        column: x => x.RoleId,
                        principalTable: "AbpRoles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_AbpUserRoles_AbpUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AbpUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AbpUserTokens",
                columns: table => new
                {
                    UserId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    LoginProvider = table.Column<string>(type: "nvarchar(64)", maxLength: 64, nullable: false),
                    Name = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: false),
                    TenantId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    Value = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AbpUserTokens", x => new { x.UserId, x.LoginProvider, x.Name });
                    table.ForeignKey(
                        name: "FK_AbpUserTokens_AbpUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AbpUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AppCustomers",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    AppUserId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ShippingAddress = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    DateOfBirth = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Gender = table.Column<int>(type: "int", nullable: false, defaultValue: 0),
                    FullName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Surname = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Email = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    PhoneNumber = table.Column<string>(type: "nvarchar(max)", nullable: true),
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
                    table.PrimaryKey("PK_AppCustomers", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AppCustomers_AbpUsers_AppUserId",
                        column: x => x.AppUserId,
                        principalTable: "AbpUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "AppCartItems",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ProductId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CartId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Quantity = table.Column<int>(type: "int", nullable: false),
                    ProductName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ProductPrice = table.Column<decimal>(type: "decimal(18,2)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AppCartItems", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AppCartItems_AppCarts_CartId",
                        column: x => x.CartId,
                        principalTable: "AppCarts",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AppProducts",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ProductName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    OriginalPrice = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    DiscountedPrice = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    DiscountPercent = table.Column<double>(type: "float", nullable: false),
                    ReleaseDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    StockCount = table.Column<int>(type: "int", nullable: false),
                    UrlSlug = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: false),
                    ImageUrl = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CategoryId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ManufacturerId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
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
                    table.PrimaryKey("PK_AppProducts", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AppProducts_AppCategories_CategoryId",
                        column: x => x.CategoryId,
                        principalTable: "AppCategories",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_AppProducts_AppManufacturers_ManufacturerId",
                        column: x => x.ManufacturerId,
                        principalTable: "AppManufacturers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

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

            migrationBuilder.CreateTable(
                name: "AppOrderItems",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ProductId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ProductName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Price = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    Quantity = table.Column<int>(type: "int", nullable: false),
                    OrderId = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AppOrderItems", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AppOrderItems_AppOrders_OrderId",
                        column: x => x.OrderId,
                        principalTable: "AppOrders",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "OpenIddictAuthorizations",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ApplicationId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    CreationDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Properties = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Scopes = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Status = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    Subject = table.Column<string>(type: "nvarchar(400)", maxLength: 400, nullable: true),
                    Type = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    ExtraProperties = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ConcurrencyStamp = table.Column<string>(type: "nvarchar(40)", maxLength: 40, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OpenIddictAuthorizations", x => x.Id);
                    table.ForeignKey(
                        name: "FK_OpenIddictAuthorizations_OpenIddictApplications_ApplicationId",
                        column: x => x.ApplicationId,
                        principalTable: "OpenIddictApplications",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "AbpEntityPropertyChanges",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    TenantId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    EntityChangeId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    NewValue = table.Column<string>(type: "nvarchar(512)", maxLength: 512, nullable: true),
                    OriginalValue = table.Column<string>(type: "nvarchar(512)", maxLength: 512, nullable: true),
                    PropertyName = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: false),
                    PropertyTypeFullName = table.Column<string>(type: "nvarchar(64)", maxLength: 64, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AbpEntityPropertyChanges", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AbpEntityPropertyChanges_AbpEntityChanges_EntityChangeId",
                        column: x => x.EntityChangeId,
                        principalTable: "AbpEntityChanges",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

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

            migrationBuilder.CreateTable(
                name: "AppSpecifications",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ProductId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    SpecType = table.Column<string>(type: "nvarchar(21)", maxLength: 21, nullable: false),
                    CableType = table.Column<int>(type: "int", nullable: true),
                    Length = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    MaxPower = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    DataTransferSpeed = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Connector1 = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Connector2 = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IsBraided = table.Column<bool>(type: "bit", nullable: true),
                    CableColor = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CableWarranty = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CaseFanFanSize = table.Column<int>(type: "int", nullable: true),
                    MaxRpm = table.Column<int>(type: "int", nullable: true),
                    CaseFanNoiseLevel = table.Column<float>(type: "real", nullable: true),
                    Airflow = table.Column<float>(type: "real", nullable: true),
                    StaticPressure = table.Column<float>(type: "real", nullable: true),
                    Connector = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    BearingType = table.Column<int>(type: "int", nullable: true),
                    CaseFanHasRgb = table.Column<bool>(type: "bit", nullable: true),
                    CaseFanColor = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CaseFormFactorId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    CaseColor = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    MaxGpuLength = table.Column<float>(type: "real", nullable: true),
                    MaxCpuCoolerHeight = table.Column<float>(type: "real", nullable: true),
                    MaxPsuLength = table.Column<float>(type: "real", nullable: true),
                    CoolingSupport = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    FanSupport = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    RadiatorSupport = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    DriveBays = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    FrontPanelPorts = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ChairType = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ChairMaterial = table.Column<int>(type: "int", nullable: true),
                    ChairMaxWeight = table.Column<int>(type: "int", nullable: true),
                    ArmrestType = table.Column<int>(type: "int", nullable: true),
                    BackrestAdjustment = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    SeatHeight = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    HasLumbarSupport = table.Column<bool>(type: "bit", nullable: true),
                    HasHeadrest = table.Column<bool>(type: "bit", nullable: true),
                    ChairBaseType = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    WheelType = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ChairColor = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ChargerType = table.Column<int>(type: "int", nullable: true),
                    ChargerTotalWattage = table.Column<int>(type: "int", nullable: true),
                    ChargerPortCount = table.Column<int>(type: "int", nullable: true),
                    ChargerUsbCPorts = table.Column<int>(type: "int", nullable: true),
                    ChargerUsbAPorts = table.Column<int>(type: "int", nullable: true),
                    ChargerMaxOutputPerPort = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ChargerFastChargingProtocols = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CableIncluded = table.Column<bool>(type: "bit", nullable: true),
                    HasFoldablePlug = table.Column<bool>(type: "bit", nullable: true),
                    Technology = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ChargerColor = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ConsoleProcessor = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ConsoleGraphics = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ConsoleRAM = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ConsoleStorage = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    OpticalDrive = table.Column<int>(type: "int", nullable: true),
                    ConsoleMaxResolution = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    MaxFrameRate = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    HDRSupport = table.Column<bool>(type: "bit", nullable: true),
                    ConsoleConnectivity = table.Column<int>(type: "int", nullable: true),
                    HasEthernet = table.Column<bool>(type: "bit", nullable: true),
                    ConsoleWifiVersion = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ConsoleBluetoothVersion = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CoolerType = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CpuCoolerFanSize = table.Column<int>(type: "int", nullable: true),
                    RadiatorSize = table.Column<int>(type: "int", nullable: true),
                    CpuCoolerHeight = table.Column<float>(type: "real", nullable: true),
                    TdpSupport = table.Column<int>(type: "int", nullable: true),
                    CpuCoolerNoiseLevel = table.Column<int>(type: "int", nullable: true),
                    CpuCoolerColor = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    LedLighting = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CpuSocketId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    CoreCount = table.Column<int>(type: "int", nullable: true),
                    ThreadCount = table.Column<int>(type: "int", nullable: true),
                    BaseClock = table.Column<float>(type: "real", nullable: true),
                    CpuBoostClock = table.Column<float>(type: "real", nullable: true),
                    L3Cache = table.Column<int>(type: "int", nullable: true),
                    Tdp = table.Column<int>(type: "int", nullable: true),
                    HasIntegratedGraphics = table.Column<bool>(type: "bit", nullable: true),
                    DeskWidth = table.Column<int>(type: "int", nullable: true),
                    Depth = table.Column<int>(type: "int", nullable: true),
                    DeskHeight = table.Column<float>(type: "real", nullable: true),
                    DeskMaterial = table.Column<int>(type: "int", nullable: true),
                    DeskMaxWeight = table.Column<int>(type: "int", nullable: true),
                    IsHeightAdjustable = table.Column<bool>(type: "bit", nullable: true),
                    HasCableManagement = table.Column<bool>(type: "bit", nullable: true),
                    HasCupHolder = table.Column<bool>(type: "bit", nullable: true),
                    HasHeadphoneHook = table.Column<bool>(type: "bit", nullable: true),
                    DeskSurfaceType = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    DeskColor = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Chipset = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    MemorySize = table.Column<int>(type: "int", nullable: true),
                    MemoryType = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    GpuBoostClock = table.Column<float>(type: "real", nullable: true),
                    GpuInterface = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    RecommendedPsu = table.Column<int>(type: "int", nullable: true),
                    GpuLength = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    HandheldProcessor = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    HandheldGraphics = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    HandheldRAM = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    HandheldStorage = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    HandheldDisplay = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    HandheldBatteryLife = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    HandheldWeight = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    HandheldOperatingSystem = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    HandheldConnectivity = table.Column<int>(type: "int", nullable: true),
                    HandheldWifiVersion = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    HandheldBluetoothVersion = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    HeadsetConnectivity = table.Column<int>(type: "int", nullable: true),
                    HeadsetHasMicrophone = table.Column<bool>(type: "bit", nullable: true),
                    IsSurroundSound = table.Column<bool>(type: "bit", nullable: true),
                    IsNoiseCancelling = table.Column<bool>(type: "bit", nullable: true),
                    DriverSize = table.Column<int>(type: "int", nullable: true),
                    Frequency = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    HeadsetMicrophoneType = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Impedance = table.Column<int>(type: "int", nullable: true),
                    HeadsetSensitivity = table.Column<int>(type: "int", nullable: true),
                    HeadsetColor = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    HubType = table.Column<int>(type: "int", nullable: true),
                    HubPortCount = table.Column<int>(type: "int", nullable: true),
                    HubUsbAPorts = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    HubUsbCPorts = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    HdmiPorts = table.Column<int>(type: "int", nullable: true),
                    DisplayPorts = table.Column<int>(type: "int", nullable: true),
                    EthernetPort = table.Column<bool>(type: "bit", nullable: true),
                    SdCardReader = table.Column<bool>(type: "bit", nullable: true),
                    AudioJack = table.Column<bool>(type: "bit", nullable: true),
                    MaxDisplays = table.Column<int>(type: "int", nullable: true),
                    HubMaxResolution = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PowerDelivery = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    HubColor = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    KeyboardType = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    SwitchTypeId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    Layout = table.Column<int>(type: "int", nullable: true),
                    KeyboardConnectivity = table.Column<int>(type: "int", nullable: true),
                    Backlight = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CPU = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    LaptopRAM = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    LaptopStorage = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    LaptopDisplay = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    GraphicsCard = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    LaptopOperatingSystem = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    LaptopBatteryLife = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Weight = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    LaptopWarranty = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IsForGaming = table.Column<bool>(type: "bit", nullable: true),
                    MemoryCardCapacity = table.Column<int>(type: "int", nullable: true),
                    CardType = table.Column<int>(type: "int", nullable: true),
                    SpeedClass = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    MemoryCardReadSpeed = table.Column<int>(type: "int", nullable: true),
                    MemoryCardWriteSpeed = table.Column<int>(type: "int", nullable: true),
                    MemoryCardWarranty = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Waterproof = table.Column<bool>(type: "bit", nullable: true),
                    Shockproof = table.Column<bool>(type: "bit", nullable: true),
                    MicrophoneMicrophoneType = table.Column<int>(type: "int", nullable: true),
                    PolarPattern = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    MicrophoneFrequency = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    SampleRate = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    MicrophoneSensitivity = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    MicrophoneConnectivity = table.Column<int>(type: "int", nullable: true),
                    MicrophoneConnection = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    HasShockMount = table.Column<bool>(type: "bit", nullable: true),
                    HasPopFilter = table.Column<bool>(type: "bit", nullable: true),
                    MicrophoneHasRgb = table.Column<bool>(type: "bit", nullable: true),
                    MicrophoneColor = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    RefreshRate = table.Column<int>(type: "int", nullable: true),
                    MonitorResolution = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ScreenSize = table.Column<float>(type: "real", nullable: true),
                    ResponseTime = table.Column<int>(type: "int", nullable: true),
                    ResponseTimeMs = table.Column<int>(type: "int", nullable: true),
                    ColorGamut = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Brightness = table.Column<int>(type: "int", nullable: true),
                    VesaMount = table.Column<bool>(type: "bit", nullable: true),
                    PanelTypeId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    MotherboardSocketId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    ChipsetId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    MotherboardFormFactorId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    RamSlots = table.Column<int>(type: "int", nullable: true),
                    MaxRam = table.Column<int>(type: "int", nullable: true),
                    MotherboardRamTypeId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    M2Slots = table.Column<int>(type: "int", nullable: true),
                    SataPorts = table.Column<int>(type: "int", nullable: true),
                    HasWifi = table.Column<bool>(type: "bit", nullable: true),
                    MousePadWidth = table.Column<int>(type: "int", nullable: true),
                    MousePadHeight = table.Column<int>(type: "int", nullable: true),
                    Thickness = table.Column<float>(type: "real", nullable: true),
                    MousePadMaterial = table.Column<int>(type: "int", nullable: true),
                    MousePadSurfaceType = table.Column<int>(type: "int", nullable: true),
                    MousePadBaseType = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    MousePadHasRgb = table.Column<bool>(type: "bit", nullable: true),
                    IsWashable = table.Column<bool>(type: "bit", nullable: true),
                    PadColor = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Dpi = table.Column<int>(type: "int", nullable: true),
                    ButtonCount = table.Column<int>(type: "int", nullable: true),
                    PollingRate = table.Column<int>(type: "int", nullable: true),
                    SensorType = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    MouseWeight = table.Column<int>(type: "int", nullable: true),
                    MouseConnectivity = table.Column<int>(type: "int", nullable: true),
                    MouseColor = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    BacklightColor = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    DeviceType = table.Column<int>(type: "int", nullable: true),
                    WifiStandard = table.Column<int>(type: "int", nullable: true),
                    MaxSpeed = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    NetworkFrequency = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    EthernetPorts = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    AntennaCount = table.Column<int>(type: "int", nullable: true),
                    HasUsb = table.Column<bool>(type: "bit", nullable: true),
                    SecurityProtocol = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Coverage = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PowerBankCapacity = table.Column<int>(type: "int", nullable: true),
                    PowerBankTotalWattage = table.Column<int>(type: "int", nullable: true),
                    PowerBankPortCount = table.Column<int>(type: "int", nullable: true),
                    PowerBankUsbCPorts = table.Column<int>(type: "int", nullable: true),
                    PowerBankUsbAPorts = table.Column<int>(type: "int", nullable: true),
                    PowerBankInputPorts = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PowerBankMaxOutputPerPort = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PowerBankFastChargingProtocols = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    RechargingTime = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    HasDisplay = table.Column<bool>(type: "bit", nullable: true),
                    PowerBankWeight = table.Column<int>(type: "int", nullable: true),
                    Color = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Wattage = table.Column<int>(type: "int", nullable: true),
                    EfficiencyRating = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Modularity = table.Column<int>(type: "int", nullable: true),
                    PsuFormFactorId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    RamRamTypeId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    RamCapacity = table.Column<int>(type: "int", nullable: true),
                    Speed = table.Column<int>(type: "int", nullable: true),
                    ModuleCount = table.Column<int>(type: "int", nullable: true),
                    Timing = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Voltage = table.Column<float>(type: "real", nullable: true),
                    RamHasRgb = table.Column<bool>(type: "bit", nullable: true),
                    RamFormFactor = table.Column<int>(type: "int", nullable: true),
                    SoftwareType = table.Column<int>(type: "int", nullable: true),
                    LicenseType = table.Column<int>(type: "int", nullable: true),
                    Platform = table.Column<int>(type: "int", nullable: true),
                    Version = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Language = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    DeliveryMethod = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    SystemRequirements = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IsSubscription = table.Column<bool>(type: "bit", nullable: true),
                    SpeakerType = table.Column<int>(type: "int", nullable: true),
                    SpeakerTotalWattage = table.Column<int>(type: "int", nullable: true),
                    SpeakerFrequency = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    SpeakerConnectivity = table.Column<int>(type: "int", nullable: true),
                    SpeakerInputPorts = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    HasBluetooth = table.Column<bool>(type: "bit", nullable: true),
                    HasRemote = table.Column<bool>(type: "bit", nullable: true),
                    SpeakerColor = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    StorageType = table.Column<int>(type: "int", nullable: true),
                    StorageInterface = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    StorageCapacity = table.Column<int>(type: "int", nullable: true),
                    StorageReadSpeed = table.Column<int>(type: "int", nullable: true),
                    StorageWriteSpeed = table.Column<int>(type: "int", nullable: true),
                    StorageFormFactor = table.Column<int>(type: "int", nullable: true),
                    Rpm = table.Column<int>(type: "int", nullable: true),
                    WebcamResolution = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    FrameRate = table.Column<int>(type: "int", nullable: true),
                    FocusType = table.Column<int>(type: "int", nullable: true),
                    FieldOfView = table.Column<int>(type: "int", nullable: true),
                    WebcamConnectivity = table.Column<int>(type: "int", nullable: true),
                    Webcam_Connection = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    WebcamHasMicrophone = table.Column<bool>(type: "bit", nullable: true),
                    HasPrivacyShutter = table.Column<bool>(type: "bit", nullable: true),
                    MountType = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    WebcamColor = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AppSpecifications", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AppSpecifications_AppChipsets_ChipsetId",
                        column: x => x.ChipsetId,
                        principalTable: "AppChipsets",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_AppSpecifications_AppFormFactors_CaseFormFactorId",
                        column: x => x.CaseFormFactorId,
                        principalTable: "AppFormFactors",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_AppSpecifications_AppFormFactors_MotherboardFormFactorId",
                        column: x => x.MotherboardFormFactorId,
                        principalTable: "AppFormFactors",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_AppSpecifications_AppFormFactors_PsuFormFactorId",
                        column: x => x.PsuFormFactorId,
                        principalTable: "AppFormFactors",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_AppSpecifications_AppPanelTypes_PanelTypeId",
                        column: x => x.PanelTypeId,
                        principalTable: "AppPanelTypes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_AppSpecifications_AppProducts_ProductId",
                        column: x => x.ProductId,
                        principalTable: "AppProducts",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_AppSpecifications_AppRamTypes_MotherboardRamTypeId",
                        column: x => x.MotherboardRamTypeId,
                        principalTable: "AppRamTypes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_AppSpecifications_AppRamTypes_RamRamTypeId",
                        column: x => x.RamRamTypeId,
                        principalTable: "AppRamTypes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_AppSpecifications_AppSockets_CpuSocketId",
                        column: x => x.CpuSocketId,
                        principalTable: "AppSockets",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_AppSpecifications_AppSockets_MotherboardSocketId",
                        column: x => x.MotherboardSocketId,
                        principalTable: "AppSockets",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_AppSpecifications_AppSwitchTypes_SwitchTypeId",
                        column: x => x.SwitchTypeId,
                        principalTable: "AppSwitchTypes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "AppStoreInventories",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    StoreId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ProductId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Quantity = table.Column<int>(type: "int", nullable: false),
                    ReorderLevel = table.Column<int>(type: "int", nullable: false, defaultValue: 10),
                    ReorderQuantity = table.Column<int>(type: "int", nullable: false, defaultValue: 50),
                    IsAvailableForSale = table.Column<bool>(type: "bit", nullable: false, defaultValue: true),
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
                    table.PrimaryKey("PK_AppStoreInventories", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AppStoreInventories_AppProducts_ProductId",
                        column: x => x.ProductId,
                        principalTable: "AppProducts",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_AppStoreInventories_AppStores_StoreId",
                        column: x => x.StoreId,
                        principalTable: "AppStores",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "OpenIddictTokens",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ApplicationId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    AuthorizationId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    CreationDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ExpirationDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Payload = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Properties = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    RedemptionDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ReferenceId = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    Status = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    Subject = table.Column<string>(type: "nvarchar(400)", maxLength: 400, nullable: true),
                    Type = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: true),
                    ExtraProperties = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ConcurrencyStamp = table.Column<string>(type: "nvarchar(40)", maxLength: 40, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OpenIddictTokens", x => x.Id);
                    table.ForeignKey(
                        name: "FK_OpenIddictTokens_OpenIddictApplications_ApplicationId",
                        column: x => x.ApplicationId,
                        principalTable: "OpenIddictApplications",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_OpenIddictTokens_OpenIddictAuthorizations_AuthorizationId",
                        column: x => x.AuthorizationId,
                        principalTable: "OpenIddictAuthorizations",
                        principalColumn: "Id");
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
                        name: "FK_AppCaseMaterials_AppMaterials_MaterialId",
                        column: x => x.MaterialId,
                        principalTable: "AppMaterials",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_AppCaseMaterials_AppSpecifications_CaseSpecificationId",
                        column: x => x.CaseSpecificationId,
                        principalTable: "AppSpecifications",
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
                        name: "FK_AppCpuCoolerSocketSupports_AppSockets_SocketId",
                        column: x => x.SocketId,
                        principalTable: "AppSockets",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_AppCpuCoolerSocketSupports_AppSpecifications_CpuCoolerSpecificationId",
                        column: x => x.CpuCoolerSpecificationId,
                        principalTable: "AppSpecifications",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_AbpAuditLogActions_AuditLogId",
                table: "AbpAuditLogActions",
                column: "AuditLogId");

            migrationBuilder.CreateIndex(
                name: "IX_AbpAuditLogActions_TenantId_ServiceName_MethodName_ExecutionTime",
                table: "AbpAuditLogActions",
                columns: new[] { "TenantId", "ServiceName", "MethodName", "ExecutionTime" });

            migrationBuilder.CreateIndex(
                name: "IX_AbpAuditLogs_TenantId_ExecutionTime",
                table: "AbpAuditLogs",
                columns: new[] { "TenantId", "ExecutionTime" });

            migrationBuilder.CreateIndex(
                name: "IX_AbpAuditLogs_TenantId_UserId_ExecutionTime",
                table: "AbpAuditLogs",
                columns: new[] { "TenantId", "UserId", "ExecutionTime" });

            migrationBuilder.CreateIndex(
                name: "IX_AbpBackgroundJobs_IsAbandoned_NextTryTime",
                table: "AbpBackgroundJobs",
                columns: new[] { "IsAbandoned", "NextTryTime" });

            migrationBuilder.CreateIndex(
                name: "IX_AbpBlobContainers_TenantId_Name",
                table: "AbpBlobContainers",
                columns: new[] { "TenantId", "Name" });

            migrationBuilder.CreateIndex(
                name: "IX_AbpBlobs_ContainerId",
                table: "AbpBlobs",
                column: "ContainerId");

            migrationBuilder.CreateIndex(
                name: "IX_AbpBlobs_TenantId_ContainerId_Name",
                table: "AbpBlobs",
                columns: new[] { "TenantId", "ContainerId", "Name" });

            migrationBuilder.CreateIndex(
                name: "IX_AbpEntityChanges_AuditLogId",
                table: "AbpEntityChanges",
                column: "AuditLogId");

            migrationBuilder.CreateIndex(
                name: "IX_AbpEntityChanges_TenantId_EntityTypeFullName_EntityId",
                table: "AbpEntityChanges",
                columns: new[] { "TenantId", "EntityTypeFullName", "EntityId" });

            migrationBuilder.CreateIndex(
                name: "IX_AbpEntityPropertyChanges_EntityChangeId",
                table: "AbpEntityPropertyChanges",
                column: "EntityChangeId");

            migrationBuilder.CreateIndex(
                name: "IX_AbpFeatureGroups_Name",
                table: "AbpFeatureGroups",
                column: "Name",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_AbpFeatures_GroupName",
                table: "AbpFeatures",
                column: "GroupName");

            migrationBuilder.CreateIndex(
                name: "IX_AbpFeatures_Name",
                table: "AbpFeatures",
                column: "Name",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_AbpFeatureValues_Name_ProviderName_ProviderKey",
                table: "AbpFeatureValues",
                columns: new[] { "Name", "ProviderName", "ProviderKey" },
                unique: true,
                filter: "[ProviderName] IS NOT NULL AND [ProviderKey] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_AbpLinkUsers_SourceUserId_SourceTenantId_TargetUserId_TargetTenantId",
                table: "AbpLinkUsers",
                columns: new[] { "SourceUserId", "SourceTenantId", "TargetUserId", "TargetTenantId" },
                unique: true,
                filter: "[SourceTenantId] IS NOT NULL AND [TargetTenantId] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_AbpOrganizationUnitRoles_RoleId_OrganizationUnitId",
                table: "AbpOrganizationUnitRoles",
                columns: new[] { "RoleId", "OrganizationUnitId" });

            migrationBuilder.CreateIndex(
                name: "IX_AbpOrganizationUnits_Code",
                table: "AbpOrganizationUnits",
                column: "Code");

            migrationBuilder.CreateIndex(
                name: "IX_AbpOrganizationUnits_ParentId",
                table: "AbpOrganizationUnits",
                column: "ParentId");

            migrationBuilder.CreateIndex(
                name: "IX_AbpPermissionGrants_TenantId_Name_ProviderName_ProviderKey",
                table: "AbpPermissionGrants",
                columns: new[] { "TenantId", "Name", "ProviderName", "ProviderKey" },
                unique: true,
                filter: "[TenantId] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_AbpPermissionGroups_Name",
                table: "AbpPermissionGroups",
                column: "Name",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_AbpPermissions_GroupName",
                table: "AbpPermissions",
                column: "GroupName");

            migrationBuilder.CreateIndex(
                name: "IX_AbpPermissions_Name",
                table: "AbpPermissions",
                column: "Name",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_AbpRoleClaims_RoleId",
                table: "AbpRoleClaims",
                column: "RoleId");

            migrationBuilder.CreateIndex(
                name: "IX_AbpRoles_NormalizedName",
                table: "AbpRoles",
                column: "NormalizedName");

            migrationBuilder.CreateIndex(
                name: "IX_AbpSecurityLogs_TenantId_Action",
                table: "AbpSecurityLogs",
                columns: new[] { "TenantId", "Action" });

            migrationBuilder.CreateIndex(
                name: "IX_AbpSecurityLogs_TenantId_ApplicationName",
                table: "AbpSecurityLogs",
                columns: new[] { "TenantId", "ApplicationName" });

            migrationBuilder.CreateIndex(
                name: "IX_AbpSecurityLogs_TenantId_Identity",
                table: "AbpSecurityLogs",
                columns: new[] { "TenantId", "Identity" });

            migrationBuilder.CreateIndex(
                name: "IX_AbpSecurityLogs_TenantId_UserId",
                table: "AbpSecurityLogs",
                columns: new[] { "TenantId", "UserId" });

            migrationBuilder.CreateIndex(
                name: "IX_AbpSessions_Device",
                table: "AbpSessions",
                column: "Device");

            migrationBuilder.CreateIndex(
                name: "IX_AbpSessions_SessionId",
                table: "AbpSessions",
                column: "SessionId");

            migrationBuilder.CreateIndex(
                name: "IX_AbpSessions_TenantId_UserId",
                table: "AbpSessions",
                columns: new[] { "TenantId", "UserId" });

            migrationBuilder.CreateIndex(
                name: "IX_AbpSettingDefinitions_Name",
                table: "AbpSettingDefinitions",
                column: "Name",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_AbpSettings_Name_ProviderName_ProviderKey",
                table: "AbpSettings",
                columns: new[] { "Name", "ProviderName", "ProviderKey" },
                unique: true,
                filter: "[ProviderName] IS NOT NULL AND [ProviderKey] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_AbpTenants_Name",
                table: "AbpTenants",
                column: "Name");

            migrationBuilder.CreateIndex(
                name: "IX_AbpTenants_NormalizedName",
                table: "AbpTenants",
                column: "NormalizedName");

            migrationBuilder.CreateIndex(
                name: "IX_AbpUserClaims_UserId",
                table: "AbpUserClaims",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_AbpUserLogins_LoginProvider_ProviderKey",
                table: "AbpUserLogins",
                columns: new[] { "LoginProvider", "ProviderKey" });

            migrationBuilder.CreateIndex(
                name: "IX_AbpUserOrganizationUnits_UserId_OrganizationUnitId",
                table: "AbpUserOrganizationUnits",
                columns: new[] { "UserId", "OrganizationUnitId" });

            migrationBuilder.CreateIndex(
                name: "IX_AbpUserRoles_RoleId_UserId",
                table: "AbpUserRoles",
                columns: new[] { "RoleId", "UserId" });

            migrationBuilder.CreateIndex(
                name: "IX_AbpUsers_Email",
                table: "AbpUsers",
                column: "Email");

            migrationBuilder.CreateIndex(
                name: "IX_AbpUsers_NormalizedEmail",
                table: "AbpUsers",
                column: "NormalizedEmail");

            migrationBuilder.CreateIndex(
                name: "IX_AbpUsers_NormalizedUserName",
                table: "AbpUsers",
                column: "NormalizedUserName");

            migrationBuilder.CreateIndex(
                name: "IX_AbpUsers_UserName",
                table: "AbpUsers",
                column: "UserName");

            migrationBuilder.CreateIndex(
                name: "IX_AppCartItems_CartId",
                table: "AppCartItems",
                column: "CartId");

            migrationBuilder.CreateIndex(
                name: "IX_AppCartItems_ProductId",
                table: "AppCartItems",
                column: "ProductId");

            migrationBuilder.CreateIndex(
                name: "IX_AppCarts_UserId",
                table: "AppCarts",
                column: "UserId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_AppCaseMaterials_MaterialId",
                table: "AppCaseMaterials",
                column: "MaterialId");

            migrationBuilder.CreateIndex(
                name: "IX_AppCategories_UrlSlug",
                table: "AppCategories",
                column: "UrlSlug",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_AppCpuCoolerSocketSupports_SocketId",
                table: "AppCpuCoolerSocketSupports",
                column: "SocketId");

            migrationBuilder.CreateIndex(
                name: "IX_AppCustomers_AppUserId",
                table: "AppCustomers",
                column: "AppUserId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_AppManufacturers_UrlSlug",
                table: "AppManufacturers",
                column: "UrlSlug",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_AppOrderHistories_CreationTime",
                table: "AppOrderHistories",
                column: "CreationTime");

            migrationBuilder.CreateIndex(
                name: "IX_AppOrderHistories_OrderId",
                table: "AppOrderHistories",
                column: "OrderId");

            migrationBuilder.CreateIndex(
                name: "IX_AppOrderItems_OrderId",
                table: "AppOrderItems",
                column: "OrderId");

            migrationBuilder.CreateIndex(
                name: "IX_AppOrders_CreationTime",
                table: "AppOrders",
                column: "CreationTime");

            migrationBuilder.CreateIndex(
                name: "IX_AppOrders_CustomerId",
                table: "AppOrders",
                column: "CustomerId");

            migrationBuilder.CreateIndex(
                name: "IX_AppOrders_OrderNumber",
                table: "AppOrders",
                column: "OrderNumber",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_AppOrders_OrderStatus",
                table: "AppOrders",
                column: "OrderStatus");

            migrationBuilder.CreateIndex(
                name: "IX_AppOrders_OrderType",
                table: "AppOrders",
                column: "OrderType");

            migrationBuilder.CreateIndex(
                name: "IX_AppOrders_PaymentStatus",
                table: "AppOrders",
                column: "PaymentStatus");

            migrationBuilder.CreateIndex(
                name: "IX_AppOrders_StoreId",
                table: "AppOrders",
                column: "StoreId");

            migrationBuilder.CreateIndex(
                name: "IX_AppProducts_CategoryId",
                table: "AppProducts",
                column: "CategoryId");

            migrationBuilder.CreateIndex(
                name: "IX_AppProducts_ManufacturerId",
                table: "AppProducts",
                column: "ManufacturerId");

            migrationBuilder.CreateIndex(
                name: "IX_AppProducts_UrlSlug",
                table: "AppProducts",
                column: "UrlSlug",
                unique: true);

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

            migrationBuilder.CreateIndex(
                name: "IX_AppSpecifications_CaseFormFactorId",
                table: "AppSpecifications",
                column: "CaseFormFactorId");

            migrationBuilder.CreateIndex(
                name: "IX_AppSpecifications_ChipsetId",
                table: "AppSpecifications",
                column: "ChipsetId");

            migrationBuilder.CreateIndex(
                name: "IX_AppSpecifications_CpuSocketId",
                table: "AppSpecifications",
                column: "CpuSocketId");

            migrationBuilder.CreateIndex(
                name: "IX_AppSpecifications_MotherboardFormFactorId",
                table: "AppSpecifications",
                column: "MotherboardFormFactorId");

            migrationBuilder.CreateIndex(
                name: "IX_AppSpecifications_MotherboardRamTypeId",
                table: "AppSpecifications",
                column: "MotherboardRamTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_AppSpecifications_MotherboardSocketId",
                table: "AppSpecifications",
                column: "MotherboardSocketId");

            migrationBuilder.CreateIndex(
                name: "IX_AppSpecifications_PanelTypeId",
                table: "AppSpecifications",
                column: "PanelTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_AppSpecifications_ProductId",
                table: "AppSpecifications",
                column: "ProductId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_AppSpecifications_PsuFormFactorId",
                table: "AppSpecifications",
                column: "PsuFormFactorId");

            migrationBuilder.CreateIndex(
                name: "IX_AppSpecifications_RamRamTypeId",
                table: "AppSpecifications",
                column: "RamRamTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_AppSpecifications_SwitchTypeId",
                table: "AppSpecifications",
                column: "SwitchTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_AppStoreInventories_ProductId",
                table: "AppStoreInventories",
                column: "ProductId");

            migrationBuilder.CreateIndex(
                name: "IX_AppStoreInventories_Quantity",
                table: "AppStoreInventories",
                column: "Quantity");

            migrationBuilder.CreateIndex(
                name: "IX_AppStoreInventories_StoreId",
                table: "AppStoreInventories",
                column: "StoreId");

            migrationBuilder.CreateIndex(
                name: "IX_AppStoreInventories_StoreId_ProductId",
                table: "AppStoreInventories",
                columns: new[] { "StoreId", "ProductId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_AppStores_Code",
                table: "AppStores",
                column: "Code",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_OpenIddictApplications_ClientId",
                table: "OpenIddictApplications",
                column: "ClientId");

            migrationBuilder.CreateIndex(
                name: "IX_OpenIddictAuthorizations_ApplicationId_Status_Subject_Type",
                table: "OpenIddictAuthorizations",
                columns: new[] { "ApplicationId", "Status", "Subject", "Type" });

            migrationBuilder.CreateIndex(
                name: "IX_OpenIddictScopes_Name",
                table: "OpenIddictScopes",
                column: "Name");

            migrationBuilder.CreateIndex(
                name: "IX_OpenIddictTokens_ApplicationId_Status_Subject_Type",
                table: "OpenIddictTokens",
                columns: new[] { "ApplicationId", "Status", "Subject", "Type" });

            migrationBuilder.CreateIndex(
                name: "IX_OpenIddictTokens_AuthorizationId",
                table: "OpenIddictTokens",
                column: "AuthorizationId");

            migrationBuilder.CreateIndex(
                name: "IX_OpenIddictTokens_ReferenceId",
                table: "OpenIddictTokens",
                column: "ReferenceId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AbpAuditLogActions");

            migrationBuilder.DropTable(
                name: "AbpAuditLogExcelFiles");

            migrationBuilder.DropTable(
                name: "AbpBackgroundJobs");

            migrationBuilder.DropTable(
                name: "AbpBlobs");

            migrationBuilder.DropTable(
                name: "AbpClaimTypes");

            migrationBuilder.DropTable(
                name: "AbpEntityPropertyChanges");

            migrationBuilder.DropTable(
                name: "AbpFeatureGroups");

            migrationBuilder.DropTable(
                name: "AbpFeatures");

            migrationBuilder.DropTable(
                name: "AbpFeatureValues");

            migrationBuilder.DropTable(
                name: "AbpLinkUsers");

            migrationBuilder.DropTable(
                name: "AbpOrganizationUnitRoles");

            migrationBuilder.DropTable(
                name: "AbpPermissionGrants");

            migrationBuilder.DropTable(
                name: "AbpPermissionGroups");

            migrationBuilder.DropTable(
                name: "AbpPermissions");

            migrationBuilder.DropTable(
                name: "AbpRoleClaims");

            migrationBuilder.DropTable(
                name: "AbpSecurityLogs");

            migrationBuilder.DropTable(
                name: "AbpSessions");

            migrationBuilder.DropTable(
                name: "AbpSettingDefinitions");

            migrationBuilder.DropTable(
                name: "AbpSettings");

            migrationBuilder.DropTable(
                name: "AbpTenantConnectionStrings");

            migrationBuilder.DropTable(
                name: "AbpUserClaims");

            migrationBuilder.DropTable(
                name: "AbpUserDelegations");

            migrationBuilder.DropTable(
                name: "AbpUserLogins");

            migrationBuilder.DropTable(
                name: "AbpUserOrganizationUnits");

            migrationBuilder.DropTable(
                name: "AbpUserRoles");

            migrationBuilder.DropTable(
                name: "AbpUserTokens");

            migrationBuilder.DropTable(
                name: "AppCartItems");

            migrationBuilder.DropTable(
                name: "AppCaseMaterials");

            migrationBuilder.DropTable(
                name: "AppCpuCoolerSocketSupports");

            migrationBuilder.DropTable(
                name: "AppCustomers");

            migrationBuilder.DropTable(
                name: "AppOrderHistories");

            migrationBuilder.DropTable(
                name: "AppOrderItems");

            migrationBuilder.DropTable(
                name: "AppRecentlyViewedProducts");

            migrationBuilder.DropTable(
                name: "AppStoreInventories");

            migrationBuilder.DropTable(
                name: "OpenIddictScopes");

            migrationBuilder.DropTable(
                name: "OpenIddictTokens");

            migrationBuilder.DropTable(
                name: "AbpBlobContainers");

            migrationBuilder.DropTable(
                name: "AbpEntityChanges");

            migrationBuilder.DropTable(
                name: "AbpTenants");

            migrationBuilder.DropTable(
                name: "AbpOrganizationUnits");

            migrationBuilder.DropTable(
                name: "AbpRoles");

            migrationBuilder.DropTable(
                name: "AppCarts");

            migrationBuilder.DropTable(
                name: "AppMaterials");

            migrationBuilder.DropTable(
                name: "AppSpecifications");

            migrationBuilder.DropTable(
                name: "AbpUsers");

            migrationBuilder.DropTable(
                name: "AppOrders");

            migrationBuilder.DropTable(
                name: "AppStores");

            migrationBuilder.DropTable(
                name: "OpenIddictAuthorizations");

            migrationBuilder.DropTable(
                name: "AbpAuditLogs");

            migrationBuilder.DropTable(
                name: "AppChipsets");

            migrationBuilder.DropTable(
                name: "AppFormFactors");

            migrationBuilder.DropTable(
                name: "AppPanelTypes");

            migrationBuilder.DropTable(
                name: "AppProducts");

            migrationBuilder.DropTable(
                name: "AppRamTypes");

            migrationBuilder.DropTable(
                name: "AppSockets");

            migrationBuilder.DropTable(
                name: "AppSwitchTypes");

            migrationBuilder.DropTable(
                name: "OpenIddictApplications");

            migrationBuilder.DropTable(
                name: "AppCategories");

            migrationBuilder.DropTable(
                name: "AppManufacturers");
        }
    }
}
