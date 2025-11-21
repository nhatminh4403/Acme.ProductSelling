using Acme.ProductSelling.Permissions;
using Acme.ProductSelling.Stores.Dtos;
using Acme.ProductSelling.Stores.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace Acme.ProductSelling.Web.Pages.Admin.Stores
{
    [Authorize(ProductSellingPermissions.Stores.Create)]
    public class CreateModel : AdminPageModelBase
    {
        [BindProperty(SupportsGet = true)]
        public string Prefix { get; set; }
        [BindProperty]
        public CreateUpdateStoreDto Store { get; set; }

        private readonly IStoreAppService _storeAppService;

        public CreateModel(IStoreAppService storeAppService)
        {
            _storeAppService = storeAppService;
        }

        public void OnGet()
        {
            Store = new CreateUpdateStoreDto();
            if (Prefix != RoleBasedPrefix)
            {
                Response.Redirect(GetUrl("/stores/create"));
            }
        }
        public async Task<IActionResult> OnPostAsync()
        {
            await _storeAppService.CreateAsync(Store);
            return Redirect(GetUrl("/stores"));
        }
    }
}
