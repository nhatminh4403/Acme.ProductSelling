using Acme.ProductSelling.Blogs;
using Acme.ProductSelling.Carts;
using Acme.ProductSelling.Categories;
using Acme.ProductSelling.Chatbots;
using Acme.ProductSelling.Comments;
using Acme.ProductSelling.Manufacturers;
using Acme.ProductSelling.Orders;
using Acme.ProductSelling.Products;
using Acme.ProductSelling.Products.Lookups;
using Acme.ProductSelling.Specifications.Junctions;
using Acme.ProductSelling.Specifications.Models;
using Acme.ProductSelling.Users;
using Microsoft.EntityFrameworkCore;
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

    //Model Management
    public DbSet<Category> Categories { get; set; }
    public DbSet<Product> Products { get; set; }
    public DbSet<Manufacturer> Manufacturers { get; set; }
    public DbSet<Order> Orders { get; set; }
    public DbSet<OrderItem> OrderItems { get; set; }
    public DbSet<OrderHistory> OrderHistories { get; set; }
    public DbSet<Cart> Carts { get; set; }
    public DbSet<CartItem> CartItems { get; set; }
    public DbSet<Comment> Comments { get; set; }
    public DbSet<Blog> Blogs { get; set; }
    public DbSet<Likes> Likes { get; set; }
    public DbSet<Customer> Customers { get; set; }
    #endregion

    #region Specs
    // Original Specs
    public DbSet<MonitorSpecification> MonitorSpecifications { get; set; }
    public DbSet<MouseSpecification> MouseSpecifications { get; set; }
    public DbSet<LaptopSpecification> LaptopSpecifications { get; set; }
    public DbSet<CpuSpecification> CpuSpecifications { get; set; }
    public DbSet<GpuSpecification> GpuSpecifications { get; set; }
    public DbSet<RamSpecification> RamSpecifications { get; set; }
    public DbSet<MotherboardSpecification> MotherboardSpecifications { get; set; }
    public DbSet<StorageSpecification> StorageSpecifications { get; set; }
    public DbSet<PsuSpecification> PsuSpecifications { get; set; }
    public DbSet<CaseSpecification> CaseSpecifications { get; set; }
    public DbSet<CpuCoolerSpecification> CpuCoolerSpecifications { get; set; }
    public DbSet<KeyboardSpecification> KeyboardSpecifications { get; set; }
    public DbSet<HeadsetSpecification> HeadsetSpecifications { get; set; }

    // New Specs
    public DbSet<CaseFanSpecification> CaseFanSpecifications { get; set; }
    public DbSet<MemoryCardSpecification> MemoryCardSpecifications { get; set; }
    public DbSet<SpeakerSpecification> SpeakerSpecifications { get; set; }
    public DbSet<MicrophoneSpecification> MicrophoneSpecifications { get; set; }
    public DbSet<WebcamSpecification> WebcamSpecifications { get; set; }
    public DbSet<MousePadSpecification> MousePadSpecifications { get; set; }
    public DbSet<ChairSpecification> ChairSpecifications { get; set; }
    public DbSet<DeskSpecification> DeskSpecifications { get; set; }
    public DbSet<SoftwareSpecification> SoftwareSpecifications { get; set; }
    public DbSet<NetworkHardwareSpecification> NetworkHardwareSpecifications { get; set; }
    public DbSet<HandheldSpecification> HandheldSpecifications { get; set; }
    public DbSet<ConsoleSpecification> ConsoleSpecifications { get; set; }
    public DbSet<HubSpecification> HubSpecifications { get; set; }
    public DbSet<CableSpecification> CableSpecifications { get; set; }
    public DbSet<ChargerSpecification> ChargerSpecifications { get; set; }
    public DbSet<PowerBankSpecification> PowerBankSpecifications { get; set; }
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
    public DbSet<ChatbotMessage> ChatbotMessages { get; set; }
    public DbSet<ChatbotTrainingData> ChatbotTrainingData { get; set; }
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

        #region Original Specifications
        builder.Entity<CpuSpecification>(b =>
        {
            b.ToTable(tablePrefix + "CpuSpecifications");
            b.HasOne(s => s.Product).WithOne(p => p.CpuSpecification).HasForeignKey<CpuSpecification>(s => s.ProductId);
            b.HasOne(s => s.Socket).WithMany().HasForeignKey(s => s.SocketId);
            b.HasQueryFilter(s => !s.Product.IsDeleted);
        });

        builder.Entity<GpuSpecification>(b =>
        {
            b.ToTable(tablePrefix + "GpuSpecifications");
            b.HasOne(s => s.Product).WithOne(s => s.GpuSpecification).HasForeignKey<GpuSpecification>(s => s.ProductId);
            b.Property(s => s.Length).HasColumnType("decimal(18,2)"); b.HasQueryFilter(s => !s.Product.IsDeleted);
        });

        builder.Entity<RamSpecification>(b =>
        {
            b.ToTable(tablePrefix + "RamSpecifications");
            b.HasOne(s => s.Product).WithOne(p => p.RamSpecification).HasForeignKey<RamSpecification>(s => s.ProductId);
            b.HasOne(s => s.RamType).WithMany().HasForeignKey(s => s.RamTypeId); b.HasQueryFilter(s => !s.Product.IsDeleted);
        });

        builder.Entity<MotherboardSpecification>(b =>
        {
            b.ToTable(tablePrefix + "MotherboardSpecifications");
            b.HasOne(s => s.Product).WithOne(p => p.MotherboardSpecification).HasForeignKey<MotherboardSpecification>(s => s.ProductId);
            b.HasOne(s => s.Socket).WithMany().HasForeignKey(s => s.SocketId);
            b.HasOne(s => s.Chipset).WithMany().HasForeignKey(s => s.ChipsetId);
            b.HasOne(s => s.FormFactor).WithMany().HasForeignKey(s => s.FormFactorId);
            b.HasOne(s => s.SupportedRamTypes).WithMany().HasForeignKey(s => s.RamTypeId); b.HasQueryFilter(s => !s.Product.IsDeleted);
        });

        builder.Entity<StorageSpecification>(b =>
        {
            b.ToTable(tablePrefix + "StorageSpecifications");
            b.HasOne(s => s.Product).WithOne(p => p.StorageSpecification).HasForeignKey<StorageSpecification>(s => s.ProductId); b.HasQueryFilter(s => !s.Product.IsDeleted);
        });

        builder.Entity<PsuSpecification>(b =>
        {
            b.ToTable(tablePrefix + "PsuSpecifications");
            b.HasOne(s => s.Product).WithOne(p => p.PsuSpecification).HasForeignKey<PsuSpecification>(s => s.ProductId);
            b.HasOne(s => s.FormFactor).WithMany().HasForeignKey(s => s.FormFactorId); b.HasQueryFilter(s => !s.Product.IsDeleted);
        });

        builder.Entity<CaseSpecification>(b =>
        {
            b.ToTable(tablePrefix + "CaseSpecifications");
            b.HasOne(s => s.Product).WithOne(p => p.CaseSpecification).HasForeignKey<CaseSpecification>(s => s.ProductId);
            b.HasOne(s => s.FormFactor).WithMany().HasForeignKey(s => s.FormFactorId);

            b.HasMany(cs => cs.Materials)
               .WithOne(cm => cm.CaseSpecification)
               .HasForeignKey(cm => cm.CaseSpecificationId)
               .IsRequired();
            b.HasQueryFilter(s => !s.Product.IsDeleted);
        });

        builder.Entity<CaseMaterial>(b =>
        {
            b.ToTable(tablePrefix + "CaseMaterials");
            b.HasKey(cm => new { cm.CaseSpecificationId, cm.MaterialId });
            b.HasOne(cm => cm.Material).WithMany().HasForeignKey(cm => cm.MaterialId);
            b.HasQueryFilter(cm => !cm.CaseSpecification.Product.IsDeleted);

        });

        builder.Entity<CpuCoolerSpecification>(b =>
        {
            b.ToTable(tablePrefix + "CpuCoolerSpecifications");
            b.HasOne(s => s.Product).WithOne(p => p.CpuCoolerSpecification).HasForeignKey<CpuCoolerSpecification>(s => s.ProductId);

            b.HasMany(ccs => ccs.SupportedSockets)
               .WithOne(css => css.CpuCoolerSpecification)
               .HasForeignKey(css => css.CpuCoolerSpecificationId)
               .IsRequired();
            b.HasQueryFilter(s => !s.Product.IsDeleted);
        });

        builder.Entity<CpuCoolerSocketSupport>(b =>
        {
            b.ToTable(tablePrefix + "CpuCoolerSocketSupports");
            b.HasKey(css => new { css.CpuCoolerSpecificationId, css.SocketId });
            b.HasOne(css => css.Socket).WithMany().HasForeignKey(css => css.SocketId);
            b.HasQueryFilter(css => !css.CpuCoolerSpecification.Product.IsDeleted);

        });

        builder.Entity<KeyboardSpecification>(b =>
        {
            b.ToTable(tablePrefix + "KeyboardSpecifications");
            b.HasOne(s => s.Product).WithOne(p => p.KeyboardSpecification).HasForeignKey<KeyboardSpecification>(s => s.ProductId);
            b.HasOne(s => s.SwitchType).WithMany().HasForeignKey(s => s.SwitchTypeId); b.HasQueryFilter(s => !s.Product.IsDeleted);
        });

        builder.Entity<MonitorSpecification>(b =>
        {
            b.ToTable(tablePrefix + "MonitorSpecifications");
            b.HasOne(s => s.Product).WithOne(p => p.MonitorSpecification).HasForeignKey<MonitorSpecification>(s => s.ProductId);
            b.HasOne(s => s.PanelType).WithMany().HasForeignKey(s => s.PanelTypeId); b.HasQueryFilter(s => !s.Product.IsDeleted);
        });

        builder.Entity<MouseSpecification>(b =>
        {
            b.ToTable(tablePrefix + "MouseSpecifications");
            b.HasOne(s => s.Product).WithOne(p => p.MouseSpecification).HasForeignKey<MouseSpecification>(s => s.ProductId); b.HasQueryFilter(s => !s.Product.IsDeleted);
        });

        builder.Entity<LaptopSpecification>(b =>
        {
            b.ToTable(tablePrefix + "LaptopSpecifications");
            b.HasOne(s => s.Product).WithOne(p => p.LaptopSpecification).HasForeignKey<LaptopSpecification>(s => s.ProductId); b.HasQueryFilter(s => !s.Product.IsDeleted);
        });

        builder.Entity<HeadsetSpecification>(b =>
        {
            b.ToTable(tablePrefix + "HeadsetSpecifications");
            b.HasOne(s => s.Product).WithOne(p => p.HeadsetSpecification).HasForeignKey<HeadsetSpecification>(s => s.ProductId); b.HasQueryFilter(s => !s.Product.IsDeleted);
        });
        #endregion

        #region New Specifications
        builder.Entity<CaseFanSpecification>(b =>
        {
            b.ToTable(tablePrefix + "CaseFanSpecifications");
            b.HasOne(s => s.Product).WithOne(p => p.CaseFanSpecification).HasForeignKey<CaseFanSpecification>(s => s.ProductId); b.HasQueryFilter(s => !s.Product.IsDeleted);
        });

        builder.Entity<MemoryCardSpecification>(b =>
        {
            b.ToTable(tablePrefix + "MemoryCardSpecifications");
            b.HasOne(s => s.Product).WithOne(p => p.MemoryCardSpecification).HasForeignKey<MemoryCardSpecification>(s => s.ProductId); b.HasQueryFilter(s => !s.Product.IsDeleted);
        });

        builder.Entity<SpeakerSpecification>(b =>
        {
            b.ToTable(tablePrefix + "SpeakerSpecifications");
            b.HasOne(s => s.Product).WithOne(p => p.SpeakerSpecification).HasForeignKey<SpeakerSpecification>(s => s.ProductId); b.HasQueryFilter(s => !s.Product.IsDeleted);
        });

        builder.Entity<MicrophoneSpecification>(b =>
        {
            b.ToTable(tablePrefix + "MicrophoneSpecifications");
            b.HasOne(s => s.Product).WithOne(p => p.MicrophoneSpecification).HasForeignKey<MicrophoneSpecification>(s => s.ProductId); b.HasQueryFilter(s => !s.Product.IsDeleted);
        });

        builder.Entity<WebcamSpecification>(b =>
        {
            b.ToTable(tablePrefix + "WebcamSpecifications");
            b.HasOne(s => s.Product).WithOne(p => p.WebcamSpecification).HasForeignKey<WebcamSpecification>(s => s.ProductId); b.HasQueryFilter(s => !s.Product.IsDeleted);
        });

        builder.Entity<MousePadSpecification>(b =>
        {
            b.ToTable(tablePrefix + "MousePadSpecifications");
            b.HasOne(s => s.Product).WithOne(p => p.MousepadSpecification).HasForeignKey<MousePadSpecification>(s => s.ProductId); b.HasQueryFilter(s => !s.Product.IsDeleted);
        });

        builder.Entity<ChairSpecification>(b =>
        {
            b.ToTable(tablePrefix + "ChairSpecifications");
            b.HasOne(s => s.Product).WithOne(p => p.ChairSpecification).HasForeignKey<ChairSpecification>(s => s.ProductId); b.HasQueryFilter(s => !s.Product.IsDeleted);
        });

        builder.Entity<DeskSpecification>(b =>
        {
            b.ToTable(tablePrefix + "DeskSpecifications");
            b.HasOne(s => s.Product).WithOne(p => p.DeskSpecification).HasForeignKey<DeskSpecification>(s => s.ProductId); b.HasQueryFilter(s => !s.Product.IsDeleted);
        });

        builder.Entity<SoftwareSpecification>(b =>
        {
            b.ToTable(tablePrefix + "SoftwareSpecifications");
            b.HasOne(s => s.Product).WithOne(p => p.SoftwareSpecification).HasForeignKey<SoftwareSpecification>(s => s.ProductId); b.HasQueryFilter(s => !s.Product.IsDeleted);
        });

        builder.Entity<NetworkHardwareSpecification>(b =>
        {
            b.ToTable(tablePrefix + "NetworkHardwareSpecifications");
            b.HasOne(s => s.Product).WithOne(p => p.NetworkHardwareSpecification).HasForeignKey<NetworkHardwareSpecification>(s => s.ProductId); b.HasQueryFilter(s => !s.Product.IsDeleted);
        });

        builder.Entity<HandheldSpecification>(b =>
        {
            b.ToTable(tablePrefix + "HandheldSpecifications");
            b.HasOne(s => s.Product).WithOne(p => p.HandheldSpecification).HasForeignKey<HandheldSpecification>(s => s.ProductId); b.HasQueryFilter(s => !s.Product.IsDeleted);
        });

        builder.Entity<ConsoleSpecification>(b =>
        {
            b.ToTable(tablePrefix + "ConsoleSpecifications");
            b.HasOne(s => s.Product).WithOne(p => p.ConsoleSpecification).HasForeignKey<ConsoleSpecification>(s => s.ProductId); b.HasQueryFilter(s => !s.Product.IsDeleted);
        });

        builder.Entity<HubSpecification>(b =>
        {
            b.ToTable(tablePrefix + "HubSpecifications");
            b.HasOne(s => s.Product).WithOne(p => p.HubSpecification).HasForeignKey<HubSpecification>(s => s.ProductId); b.HasQueryFilter(s => !s.Product.IsDeleted);
        });

        builder.Entity<CableSpecification>(b =>
        {
            b.ToTable(tablePrefix + "CableSpecifications");
            b.HasOne(s => s.Product).WithOne(p => p.CableSpecification).HasForeignKey<CableSpecification>(s => s.ProductId); b.HasQueryFilter(s => !s.Product.IsDeleted);
        });

        builder.Entity<ChargerSpecification>(b =>
        {
            b.ToTable(tablePrefix + "ChargerSpecifications");
            b.HasOne(s => s.Product).WithOne(p => p.ChargerSpecification).HasForeignKey<ChargerSpecification>(s => s.ProductId); b.HasQueryFilter(s => !s.Product.IsDeleted);
        });

        builder.Entity<PowerBankSpecification>(b =>
        {
            b.ToTable(tablePrefix + "PowerBankSpecifications");
            b.HasOne(s => s.Product).WithOne(p => p.PowerBankSpecification).HasForeignKey<PowerBankSpecification>(s => s.ProductId); b.HasQueryFilter(s => !s.Product.IsDeleted);
        });
        #endregion

        #region Orders and Carts
        builder.Entity<Order>(b =>
        {
            b.ToTable(tablePrefix + "Orders");
            b.ConfigureFullAuditedAggregateRoot();

            b.Property(o => o.OrderNumber).IsRequired().HasMaxLength(OrderConsts.MaxOrderNumberLength);
            b.HasIndex(o => o.OrderNumber).IsUnique();
            b.Property(o => o.CustomerName).IsRequired().HasMaxLength(100);
            b.Property(o => o.CustomerPhone).HasMaxLength(OrderConsts.MaxCustomerPhoneLentgth);
            b.Property(o => o.ShippingAddress).IsRequired();
            b.Property(o => o.TotalAmount).HasColumnType("decimal(18,2)").IsRequired();

            b.HasMany(o => o.OrderItems)
             .WithOne()
             .HasForeignKey(oi => oi.OrderId)
             .IsRequired();
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
             .IsRequired();
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

        #region Comments and Blogs
        builder.Entity<Comment>(b =>
        {
            b.ToTable(tablePrefix + "Comments");
            b.ConfigureFullAuditedAggregateRoot();
            b.HasIndex(c => new { c.EntityType, c.EntityId });
        });

        builder.Entity<Likes>(b =>
        {
            b.ToTable(tablePrefix + "Likes");
            b.HasKey(l => new { l.CommentId, l.UserId });
            b.HasIndex(x => new { x.CommentId, x.UserId });
        });

        builder.Entity<Blog>(b =>
        {
            b.ToTable(tablePrefix + "Blogs");
            b.ConfigureFullAuditedAggregateRoot();
            b.Property(b => b.Title).IsRequired();
            b.Property(b => b.Content).IsRequired();
            b.Property(b => b.UrlSlug).IsRequired();
        });
        #endregion

        #region Chatbot
        builder.Entity<ChatbotMessage>(b =>
        {
            b.ToTable(tablePrefix + "ChatbotMessages");
            b.ConfigureByConvention();
            b.Property(x => x.UserMessage).IsRequired().HasMaxLength(2000);
            b.Property(x => x.BotResponse).IsRequired().HasMaxLength(5000);
            b.Property(x => x.SessionId).HasMaxLength(100);
        });

        builder.Entity<ChatbotTrainingData>(b =>
        {
            b.ToTable(tablePrefix + "ChatbotTrainingData");
            b.ConfigureByConvention();
            b.Property(x => x.Message).IsRequired().HasMaxLength(500);
            b.Property(x => x.Intent).IsRequired().HasMaxLength(100);
        });
        #endregion

        #region Users
        //staff
        builder.Entity<AppUser>(b =>
        {
            b.ToTable(AbpIdentityDbProperties.DbTablePrefix + "Users");
            b.ConfigureByConvention();
            b.Property(u => u.DateOfBirth).IsRequired(false);
            b.Property(u => u.Gender).IsRequired(false).HasDefaultValue(UserGender.NONE);
        });
        //customer
        builder.Entity<Customer>(b =>
        {
            b.ToTable(tablePrefix + "Customers");
            b.ConfigureByConvention();
            b.Property(u => u.DateOfBirth).IsRequired(false);
            b.Property(u => u.Gender).IsRequired(false).HasDefaultValue(UserGender.NONE);
        });
        #endregion
    }
}