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
public class ProductSellingDbContext : AbpDbContext<ProductSellingDbContext>, ITenantManagementDbContext, IIdentityDbContext
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
    public DbSet<Customer> Addresses { get; set; }
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
        #region Default Modules
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
        #region Junction Tables
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

        #endregion
        #region Specifications

        // 0. Base Table Configuration
        builder.Entity<SpecificationBase>(b =>
        {
            b.ToTable(tablePrefix + "Specifications");

            // Product relationship
            b.HasOne(s => s.Product)
                .WithOne(p => p.SpecificationBase)
                .HasForeignKey<SpecificationBase>(s => s.ProductId)
                .OnDelete(DeleteBehavior.Cascade);

            b.HasQueryFilter(s => !s.Product.IsDeleted);
        });

        // 1. Monitor
        builder.Entity<MonitorSpecification>(b =>
        {
            b.ToTable(tablePrefix + "MonitorSpecifications");

            b.HasOne(s => s.PanelType).WithMany()
                .HasForeignKey(s => s.PanelTypeId)
                .OnDelete(DeleteBehavior.Restrict);
        });

        // 2. Mouse
        builder.Entity<MouseSpecification>(b => b.ToTable(tablePrefix + "MouseSpecifications"));

        // 3. Laptop
        builder.Entity<LaptopSpecification>(b => b.ToTable(tablePrefix + "LaptopSpecifications"));

        // 4. CPU
        builder.Entity<CpuSpecification>(b =>
        {
            b.ToTable(tablePrefix + "CpuSpecifications");

            b.HasOne(s => s.Socket).WithMany()
                .HasForeignKey(s => s.SocketId)
                .OnDelete(DeleteBehavior.Restrict);
        });

        // 5. GPU
        builder.Entity<GpuSpecification>(b =>
        {
            b.ToTable(tablePrefix + "GpuSpecifications");

            b.Property(s => s.Length).HasColumnType("decimal(18,2)");
        });

        // 6. RAM
        builder.Entity<RamSpecification>(b =>
        {
            b.ToTable(tablePrefix + "RamSpecifications");

            b.HasOne(s => s.RamType).WithMany()
                .HasForeignKey(s => s.RamTypeId)
                .OnDelete(DeleteBehavior.Restrict);
        });

        // 7. Motherboard
        builder.Entity<MotherboardSpecification>(b =>
        {
            b.ToTable(tablePrefix + "MotherboardSpecifications");

            b.HasOne(s => s.Socket).WithMany()
                .HasForeignKey(s => s.SocketId)
                .OnDelete(DeleteBehavior.Restrict);
            b.HasOne(s => s.Chipset).WithMany()
                .HasForeignKey(s => s.ChipsetId)
                .OnDelete(DeleteBehavior.Restrict);
            b.HasOne(s => s.FormFactor).WithMany()
                .HasForeignKey(s => s.FormFactorId)
                .OnDelete(DeleteBehavior.Restrict);
            b.HasOne(s => s.SupportedRamTypes).WithMany()
                .HasForeignKey(s => s.RamTypeId)
                .OnDelete(DeleteBehavior.Restrict);
        });

        // 8. Storage
        builder.Entity<StorageSpecification>(b => b.ToTable(tablePrefix + "StorageSpecifications"));

        // 9. PSU
        builder.Entity<PsuSpecification>(b =>
        {
            b.ToTable(tablePrefix + "PsuSpecifications");

            b.HasOne(s => s.FormFactor).WithMany()
                .HasForeignKey(s => s.FormFactorId)
                .OnDelete(DeleteBehavior.Restrict);
        });

        // 10. Case
        builder.Entity<CaseSpecification>(b =>
        {
            b.ToTable(tablePrefix + "CaseSpecifications");

            b.HasOne(s => s.FormFactor).WithMany()
                .HasForeignKey(s => s.FormFactorId)
                .OnDelete(DeleteBehavior.Restrict);
            b.HasMany(cs => cs.Materials)
                .WithOne(cm => cm.CaseSpecification)
                .HasForeignKey(cm => cm.CaseSpecificationId)
                .IsRequired();
        });

        // 11. CPU Cooler
        builder.Entity<CpuCoolerSpecification>(b =>
        {
            b.ToTable(tablePrefix + "CpuCoolerSpecifications");

            b.HasMany(ccs => ccs.SupportedSockets)
                .WithOne(css => css.CpuCoolerSpecification)
                .HasForeignKey(css => css.CpuCoolerSpecificationId)
                .IsRequired();
        });

        // 12. Keyboard
        builder.Entity<KeyboardSpecification>(b =>
        {
            b.ToTable(tablePrefix + "KeyboardSpecifications");

            b.HasOne(s => s.SwitchType).WithMany()
                .HasForeignKey(s => s.SwitchTypeId)
                .OnDelete(DeleteBehavior.Restrict);
        });

        // 13. Headset
        builder.Entity<HeadsetSpecification>(b => b.ToTable(tablePrefix + "HeadsetSpecifications"));

        // 14. Speaker
        builder.Entity<SpeakerSpecification>(b => b.ToTable(tablePrefix + "SpeakerSpecifications"));

        // 15. Webcam
        builder.Entity<WebcamSpecification>(b => b.ToTable(tablePrefix + "WebcamSpecifications"));

        // 16. Cable
        builder.Entity<CableSpecification>(b =>
        {
            b.ToTable(tablePrefix + "CableSpecifications");

            b.Property(s => s.Length).HasColumnType("decimal(18,2)");
        });


        // 18. Case Fan
        builder.Entity<CaseFanSpecification>(b => b.ToTable(tablePrefix + "CaseFanSpecifications"));







        // 26. Microphone
        builder.Entity<MicrophoneSpecification>(b => b.ToTable(tablePrefix + "MicrophoneSpecifications"));

        // 27. Mouse Pad
        builder.Entity<MousePadSpecification>(b => b.ToTable(tablePrefix + "MousePadSpecifications"));



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
            b.HasIndex(c => c.AppUserId).IsUnique();
        });
        builder.Entity<Address>(b =>
        {
            b.ToTable(tablePrefix + "Addresses");
            b.ConfigureByConvention();
            b.HasKey(x => x.Id);
            b.Property(x => x.FullAddress).IsRequired().HasMaxLength(AddressConsts.MaxAddressLength);

            b.HasOne<Customer>()
             .WithMany(c => c.ShippingAddresses)
             .HasForeignKey(a => a.CustomerId)
             .OnDelete(DeleteBehavior.Cascade);
        });
        #endregion
    }
}