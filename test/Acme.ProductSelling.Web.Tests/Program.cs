using Acme.ProductSelling;
using Microsoft.AspNetCore.Builder;
using Volo.Abp.AspNetCore.TestBase;

var builder = WebApplication.CreateBuilder();
builder.Environment.ContentRootPath = GetWebProjectContentRootPathHelper.Get("Acme.ProductSelling.Web.csproj");
await builder.RunAbpModuleAsync<ProductSellingWebTestModule>(applicationName: "Acme.ProductSelling.Web");

public partial class Program
{
}
