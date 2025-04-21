using Acme.ProductSelling.Samples;
using Xunit;

namespace Acme.ProductSelling.EntityFrameworkCore.Applications;

[Collection(ProductSellingTestConsts.CollectionDefinitionName)]
public class EfCoreSampleAppServiceTests : SampleAppServiceTests<ProductSellingEntityFrameworkCoreTestModule>
{

}
