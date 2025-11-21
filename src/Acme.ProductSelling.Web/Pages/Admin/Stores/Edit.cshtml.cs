using Acme.ProductSelling.Permissions;
using Acme.ProductSelling.Stores.Dtos;
using Acme.ProductSelling.Stores.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;

namespace Acme.ProductSelling.Web.Pages.Admin.Stores
{
    [Authorize(ProductSellingPermissions.Stores.Edit)]
    public class EditModel : AdminPageModelBase
    {
        [BindProperty(SupportsGet = true)]
        public string Prefix { get; set; }

        [BindProperty(SupportsGet = true)]
        [HiddenInput]
        public Guid Id { get; set; }

        [BindProperty]
        public CreateUpdateStoreDto Store { get; set; }

        private readonly IStoreAppService _storeAppService;

        public EditModel(IStoreAppService storeAppService)
        {
            _storeAppService = storeAppService;
        }

        public async Task OnGetAsync()
        {
            var storeDto = await _storeAppService.GetAsync(Id);
            Store = ObjectMapper.Map<StoreDto, CreateUpdateStoreDto>(storeDto);
            if (Prefix != RoleBasedPrefix)
            {
                Response.Redirect(GetUrl($"/stores/edit/{Id}"));
            }
        }
        public async Task<IActionResult> OnPostAsync()
        {
            await _storeAppService.UpdateAsync(Id, Store);
            return Redirect(GetUrl("/stores"));
        }
    }
}
