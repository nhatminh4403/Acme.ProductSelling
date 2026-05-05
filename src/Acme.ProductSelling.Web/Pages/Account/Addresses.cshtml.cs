using Acme.ProductSelling.Account.Dtos;
using Acme.ProductSelling.Account.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Acme.ProductSelling.Web.Pages.Account
{
    [Authorize]
    public class AddressesModel : ProductSellingPageModel
    {
        private readonly IProfileAppService _profileAppService;
        private readonly IAddressAppService _addressAppService;
        [BindProperty] 
        public UpdateShippingAddressDto ShippingAddressInput { get; set; }
        public List<AddressDto> CurrentShippingAddress { get; set; }
        [TempData]
        public string SuccessMessage { get; set; }
        [TempData]
        public string ErrorMessage { get; set; }

        public AddressesModel(IProfileAppService profileAppService, IAddressAppService addressAppService)
        {
            _profileAppService = profileAppService;
            _addressAppService = addressAppService;
        }

        public async Task OnGetAsync()
        {
            var addresses = await _addressAppService.GetListAsync();

            CurrentShippingAddress = addresses;
            //ShippingAddressInput = new UpdateShippingAddressDto
            //{
            //    FullAddress = profile.ShippingAddress
            //};
        }

        public async Task<IActionResult> OnPostUpdateAddressAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            try
            {
                await _profileAppService.UpdateShippingAddressAsync(ShippingAddressInput);
                Alerts.Success(L["Account:ProfileUpdatedSuccessfully"]);
            }
            catch (Exception ex)
            {
                ErrorMessage = ex.Message;
                return Page();
            }

            return RedirectToPage();
        }
    }
}
