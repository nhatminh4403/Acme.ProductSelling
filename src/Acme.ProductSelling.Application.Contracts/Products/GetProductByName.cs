using Volo.Abp.Application.Dtos;

namespace Acme.ProductSelling.Products
{
    public class GetProductByName : PagedAndSortedResultRequestDto
    {
        public string Sorting { get; set; } = "ProductName";
        public string Filter { get; set; } = string.Empty;
    }
}
