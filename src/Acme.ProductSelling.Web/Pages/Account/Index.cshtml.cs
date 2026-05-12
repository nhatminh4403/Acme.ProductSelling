using Acme.ProductSelling.Account.Dtos;
using Acme.ProductSelling.Account.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace Acme.ProductSelling.Web.Pages.Account
{
    [Authorize]
    public class IndexModel : ProductSellingPageModel
    {
        private readonly IProfileAppService _profileAppService;

        public CustomerProfileDto CustomerProfile { get; set; }

        [TempData]
        public string ErrorMessage { get; set; }

        public IndexModel(IProfileAppService profileAppService)
        {
            _profileAppService = profileAppService;
        }

        public async Task OnGetAsync()
        {
            CustomerProfile = await _profileAppService.GetAsync();
        }
    }
}
