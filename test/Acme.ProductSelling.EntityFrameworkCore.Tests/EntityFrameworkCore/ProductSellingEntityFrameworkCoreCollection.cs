using Xunit;

namespace Acme.ProductSelling.EntityFrameworkCore;

[CollectionDefinition(ProductSellingTestConsts.CollectionDefinitionName)]
public class ProductSellingEntityFrameworkCoreCollection : ICollectionFixture<ProductSellingEntityFrameworkCoreFixture>
{

}
