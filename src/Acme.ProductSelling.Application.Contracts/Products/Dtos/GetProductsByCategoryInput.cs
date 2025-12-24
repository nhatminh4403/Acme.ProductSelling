using System;
using Volo.Abp.Application.Dtos;
namespace Acme.ProductSelling.Products.Dtos
{
    public class GetProductsByCategoryInput : PagedAndSortedResultRequestDto
    {
        public Guid CategoryId { get; set; }
        }
}
