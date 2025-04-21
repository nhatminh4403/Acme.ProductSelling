using Acme.ProductSelling.Samples;
using Xunit;

namespace Acme.ProductSelling.EntityFrameworkCore.Domains;

[Collection(ProductSellingTestConsts.CollectionDefinitionName)]
public class EfCoreSampleDomainTests : SampleDomainTests<ProductSellingEntityFrameworkCoreTestModule>
{

}
