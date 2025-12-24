using Volo.Abp.Application.Dtos;
namespace Acme.ProductSelling.Products.Dtos
{
    public class GetProductByNameDto : PagedAndSortedResultRequestDto
    {
        public string Filter { get; set; } = string.Empty;

    }
}
