using Acme.ProductSelling.Blogs;
using Acme.ProductSelling.Products.Dtos;
using Acme.ProductSelling.Products.Services;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Domain.Entities;

namespace Acme.ProductSelling.Web.Pages.Products
{
    public class ProductDetailModel : ProductSellingPageModel
    {
        //[BindProperty(SupportsGet = true)]
        //public Guid Id { get; set; }

        [BindProperty(SupportsGet = true)]
        public string Slug { get; set; }

        public ProductDto Product { get; private set; }
        public PagedResultDto<BlogDto> Blogs { get; private set; }


        private readonly IProductLookupAppService _productAppService;
        private readonly IBlogAppService _blogAppService;
        public ProductDetailModel(IProductLookupAppService productAppService, IBlogAppService blogAppService)
        {
            _productAppService = productAppService;
            _blogAppService = blogAppService;
        }


        public async Task<IActionResult> OnGetAsync()
        {
            try
            {

                var input = new PagedAndSortedResultRequestDto
                {
                    MaxResultCount = 4,
                    SkipCount = 0,
                    Sorting = "CreationTime DESC"
                };

                if (string.IsNullOrWhiteSpace(this.Slug))
                {
                    return NotFound();
                }
                //Product = await _productAppService.GetAsync(Id);
                Product = await _productAppService.GetProductBySlug(Slug);



                //Blogs = await _blogAppService.GetListAsync(input);

                return Page();
            }
            catch (EntityNotFoundException)
            {
                return NotFound();
            }
        }
    }
}
