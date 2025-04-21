using System.Threading.Tasks;
using Shouldly;
using Xunit;

namespace Acme.ProductSelling.Pages;

[Collection(ProductSellingTestConsts.CollectionDefinitionName)]
public class Index_Tests : ProductSellingWebTestBase
{
    [Fact]
    public async Task Welcome_Page()
    {
        var response = await GetResponseAsStringAsync("/");
        response.ShouldNotBeNull();
    }
}
