using Volo.Abp.Application.Dtos;
namespace Acme.ProductSelling.Orders.Dtos
{
    public class GetOrderListInput : PagedAndSortedResultRequestDto
    {
        public bool IncludeDeleted { get; set; }
    }
}
