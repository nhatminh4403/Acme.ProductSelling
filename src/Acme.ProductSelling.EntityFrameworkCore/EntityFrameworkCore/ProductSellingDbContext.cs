using Acme.ProductSelling.Carts;
using Acme.ProductSelling.Categories;
using Acme.ProductSelling.Manufacturers;
using Acme.ProductSelling.Orders;
using Acme.ProductSelling.Products;
using Acme.ProductSelling.Specifications;
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

    /* Notice: We only implemented IIdentityProDbContext and ISaasDbContext
     * and replaced them for this DbContext. This allows you to perform JOIN
     * queries for the entities of these modules over the repositories easily. You
     * typically don't need that for other modules. But, if you need, you can
     * implement the DbContext interface of the needed module and use ReplaceDbContext
     * attribute just like IIdentityProDbContext and ISaasDbContext.
     *
     * More info: Replacing a DbContext of a module ensures that the related module
     * uses this DbContext on runtime. Otherwise, it will use its own DbContext class.
     */

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
    public DbSet<Manufacturer> Manufacturers { get; set; }
    public DbSet<Order> Orders { get; set; }
    public DbSet<OrderItem> OrderItems { get; set; }

    public DbSet<Cart> Carts { get; set; }
    public DbSet<CartItem> CartItems { get; set; }
    #endregion

    public ProductSellingDbContext(DbContextOptions<ProductSellingDbContext> options)
        : base(options)
    {

    }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        /* Include modules to your migration db context */

        builder.ConfigurePermissionManagement();
        builder.ConfigureSettingManagement();
        builder.ConfigureBackgroundJobs();
        builder.ConfigureAuditLogging();
        builder.ConfigureFeatureManagement();
        builder.ConfigureIdentity();
        builder.ConfigureOpenIddict();
        builder.ConfigureTenantManagement();
        builder.ConfigureBlobStoring();

        /* Configure your own tables/entities inside here */

        builder.Entity<Category>(b =>
        {
            b.ToTable("Categories");
            b.Property(c => c.Name).IsRequired().HasMaxLength(100);
            b.HasMany(c => c.Products).WithOne(p => p.Category).HasForeignKey(p => p.CategoryId);
            b.HasIndex(p => p.UrlSlug);
        });
        builder.Entity<Manufacturer>(b =>
        {
            b.ToTable("Manufacturers");
            b.Property(c => c.Name).IsRequired().HasMaxLength(100);
            b.HasMany(c => c.Products).WithOne(p => p.Manufacturer).HasForeignKey(p => p.ManufacturerId);
            b.HasIndex(p => p.UrlSlug);
        });
        builder.Entity<Product>(b =>
        {
            b.ToTable("Products");
            b.Property(p => p.ProductName).IsRequired().HasMaxLength(100);
            b.Property(p => p.OriginalPrice).HasColumnType("decimal(18,2)");
            b.Property(p => p.DiscountedPrice).HasColumnType("decimal(18,2)").IsRequired(false);
            b.Property(p => p.DiscountPercent).IsRequired(true);
            b.HasOne(p => p.Category).WithMany(c => c.Products).HasForeignKey(p => p.CategoryId);
            b.HasOne(p => p.Manufacturer).WithMany(m => m.Products).HasForeignKey(p => p.ManufacturerId);
            b.HasOne(p => p.MonitorSpecification).WithOne().HasForeignKey<Product>(p => p.MonitorSpecificationId).IsRequired(false);
            b.HasOne(p => p.MouseSpecification).WithOne().HasForeignKey<Product>(p => p.MouseSpecificationId).IsRequired(false);
            b.HasOne(p => p.LaptopSpecification).WithOne().HasForeignKey<Product>(p => p.LaptopSpecificationId).IsRequired(false); // Giả sử có
            b.HasOne(p => p.CpuSpecification).WithOne().HasForeignKey<Product>(p => p.CpuSpecificationId).IsRequired(false);
            b.HasOne(p => p.GpuSpecification).WithOne().HasForeignKey<Product>(p => p.GpuSpecificationId).IsRequired(false);
            b.HasOne(p => p.RamSpecification).WithOne().HasForeignKey<Product>(p => p.RamSpecificationId).IsRequired(false);
            b.HasOne(p => p.MotherboardSpecification).WithOne().HasForeignKey<Product>(p => p.MotherboardSpecificationId).IsRequired(false);
            b.HasOne(p => p.StorageSpecification).WithOne().HasForeignKey<Product>(p => p.StorageSpecificationId).IsRequired(false);
            b.HasOne(p => p.PsuSpecification).WithOne().HasForeignKey<Product>(p => p.PsuSpecificationId).IsRequired(false);
            b.HasOne(p => p.CaseSpecification).WithOne().HasForeignKey<Product>(p => p.CaseSpecificationId).IsRequired(false);
            b.HasOne(p => p.CpuCoolerSpecification).WithOne().HasForeignKey<Product>(p => p.CpuCoolerSpecificationId).IsRequired(false);
            b.HasOne(p => p.KeyboardSpecification).WithOne().HasForeignKey<Product>(p => p.KeyboardSpecificationId).IsRequired(false);
            b.HasOne(p => p.HeadsetSpecification).WithOne().HasForeignKey<Product>(p => p.HeadsetSpecificationId).IsRequired(false);
            b.HasIndex(p => p.UrlSlug);
        });



        builder.Entity<MonitorSpecification>(b => { b.ToTable("AppMonitorSpecifications"); });
        builder.Entity<MouseSpecification>(b => { b.ToTable("AppMouseSpecifications"); });
        builder.Entity<LaptopSpecification>(b => { b.ToTable("AppLaptopSpecifications"); /* Cấu hình cột nếu cần */ });

        builder.Entity<CpuSpecification>(b => { b.ToTable("AppCpuSpecifications"); /* Cấu hình cột nếu cần */ });
        builder.Entity<GpuSpecification>(b =>
        {
            b.ToTable("AppGpuSpecifications");
            b.Property(s => s.Length).HasColumnType("decimal(18,2)");
        });
        builder.Entity<RamSpecification>(b => { b.ToTable("AppRamSpecifications"); });
        builder.Entity<MotherboardSpecification>(b => { b.ToTable("AppMotherboardSpecifications"); });
        builder.Entity<StorageSpecification>(b => { b.ToTable("AppStorageSpecifications"); });
        builder.Entity<PsuSpecification>(b => { b.ToTable("AppPsuSpecifications"); });
        builder.Entity<CaseSpecification>(b => { b.ToTable("AppCaseSpecifications"); b.Property(s => s.MaxGpuLength).HasColumnType("decimal(18,2)"); b.Property(s => s.MaxCpuCoolerHeight).HasColumnType("decimal(18,2)"); });
        builder.Entity<CpuCoolerSpecification>(b => { b.ToTable("AppCpuCoolerSpecifications"); b.Property(s => s.Height).HasColumnType("decimal(18,2)"); });
        builder.Entity<KeyboardSpecification>(b => { b.ToTable("AppKeyboardSpecifications"); });
        builder.Entity<HeadsetSpecification>(b => { b.ToTable("AppHeadsetSpecifications"); });



        builder.Entity<Order>(b =>
        {
            b.ToTable("AppOrders"); // Tên bảng
            b.ConfigureFullAuditedAggregateRoot(); // Cấu hình các trường audit

            b.Property(o => o.OrderNumber).IsRequired().HasMaxLength(OrderConsts.MaxOrderNumberLength);
            b.HasIndex(o => o.OrderNumber).IsUnique();
            b.Property(o => o.CustomerName).IsRequired().HasMaxLength(100);
            b.Property(o => o.CustomerPhone).HasMaxLength(OrderConsts.MaxCustomerPhoneLentgth);
            b.Property(o => o.ShippingAddress).IsRequired(); // Độ dài mặc định hoặc set MaxLength

            b.Property(o => o.TotalAmount).HasColumnType("decimal(18,2)").IsRequired();


            b.HasMany(o => o.OrderItems)
             .WithOne()
             .HasForeignKey(oi => oi.OrderId)
             .IsRequired();
        });

        builder.Entity<OrderItem>(b =>
        {
            b.ToTable("AppOrderItems");
            b.ConfigureByConvention();

            b.Property(oi => oi.ProductName).IsRequired();
            b.Property(oi => oi.Price).HasColumnType("decimal(18,2)").IsRequired();
            b.Property(oi => oi.Quantity).IsRequired();

        });


        builder.Entity<Cart>(b =>
        {
            b.ToTable("AppCarts");
            b.ConfigureAuditedAggregateRoot();

            b.HasIndex(c => c.UserId).IsUnique();

            b.HasMany(c => c.Items)
             .WithOne()
             .HasForeignKey(ci => ci.CartId)
             .IsRequired();
        });

        builder.Entity<CartItem>(b =>
        {
            b.ToTable("AppCartItems");
            b.ConfigureByConvention();
            b.Property(ci => ci.ProductPrice).HasConversion<decimal>()
                .HasColumnType("decimal(18,2)")
                .IsRequired();
            b.HasIndex(ci => ci.ProductId);
        });
    }
}
