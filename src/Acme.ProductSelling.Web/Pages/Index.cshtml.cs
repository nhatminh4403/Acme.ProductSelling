using Acme.ProductSelling.Products;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using Volo.Abp.Application.Dtos;

namespace Acme.ProductSelling.Web.Pages;

public class IndexModel : ProductSellingPageModel
{
    [BindProperty(SupportsGet = true)]
    public int CurrentPage { get; set; } = 1; // Trang hiện tại, mặc định là 1

    public PagedResultDto<ProductDto> ProductList { get; set; } // Giữ danh sách sản phẩm

    private readonly IProductAppService _productAppService;
    public  int PageSize = 9; // Ví dụ: 9 sản phẩm/trang

    public IndexModel(IProductAppService productAppService)
    {
        _productAppService = productAppService;
    }

    public async Task OnGetAsync()
    {
        // Tạo input DTO cho GetListAsync
        var input = new PagedAndSortedResultRequestDto
        {
            MaxResultCount = PageSize,
            SkipCount = (CurrentPage - 1) * PageSize,
            Sorting = "ProductName" // Sắp xếp theo tên (hoặc trường khác)
        };

        // Gọi service để lấy danh sách sản phẩm phân trang
        ProductList = await _productAppService.GetListAsync(input);
    }

}
