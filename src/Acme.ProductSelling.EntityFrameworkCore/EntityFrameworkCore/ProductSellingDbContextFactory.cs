using System;
using System.IO;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;

namespace Acme.ProductSelling.EntityFrameworkCore;

/* This class is needed for EF Core console commands
 * (like Add-Migration and Update-Database commands) */
public class ProductSellingDbContextFactory : IDesignTimeDbContextFactory<ProductSellingDbContext>
{
    public ProductSellingDbContext CreateDbContext(string[] args)
    {
        var configuration = BuildConfiguration();
        
        ProductSellingEfCoreEntityExtensionMappings.Configure();

        var builder = new DbContextOptionsBuilder<ProductSellingDbContext>()
            .UseSqlServer(configuration.GetConnectionString("Default"));
        
        return new ProductSellingDbContext(builder.Options);
    }

    private static IConfigurationRoot BuildConfiguration()
    {
        var builder = new ConfigurationBuilder()
            .SetBasePath(Path.Combine(Directory.GetCurrentDirectory(), "../Acme.ProductSelling.DbMigrator/"))
            .AddJsonFile("appsettings.json", optional: false);

        return builder.Build();
    }
}
