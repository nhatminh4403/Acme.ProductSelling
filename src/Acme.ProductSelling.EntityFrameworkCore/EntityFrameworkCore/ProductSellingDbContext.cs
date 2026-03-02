using Acme.ProductSelling.Carts;
using Acme.ProductSelling.Categories;
using Acme.ProductSelling.Manufacturers;
using Acme.ProductSelling.Orders;
using Acme.ProductSelling.Products;
using Acme.ProductSelling.Products.Lookups;
using Acme.ProductSelling.Specifications;
using Acme.ProductSelling.Specifications.Junctions;
using Acme.ProductSelling.Specifications.Models;
using Acme.ProductSelling.StoreInventories;
using Acme.ProductSelling.Stores;
using Acme.ProductSelling.Users;
using Microsoft.EntityFrameworkCore;
using System;
using Volo.Abp.AuditLogging.EntityFrameworkCore;
using Volo.Abp.BackgroundJobs.EntityFrameworkCore;
using Volo.Abp.BlobStoring.Database.EntityFrameworkCore;
using Volo.Abp.Data;
using Volo.Abp.DependencyInjection;
using Volo.Abp.EntityFrameworkCore;
using Volo.Abp.EntityFrameworkCore.Modeling;
using Volo.Abp.FeatureManagement.EntityFrameworkCore;
using Volo.Abp.Identity;
using Volo.Abp.Identity.EntityFrameworkCore;
using Volo.Abp.OpenIddict.EntityFrameworkCore;
using Volo.Abp.PermissionManagement.EntityFrameworkCore;
using Volo.Abp.SettingManagement.EntityFrameworkCore;
using Volo.Abp.TenantManagement;
using Volo.Abp.TenantManagement.EntityFrameworkCore;

namespace Acme.ProductSelling.EntityFrameworkCore;

[ReplaceDbContext(typeof(IIdentityDbContext))]
[ReplaceDbContext(typeof(ITenantManagementDbContext))]
[ConnectionStringName("Default")]
public class ProductSellingDbContext :
    AbpDbContext<ProductSellingDbContext>,
    ITenantManagementDbContext,
    IIdentityDbContext
{
    /* Add DbSet properties for your Aggregate Roots / Entities here. */

    #region Entities from the modules

    // Identity
    public DbSet<IdentityUser> Users { get; set; }
    public DbSet<IdentityRole> Roles { get; set; }
    public DbSet<IdentityClaimType> ClaimTypes { get; set; }
    public DbSet<OrganizationUnit> OrganizationUnits { get; set; }
    public DbSet<IdentitySecurityLog> SecurityLogs { get; set; }
    public DbSet<IdentityLinkUser> LinkUsers { get; set; }
    public DbSet<IdentityUserDelegation> UserDelegations { get; set; }
    public DbSet<IdentitySession> Sessions { get; set; }

    // Tenant Management
    public DbSet<Tenant> Tenants { get; set; }
    public DbSet<TenantConnectionString> TenantConnectionStrings { get; set; }

    //Domain Models Management
    public DbSet<Category> Categories { get; set; }
    public DbSet<Product> Products { get; set; }
    public DbSet<Store> Stores { get; set; }
    public DbSet<StoreInventory> StoreInventories { get; set; }
    public DbSet<Manufacturer> Manufacturers { get; set; }
    public DbSet<Order> Orders { get; set; }
    public DbSet<OnlineOrder> OnlineOrders { get; set; }
    public DbSet<InStoreOrder> InStoreOrders { get; set; }
    public DbSet<OrderItem> OrderItems { get; set; }
    public DbSet<OrderHistory> OrderHistories { get; set; }
    public DbSet<Cart> Carts { get; set; }
    public DbSet<CartItem> CartItems { get; set; }
    //public DbSet<Comment> Comments { get; set; }
    //public DbSet<Blog> Blogs { get; set; }
    //public DbSet<Likes> Likes { get; set; }
    public DbSet<Customer> Customers { get; set; }
    public DbSet<RecentlyViewedProduct> RecentlyViewedProducts { get; set; }
    #endregion

    #region Specs

    public DbSet<SpecificationBase> Specifications { get; set; }
    #endregion

    #region Lookups and Junctions
    public DbSet<CpuSocket> Sockets { get; set; }
    public DbSet<Chipset> Chipsets { get; set; }
    public DbSet<FormFactor> FormFactors { get; set; }
    public DbSet<Material> Materials { get; set; }
    public DbSet<PanelType> PanelTypes { get; set; }
    public DbSet<RamType> RamTypes { get; set; }
    public DbSet<SwitchType> SwitchTypes { get; set; }

    public DbSet<CaseMaterial> CaseMaterials { get; set; }
    public DbSet<CpuCoolerSocketSupport> CpuCoolerSocketSupports { get; set; }
    #endregion

    #region Chatbot
    //public DbSet<ChatbotMessage> ChatbotMessages { get; set; }
    //public DbSet<ChatbotTrainingData> ChatbotTrainingData { get; set; }
    #endregion

    public ProductSellingDbContext(DbContextOptions<ProductSellingDbContext> options)
        : base(options)
    {
    }

    const string tablePrefix = "App";

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        /* Include modules to your migration db context */
        #region Modules
        builder.ConfigurePermissionManagement();
        builder.ConfigureSettingManagement();
        builder.ConfigureBackgroundJobs();
        builder.ConfigureAuditLogging();
        builder.ConfigureFeatureManagement();
        builder.ConfigureIdentity();
        builder.ConfigureOpenIddict();
        builder.ConfigureTenantManagement();
        builder.ConfigureBlobStoring();
        #endregion

        /* Configure your own tables/entities inside here */

        #region Core Entities
        builder.Entity<Category>(b =>
        {
            b.ToTable(tablePrefix + "Categories");
            b.ConfigureByConvention();
            b.Property(c => c.Name).IsRequired().HasMaxLength(100);
            b.HasIndex(p => p.UrlSlug).IsUnique();
        });

        builder.Entity<Manufacturer>(b =>
        {
            b.ToTable(tablePrefix + "Manufacturers");
            b.ConfigureByConvention();
            b.Property(c => c.Name).IsRequired().HasMaxLength(100);
            b.HasMany(c => c.Products).WithOne(p => p.Manufacturer).HasForeignKey(p => p.ManufacturerId);
            b.HasIndex(p => p.UrlSlug).IsUnique();
        });

        builder.Entity<Product>(b =>
        {
            b.ToTable(tablePrefix + "Products");
            b.Property(p => p.ProductName).IsRequired().HasMaxLength(100);
            b.Property(p => p.OriginalPrice).HasColumnType("decimal(18,2)");
            b.Property(p => p.DiscountedPrice).HasColumnType("decimal(18,2)").IsRequired(false);
            b.Property(p => p.DiscountPercent).IsRequired(true);

            b.HasOne(p => p.Category).WithMany(c => c.Products).HasForeignKey(p => p.CategoryId).OnDelete(DeleteBehavior.Restrict);
            b.HasOne(p => p.Manufacturer).WithMany(m => m.Products).HasForeignKey(p => p.ManufacturerId).OnDelete(DeleteBehavior.Restrict);
            b.HasIndex(p => p.UrlSlug).IsUnique();
        });

        builder.Entity<RecentlyViewedProduct>(b =>
        {
            b.ToTable(tablePrefix + "RecentlyViewedProducts", ProductSellingConsts.DbSchema);
            b.ConfigureByConvention();
            // Index for fast lookups
            b.HasKey(x => x.Id);

            b.Property(x => x.UserId)
                .IsRequired();

            b.Property(x => x.ProductId)
                .IsRequired();

            b.Property(x => x.ViewedAt)
                .IsRequired();

            // Indexes
            b.HasIndex(x => new { x.UserId, x.ViewedAt })
                .IsDescending(false, true)
                .HasDatabaseName("IX_RecentlyViewed_UserId_ViewedAt");

            b.HasIndex(x => new { x.UserId, x.ProductId })
                .IsUnique()
                .HasDatabaseName("IX_RecentlyViewed_UserId_ProductId");

            b.HasIndex(x => x.ViewedAt)
                .HasDatabaseName("IX_RecentlyViewed_ViewedAt");

            // Relationships
            b.HasOne(x => x.Product)
                .WithMany()
                .HasForeignKey(x => x.ProductId)
                .OnDelete(DeleteBehavior.Cascade);
            b.HasQueryFilter(x => !x.Product.IsDeleted);

        });

        builder.Entity<StoreInventory>(b =>
        {
            b.ToTable(tablePrefix + "StoreInventories");
            b.ConfigureByConvention();

            b.Property(si => si.Quantity).IsRequired();
            b.Property(si => si.ReorderLevel).IsRequired().HasDefaultValue(10);
            b.Property(si => si.ReorderQuantity).IsRequired().HasDefaultValue(50);
            b.Property(si => si.IsAvailableForSale).IsRequired().HasDefaultValue(true);

            // Relationships
            b.HasOne(si => si.Store)
                .WithMany()
                .HasForeignKey(si => si.StoreId)
                .OnDelete(DeleteBehavior.Restrict);

            b.HasOne(si => si.Product)
                .WithMany(p => p.StoreInventories)
                .HasForeignKey(si => si.ProductId)
                .OnDelete(DeleteBehavior.Cascade);

            // Indexes
            b.HasIndex(si => new { si.StoreId, si.ProductId }).IsUnique();
            b.HasIndex(si => si.StoreId);
            b.HasIndex(si => si.ProductId);
            b.HasIndex(si => si.Quantity);
        });

        builder.Entity<Store>(b =>
        {
            b.ToTable(tablePrefix + "Stores");
            b.ConfigureFullAuditedAggregateRoot();
            b.ConfigureByConvention();
            b.Property(s => s.Name).IsRequired().HasMaxLength(100);
            b.Property(s => s.Code).IsRequired().HasMaxLength(20);
            b.Property(s => s.ManagerName).IsRequired();
            b.HasIndex(s => s.Code).IsUnique();

        });

        #endregion

        #region Lookup Tables
        builder.Entity<CpuSocket>(b =>
        {
            b.ToTable(tablePrefix + "Sockets");
            b.Property(s => s.Name).IsRequired().HasMaxLength(50);
        });

        builder.Entity<Chipset>(b =>
        {
            b.ToTable(tablePrefix + "Chipsets");
            b.Property(c => c.Name).IsRequired().HasMaxLength(50);
        });

        builder.Entity<FormFactor>(b =>
        {
            b.ToTable(tablePrefix + "FormFactors");
            b.Property(f => f.Name).IsRequired().HasMaxLength(50);
        });

        builder.Entity<Material>(b =>
        {
            b.ToTable(tablePrefix + "Materials");
            b.Property(m => m.Name).IsRequired().HasMaxLength(50);
        });

        builder.Entity<PanelType>(b =>
        {
            b.ToTable(tablePrefix + "PanelTypes");
            b.Property(p => p.Name).IsRequired().HasMaxLength(50);
        });

        builder.Entity<RamType>(b =>
        {
            b.ToTable(tablePrefix + "RamTypes");
            b.Property(r => r.Name).IsRequired().HasMaxLength(50);
        });

        builder.Entity<SwitchType>(b =>
        {
            b.ToTable(tablePrefix + "SwitchTypes");
            b.Property(s => s.Name).IsRequired().HasMaxLength(50);
        });
        #endregion
        builder.Entity<CaseMaterial>(b =>
        {
            b.ToTable(tablePrefix + "CaseMaterials");
            b.HasKey(cm => new { cm.CaseSpecificationId, cm.MaterialId });
            b.HasOne(cm => cm.Material).WithMany().HasForeignKey(cm => cm.MaterialId);
            b.HasQueryFilter(cm => !cm.CaseSpecification.Product.IsDeleted);
        });

        builder.Entity<CpuCoolerSocketSupport>(b =>
        {
            b.ToTable(tablePrefix + "CpuCoolerSocketSupports");
            b.HasKey(css => new { css.CpuCoolerSpecificationId, css.SocketId });
            b.HasOne(css => css.Socket).WithMany().HasForeignKey(css => css.SocketId);
            b.HasQueryFilter(css => !css.CpuCoolerSpecification.Product.IsDeleted);
        });
        #region Specifications

        builder.Entity<SpecificationBase>(b =>
        {
            b.ToTable(tablePrefix + "Specifications");
            b.HasDiscriminator<string>("SpecType")
                .HasValue<MonitorSpecification>("Monitor")//1
                .HasValue<MouseSpecification>("Mouse")
                .HasValue<LaptopSpecification>("Laptop")
                .HasValue<CpuSpecification>("CPU")
                .HasValue<GpuSpecification>("GPU")//5
                .HasValue<RamSpecification>("RAM")
                .HasValue<MotherboardSpecification>("Motherboard")
                .HasValue<StorageSpecification>("Storage")
                .HasValue<PsuSpecification>("PSU")
                .HasValue<CaseSpecification>("Case")//10
                .HasValue<CpuCoolerSpecification>("CPUCooler")
                .HasValue<KeyboardSpecification>("Keyboard")
                .HasValue<HeadsetSpecification>("Headset")
                .HasValue<SpeakerSpecification>("Speaker")
                .HasValue<WebcamSpecification>("Webcam")//15
                .HasValue<CableSpecification>("Cable")
                .HasValue<SoftwareSpecification>("Software")
                .HasValue<CaseFanSpecification>("CaseFan")
                .HasValue<ChairSpecification>("Chair")
                .HasValue<DeskSpecification>("Desk")//20
                .HasValue<ChargerSpecification>("Charger")
                .HasValue<ConsoleSpecification>("Console")
                .HasValue<HandheldSpecification>("Handheld")
                .HasValue<HubSpecification>("Hub")
                .HasValue<MemoryCardSpecification>("MemoryCard")//25
                .HasValue<MicrophoneSpecification>("Microphone")
                .HasValue<MousePadSpecification>("MousePad")
                .HasValue<NetworkHardwareSpecification>("NetworkHardware")
                .HasValue<PowerBankSpecification>("PowerBank");//29


            builder.Entity<MonitorSpecification>()
                .HasOne(s => s.PanelType).WithMany()
                .HasForeignKey(s => s.PanelTypeId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.Entity<CpuSpecification>()
                .HasOne(s => s.Socket).WithMany()
                .HasForeignKey(s => s.SocketId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.Entity<RamSpecification>()
                .HasOne(s => s.RamType).WithMany()
                .HasForeignKey(s => s.RamTypeId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.Entity<MotherboardSpecification>()
                .HasOne(s => s.Socket).WithMany()
                .HasForeignKey(s => s.SocketId)
                .OnDelete(DeleteBehavior.Restrict);
            builder.Entity<MotherboardSpecification>()
                .HasOne(s => s.Chipset).WithMany()
                .HasForeignKey(s => s.ChipsetId)
                .OnDelete(DeleteBehavior.Restrict);
            builder.Entity<MotherboardSpecification>()
                .HasOne(s => s.FormFactor).WithMany()
                .HasForeignKey(s => s.FormFactorId)
                .OnDelete(DeleteBehavior.Restrict);
            builder.Entity<MotherboardSpecification>()
                .HasOne(s => s.SupportedRamTypes).WithMany()
                .HasForeignKey(s => s.RamTypeId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.Entity<PsuSpecification>()
                .HasOne(s => s.FormFactor).WithMany()
                .HasForeignKey(s => s.FormFactorId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.Entity<CaseSpecification>()
                .HasOne(s => s.FormFactor).WithMany()
                .HasForeignKey(s => s.FormFactorId)
                .OnDelete(DeleteBehavior.Restrict);
            builder.Entity<CaseSpecification>()
                .HasMany(cs => cs.Materials)
                .WithOne(cm => cm.CaseSpecification)
                .HasForeignKey(cm => cm.CaseSpecificationId)
                .IsRequired();

            builder.Entity<CpuCoolerSpecification>()
                .HasMany(ccs => ccs.SupportedSockets)
                .WithOne(css => css.CpuCoolerSpecification)
                .HasForeignKey(css => css.CpuCoolerSpecificationId)
                .IsRequired();

            builder.Entity<KeyboardSpecification>()
                .HasOne(s => s.SwitchType).WithMany()
                .HasForeignKey(s => s.SwitchTypeId)
                .OnDelete(DeleteBehavior.Restrict);
            builder.Entity<GpuSpecification>()
                .Property(s => s.Length).HasColumnType("decimal(18,2)");
            builder.Entity<CableSpecification>()
                .Property(s => s.Length).HasColumnType("decimal(18,2)");

            //descriminater 
            // ============================================================
            // Full HasColumnName fix for AppSpecifications TPH table
            // Add all of the following inside OnModelCreating, after the
            // main SpecificationBase block.
            //
            // Rule: every property that exists in 2+ spec types gets an
            // explicit column name on EVERY type that owns it, so there
            // are no silent case-insensitive collisions and no surprises
            // when new spec types are added later.
            // ============================================================

            // ── HasRGB / HasRgb (the actual crash) ──────────────────────
            // SQL Server is case-insensitive: HasRGB == HasRgb
            builder.Entity<RamSpecification>()
                .Property(s => s.HasRgb).HasColumnName("RamHasRgb");
            builder.Entity<CaseFanSpecification>()
                .Property(s => s.HasRgb).HasColumnName("CaseFanHasRgb");
            builder.Entity<MicrophoneSpecification>()
                .Property(s => s.HasRgb).HasColumnName("MicrophoneHasRgb");
            builder.Entity<MousePadSpecification>()
                .Property(s => s.HasRgb).HasColumnName("MousePadHasRgb");

            // ── Color (14 spec types) ────────────────────────────────────
            builder.Entity<CaseSpecification>()
                .Property(s => s.Color).HasColumnName("CaseColor");
            builder.Entity<CaseFanSpecification>()
                .Property(s => s.Color).HasColumnName("CaseFanColor");
            builder.Entity<ChairSpecification>()
                .Property(s => s.Color).HasColumnName("ChairColor");
            builder.Entity<ChargerSpecification>()
                .Property(s => s.Color).HasColumnName("ChargerColor");
            builder.Entity<CpuCoolerSpecification>()
                .Property(s => s.Color).HasColumnName("CpuCoolerColor");
            builder.Entity<HeadsetSpecification>()
                .Property(s => s.Color).HasColumnName("HeadsetColor");
            builder.Entity<HubSpecification>()
                .Property(s => s.Color).HasColumnName("HubColor");
            builder.Entity<MicrophoneSpecification>()
                .Property(s => s.Color).HasColumnName("MicrophoneColor");
            builder.Entity<MouseSpecification>()
                .Property(s => s.Color).HasColumnName("MouseColor");
            builder.Entity<MousePadSpecification>()
                .Property(s => s.Color).HasColumnName("PadColor");
            builder.Entity<SpeakerSpecification>()
                .Property(s => s.Color).HasColumnName("SpeakerColor");
            builder.Entity<CableSpecification>()
                .Property(s => s.Color).HasColumnName("CableColor");
            builder.Entity<WebcamSpecification>()
                .Property(s => s.Color).HasColumnName("WebcamColor");
            builder.Entity<DeskSpecification>()
                .Property(s => s.Color).HasColumnName("DeskColor");

            // ── Connectivity (8 spec types) ─────────────────────────────
            builder.Entity<KeyboardSpecification>()
                .Property(s => s.Connectivity).HasColumnName("KeyboardConnectivity");
            builder.Entity<HeadsetSpecification>()
                .Property(s => s.Connectivity).HasColumnName("HeadsetConnectivity");
            builder.Entity<MouseSpecification>()
                .Property(s => s.Connectivity).HasColumnName("MouseConnectivity");
            builder.Entity<MicrophoneSpecification>()
                .Property(s => s.Connectivity).HasColumnName("MicrophoneConnectivity");
            builder.Entity<ConsoleSpecification>()
                .Property(s => s.Connectivity).HasColumnName("ConsoleConnectivity");
            builder.Entity<HandheldSpecification>()
                .Property(s => s.Connectivity).HasColumnName("HandheldConnectivity");
            builder.Entity<SpeakerSpecification>()
                .Property(s => s.Connectivity).HasColumnName("SpeakerConnectivity");
            builder.Entity<WebcamSpecification>()
                .Property(s => s.Connectivity).HasColumnName("WebcamConnectivity");

            // ── Frequency (3 spec types) ────────────────────────────────
            builder.Entity<MicrophoneSpecification>()
                .Property(s => s.Frequency).HasColumnName("MicrophoneFrequency");
            builder.Entity<SpeakerSpecification>()
                .Property(s => s.Frequency).HasColumnName("SpeakerFrequency");
            builder.Entity<NetworkHardwareSpecification>()
                .Property(s => s.Frequency).HasColumnName("NetworkFrequency");

            // ── Sensitivity (Headset=int, Microphone=string) ────────────
            builder.Entity<HeadsetSpecification>()
                .Property(s => s.Sensitivity).HasColumnName("HeadsetSensitivity");
            builder.Entity<MicrophoneSpecification>()
                .Property(s => s.Sensitivity).HasColumnName("MicrophoneSensitivity");

            // ── MicrophoneType (Headset=string, Microphone=enum) ────────
            builder.Entity<HeadsetSpecification>()
                .Property(s => s.MicrophoneType).HasColumnName("HeadsetMicrophoneType");
            builder.Entity<MicrophoneSpecification>()
                .Property(s => s.MicrophoneType).HasColumnName("MicrophoneMicrophoneType");

            // ── HasMicrophone (Headset, Webcam) ─────────────────────────
            builder.Entity<HeadsetSpecification>()
                .Property(s => s.HasMicrophone).HasColumnName("HeadsetHasMicrophone");
            builder.Entity<WebcamSpecification>()
                .Property(s => s.HasMicrophone).HasColumnName("WebcamHasMicrophone");

            // ── Resolution (Monitor=string, Webcam=string) ───────────────
            builder.Entity<MonitorSpecification>()
                .Property(s => s.Resolution).HasColumnName("MonitorResolution");
            builder.Entity<WebcamSpecification>()
                .Property(s => s.Resolution).HasColumnName("WebcamResolution");

            // ── Connection (Microphone, Webcam) ─────────────────────────
            builder.Entity<MicrophoneSpecification>()
                .Property(s => s.Connection).HasColumnName("MicrophoneConnection");
            builder.Entity<WebcamSpecification>()
                .Property(s => s.Connection).HasColumnName("Webcam_Connection");

            // ── Interface (Gpu=string, Storage=string) ───────────────────
            builder.Entity<GpuSpecification>()
                .Property(s => s.Interface).HasColumnName("GpuInterface");
            builder.Entity<StorageSpecification>()
                .Property(s => s.Interface).HasColumnName("StorageInterface");

            // ── Capacity (MemoryCard, PowerBank, Ram, Storage) ──────────
            builder.Entity<MemoryCardSpecification>()
                .Property(s => s.Capacity).HasColumnName("MemoryCardCapacity");
            builder.Entity<PowerBankSpecification>()
                .Property(s => s.Capacity).HasColumnName("PowerBankCapacity");
            builder.Entity<RamSpecification>()
                .Property(s => s.Capacity).HasColumnName("RamCapacity");
            builder.Entity<StorageSpecification>()
                .Property(s => s.Capacity).HasColumnName("StorageCapacity");

            // ── TotalWattage (Charger, PowerBank, Speaker) ───────────────
            builder.Entity<ChargerSpecification>()
                .Property(s => s.TotalWattage).HasColumnName("ChargerTotalWattage");
            builder.Entity<PowerBankSpecification>()
                .Property(s => s.TotalWattage).HasColumnName("PowerBankTotalWattage");
            builder.Entity<SpeakerSpecification>()
                .Property(s => s.TotalWattage).HasColumnName("SpeakerTotalWattage");

            // ── PortCount (Charger, Hub, PowerBank) ─────────────────────
            builder.Entity<ChargerSpecification>()
                .Property(s => s.PortCount).HasColumnName("ChargerPortCount");
            builder.Entity<HubSpecification>()
                .Property(s => s.PortCount).HasColumnName("HubPortCount");
            builder.Entity<PowerBankSpecification>()
                .Property(s => s.PortCount).HasColumnName("PowerBankPortCount");

            // ── UsbCPorts (Charger, Hub, PowerBank) ─────────────────────
            builder.Entity<ChargerSpecification>()
                .Property(s => s.UsbCPorts).HasColumnName("ChargerUsbCPorts");
            builder.Entity<HubSpecification>()
                .Property(s => s.UsbCPorts).HasColumnName("HubUsbCPorts");
            builder.Entity<PowerBankSpecification>()
                .Property(s => s.UsbCPorts).HasColumnName("PowerBankUsbCPorts");

            // ── UsbAPorts (Charger, Hub, PowerBank) ─────────────────────
            builder.Entity<ChargerSpecification>()
                .Property(s => s.UsbAPorts).HasColumnName("ChargerUsbAPorts");
            builder.Entity<HubSpecification>()
                .Property(s => s.UsbAPorts).HasColumnName("HubUsbAPorts");
            builder.Entity<PowerBankSpecification>()
                .Property(s => s.UsbAPorts).HasColumnName("PowerBankUsbAPorts");

            // ── InputPorts (PowerBank, Speaker) ─────────────────────────
            builder.Entity<PowerBankSpecification>()
                .Property(s => s.InputPorts).HasColumnName("PowerBankInputPorts");
            builder.Entity<SpeakerSpecification>()
                .Property(s => s.InputPorts).HasColumnName("SpeakerInputPorts");

            // ── MaxOutputPerPort (Charger, PowerBank) ───────────────────
            builder.Entity<ChargerSpecification>()
                .Property(s => s.MaxOutputPerPort).HasColumnName("ChargerMaxOutputPerPort");
            builder.Entity<PowerBankSpecification>()
                .Property(s => s.MaxOutputPerPort).HasColumnName("PowerBankMaxOutputPerPort");

            // ── FastChargingProtocols (Charger, PowerBank) ──────────────
            builder.Entity<ChargerSpecification>()
                .Property(s => s.FastChargingProtocols).HasColumnName("ChargerFastChargingProtocols");
            builder.Entity<PowerBankSpecification>()
                .Property(s => s.FastChargingProtocols).HasColumnName("PowerBankFastChargingProtocols");

            // ── Weight (Mouse=int, Handheld=string, PowerBank=int) ───────
            builder.Entity<MouseSpecification>()
                .Property(s => s.Weight).HasColumnName("MouseWeight");
            builder.Entity<HandheldSpecification>()
                .Property(s => s.Weight).HasColumnName("HandheldWeight");
            builder.Entity<PowerBankSpecification>()
                .Property(s => s.Weight).HasColumnName("PowerBankWeight");

            // ── Material (Chair=enum, Desk=enum, MousePad=enum) ─────────
            builder.Entity<ChairSpecification>()
                .Property(s => s.Material).HasColumnName("ChairMaterial");
            builder.Entity<DeskSpecification>()
                .Property(s => s.Material).HasColumnName("DeskMaterial");
            builder.Entity<MousePadSpecification>()
                .Property(s => s.Material).HasColumnName("MousePadMaterial");

            // ── SurfaceType (Desk=string, MousePad=enum) ─────────────────
            builder.Entity<DeskSpecification>()
                .Property(s => s.SurfaceType).HasColumnName("DeskSurfaceType");
            builder.Entity<MousePadSpecification>()
                .Property(s => s.SurfaceType).HasColumnName("MousePadSurfaceType");

            // ── BaseType (Chair=string, MousePad=string) ─────────────────
            builder.Entity<ChairSpecification>()
                .Property(s => s.BaseType).HasColumnName("ChairBaseType");
            builder.Entity<MousePadSpecification>()
                .Property(s => s.BaseType).HasColumnName("MousePadBaseType");

            // ── MaxWeight (Chair=int, Desk=int) ─────────────────────────
            builder.Entity<ChairSpecification>()
                .Property(s => s.MaxWeight).HasColumnName("ChairMaxWeight");
            builder.Entity<DeskSpecification>()
                .Property(s => s.MaxWeight).HasColumnName("DeskMaxWeight");

            // ── Height (CpuCooler=float?, Desk=float, MousePad=int) ──────
            builder.Entity<CpuCoolerSpecification>()
                .Property(s => s.Height).HasColumnName("CpuCoolerHeight");
            builder.Entity<DeskSpecification>()
                .Property(s => s.Height).HasColumnName("DeskHeight");
            builder.Entity<MousePadSpecification>()
                .Property(s => s.Height).HasColumnName("MousePadHeight");

            // ── Width (Desk=int, MousePad=int) ───────────────────────────
            builder.Entity<DeskSpecification>()
                .Property(s => s.Width).HasColumnName("DeskWidth");
            builder.Entity<MousePadSpecification>()
                .Property(s => s.Width).HasColumnName("MousePadWidth");

            // ── Processor (Console, Handheld) ───────────────────────────
            builder.Entity<ConsoleSpecification>()
                .Property(s => s.Processor).HasColumnName("ConsoleProcessor");
            builder.Entity<HandheldSpecification>()
                .Property(s => s.Processor).HasColumnName("HandheldProcessor");

            // ── Graphics (Console, Handheld) ────────────────────────────
            builder.Entity<ConsoleSpecification>()
                .Property(s => s.Graphics).HasColumnName("ConsoleGraphics");
            builder.Entity<HandheldSpecification>()
                .Property(s => s.Graphics).HasColumnName("HandheldGraphics");

            // ── RAM (Console, Handheld, Laptop) ─────────────────────────
            builder.Entity<ConsoleSpecification>()
                .Property(s => s.RAM).HasColumnName("ConsoleRAM");
            builder.Entity<HandheldSpecification>()
                .Property(s => s.RAM).HasColumnName("HandheldRAM");
            builder.Entity<LaptopSpecification>()
                .Property(s => s.RAM).HasColumnName("LaptopRAM");

            // ── Storage (Console, Handheld, Laptop) ─────────────────────
            builder.Entity<ConsoleSpecification>()
                .Property(s => s.Storage).HasColumnName("ConsoleStorage");
            builder.Entity<HandheldSpecification>()
                .Property(s => s.Storage).HasColumnName("HandheldStorage");
            builder.Entity<LaptopSpecification>()
                .Property(s => s.Storage).HasColumnName("LaptopStorage");

            // ── Display (Handheld, Laptop) ───────────────────────────────
            builder.Entity<HandheldSpecification>()
                .Property(s => s.Display).HasColumnName("HandheldDisplay");
            builder.Entity<LaptopSpecification>()
                .Property(s => s.Display).HasColumnName("LaptopDisplay");

            // ── BatteryLife (Handheld, Laptop) ──────────────────────────
            builder.Entity<HandheldSpecification>()
                .Property(s => s.BatteryLife).HasColumnName("HandheldBatteryLife");
            builder.Entity<LaptopSpecification>()
                .Property(s => s.BatteryLife).HasColumnName("LaptopBatteryLife");

            // ── OperatingSystem (Handheld, Laptop) ──────────────────────
            builder.Entity<HandheldSpecification>()
                .Property(s => s.OperatingSystem).HasColumnName("HandheldOperatingSystem");
            builder.Entity<LaptopSpecification>()
                .Property(s => s.OperatingSystem).HasColumnName("LaptopOperatingSystem");

            // ── WifiVersion (Console, Handheld) ─────────────────────────
            builder.Entity<ConsoleSpecification>()
                .Property(s => s.WifiVersion).HasColumnName("ConsoleWifiVersion");
            builder.Entity<HandheldSpecification>()
                .Property(s => s.WifiVersion).HasColumnName("HandheldWifiVersion");

            // ── BluetoothVersion (Console, Handheld) ────────────────────
            builder.Entity<ConsoleSpecification>()
                .Property(s => s.BluetoothVersion).HasColumnName("ConsoleBluetoothVersion");
            builder.Entity<HandheldSpecification>()
                .Property(s => s.BluetoothVersion).HasColumnName("HandheldBluetoothVersion");

            // ── MaxResolution (Console, Hub) ────────────────────────────
            builder.Entity<ConsoleSpecification>()
                .Property(s => s.MaxResolution).HasColumnName("ConsoleMaxResolution");
            builder.Entity<HubSpecification>()
                .Property(s => s.MaxResolution).HasColumnName("HubMaxResolution");

            // ── Warranty (Cable, MemoryCard, Laptop) ────────────────────
            builder.Entity<CableSpecification>()
                .Property(s => s.Warranty).HasColumnName("CableWarranty");
            builder.Entity<MemoryCardSpecification>()
                .Property(s => s.Warranty).HasColumnName("MemoryCardWarranty");
            builder.Entity<LaptopSpecification>()
                .Property(s => s.Warranty).HasColumnName("LaptopWarranty");

            // ── BoostClock (Cpu=float, Gpu=float) ───────────────────────
            builder.Entity<CpuSpecification>()
                .Property(s => s.BoostClock).HasColumnName("CpuBoostClock");
            builder.Entity<GpuSpecification>()
                .Property(s => s.BoostClock).HasColumnName("GpuBoostClock");

            // ── FanSize (CaseFan=int, CpuCooler=int) ────────────────────
            builder.Entity<CaseFanSpecification>()
                .Property(s => s.FanSize).HasColumnName("CaseFanFanSize");
            builder.Entity<CpuCoolerSpecification>()
                .Property(s => s.FanSize).HasColumnName("CpuCoolerFanSize");

            // ── NoiseLevel (CaseFan=float, CpuCooler=int) ───────────────
            builder.Entity<CaseFanSpecification>()
                .Property(s => s.NoiseLevel).HasColumnName("CaseFanNoiseLevel");
            builder.Entity<CpuCoolerSpecification>()
                .Property(s => s.NoiseLevel).HasColumnName("CpuCoolerNoiseLevel");

            // ── ReadSpeed (MemoryCard, Storage) ─────────────────────────
            builder.Entity<MemoryCardSpecification>()
                .Property(s => s.ReadSpeed).HasColumnName("MemoryCardReadSpeed");
            builder.Entity<StorageSpecification>()
                .Property(s => s.ReadSpeed).HasColumnName("StorageReadSpeed");

            // ── WriteSpeed (MemoryCard, Storage) ────────────────────────
            builder.Entity<MemoryCardSpecification>()
                .Property(s => s.WriteSpeed).HasColumnName("MemoryCardWriteSpeed");
            builder.Entity<StorageSpecification>()
                .Property(s => s.WriteSpeed).HasColumnName("StorageWriteSpeed");

            // ── FormFactorId (Case, Motherboard, Psu) ───────────────────
            builder.Entity<CaseSpecification>()
                .Property(s => s.FormFactorId).HasColumnName("CaseFormFactorId");
            builder.Entity<MotherboardSpecification>()
                .Property(s => s.FormFactorId).HasColumnName("MotherboardFormFactorId");
            builder.Entity<PsuSpecification>()
                .Property(s => s.FormFactorId).HasColumnName("PsuFormFactorId");

            // ── SocketId (Cpu, Motherboard) ──────────────────────────────
            builder.Entity<CpuSpecification>()
                .Property(s => s.SocketId).HasColumnName("CpuSocketId");
            builder.Entity<MotherboardSpecification>()
                .Property(s => s.SocketId).HasColumnName("MotherboardSocketId");

            // ── RamTypeId (Ram, Motherboard) ────────────────────────────
            builder.Entity<RamSpecification>()
                .Property(s => s.RamTypeId).HasColumnName("RamRamTypeId");
            builder.Entity<MotherboardSpecification>()
                .Property(s => s.RamTypeId).HasColumnName("MotherboardRamTypeId");

            // ── GpuSpecification.Length (decimal) ───────────────────────
            // Only one spec has Length as decimal — explicit type needed
            builder.Entity<GpuSpecification>()
                .Property(s => s.Length).HasColumnName("GpuLength").HasColumnType("decimal(18,2)");


            // Product relationship
            b.HasOne(s => s.Product)
                .WithOne(p => p.SpecificationBase)
                .HasForeignKey<SpecificationBase>(s => s.ProductId)
                .OnDelete(DeleteBehavior.Cascade);

            b.HasQueryFilter(s => !s.Product.IsDeleted);

            //// Columns that need explicit type mapping
            //b.Property<decimal>("Length").HasColumnType("decimal(18,2)").IsRequired(false);
        });
        #endregion

        #region Orders and Carts
        builder.Entity<Order>(b =>
        {
            b.ToTable(tablePrefix + "Orders");
            b.ConfigureFullAuditedAggregateRoot();
            b.ConfigureByConvention();

            b.Property(o => o.OrderNumber).IsRequired().HasMaxLength(OrderConsts.MaxOrderNumberLength);
            b.HasIndex(o => o.OrderNumber).IsUnique();
            b.Property(o => o.CustomerName).IsRequired().HasMaxLength(100);
            b.Property(o => o.CustomerPhone).HasMaxLength(OrderConsts.MaxCustomerPhoneLentgth);
            b.Property(o => o.TotalAmount).HasColumnType("decimal(18,2)").IsRequired();
            b.Property(x => x.PaymentMethod).IsRequired();
            /*            b.Property(x => x.SellerName).HasMaxLength(128).IsRequired(false);
                        b.Property(x => x.CashierName).HasMaxLength(128).IsRequired(false);
                        b.Property(x => x.FulfillerName).HasMaxLength(128).IsRequired(false);*/

            b.HasIndex(x => x.OrderNumber).IsUnique();
            b.HasIndex(x => x.StoreId);
            b.HasIndex(x => x.OrderType);
            b.HasIndex(x => x.OrderStatus);
            b.HasIndex(x => x.PaymentStatus);
            b.HasIndex(x => x.CustomerId);
            b.HasIndex(x => x.CreationTime);

            b.HasMany(o => o.OrderItems)
             .WithOne()
             .HasForeignKey(oi => oi.OrderId)
             .IsRequired();
            b.HasMany(x => x.OrderHistories)
                .WithOne()
                .HasForeignKey(x => x.OrderId)
                .OnDelete(DeleteBehavior.Cascade);
            b.HasDiscriminator<OrderType>(nameof(Order.OrderType))
                .HasValue<OnlineOrder>(OrderType.Online)
                .HasValue<InStoreOrder>(OrderType.InStore);
        });
        builder.Entity<OnlineOrder>(b =>
        {
            b.Property(o => o.ShippingAddress).IsRequired(false);
        });
        builder.Entity<InStoreOrder>(b =>
        {
            b.Property(o => o.SellerId).IsRequired(false);
            b.Property(o => o.SellerName).HasMaxLength(128).IsRequired(false);
            b.Property(o => o.CashierId).IsRequired(false);
            b.Property(o => o.CashierName).HasMaxLength(128).IsRequired(false);
            b.Property(o => o.FulfillerId).IsRequired(false);
            b.Property(o => o.FulfillerName).HasMaxLength(128).IsRequired(false);
            b.Property(o => o.CompletedAt).IsRequired(false);
            b.Property(o => o.FulfilledAt).IsRequired(false);
        });
        builder.Entity<OrderItem>(b =>
        {
            b.ToTable(tablePrefix + "OrderItems");
            b.ConfigureByConvention();

            b.Property(oi => oi.ProductName).IsRequired();
            b.Property(oi => oi.Price).HasColumnType("decimal(18,2)").IsRequired();
            b.Property(oi => oi.Quantity).IsRequired();
        });

        builder.Entity<OrderHistory>(b =>
        {
            b.ToTable(tablePrefix + "OrderHistories");
            b.ConfigureByConvention();

            b.Property(x => x.ChangeDescription).HasMaxLength(500);
            b.Property(x => x.ChangedBy).HasMaxLength(256);

            b.HasIndex(x => x.OrderId);
            b.HasIndex(x => x.CreationTime);
        });

        builder.Entity<Cart>(b =>
        {
            b.ToTable(tablePrefix + "Carts");
            b.ConfigureAuditedAggregateRoot();

            b.HasIndex(c => c.UserId).IsUnique();

            b.HasMany(c => c.Items)
             .WithOne()
             .HasForeignKey(ci => ci.CartId)
             .IsRequired().OnDelete(DeleteBehavior.Cascade);


        });

        builder.Entity<CartItem>(b =>
        {
            b.ToTable(tablePrefix + "CartItems");
            b.ConfigureByConvention();
            b.Property(ci => ci.ProductPrice).HasConversion<decimal>()
                .HasColumnType("decimal(18,2)")
                .IsRequired();
            b.HasIndex(ci => ci.ProductId);
        });
        #endregion

        #region Users
        //staff
        builder.Entity<AppUser>(b =>
        {
            b.ToTable(AbpIdentityDbProperties.DbTablePrefix + "Users");
            b.ConfigureByConvention();
            b.Property<Guid?>("AssignedStoreId")
                 .HasColumnName("AssignedStoreId");
            b.HasOne(u => u.Customer)
             .WithOne(c => c.AppUser)
             .HasForeignKey<Customer>(c => c.AppUserId)
             .IsRequired()
             .OnDelete(DeleteBehavior.Restrict);
        });
        //customer
        builder.Entity<Customer>(b =>
        {
            b.ToTable(tablePrefix + "Customers");
            b.ConfigureByConvention();
            b.Property(u => u.DateOfBirth).IsRequired(false);
            b.Property(u => u.Gender).IsRequired().HasDefaultValue(UserGender.NONE);
            b.Property(u => u.PhoneNumber).IsRequired(false);
            b.Property(u => u.ShippingAddress).IsRequired(false);
            b.HasIndex(c => c.AppUserId).IsUnique();

        });
        #endregion
    }
}