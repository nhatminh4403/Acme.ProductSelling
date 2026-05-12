using Acme.ProductSelling.Account.Dtos;
using Acme.ProductSelling.Account.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Acme.ProductSelling.Web.Pages.Account
{
    [Authorize]
    public class AddressesModel : ProductSellingPageModel
    {
        private readonly IAddressAppService _addressAppService;
        [BindProperty]
        public UpdateShippingAddressDto UpdateAddressInput { get; set; }
        [BindProperty]
        public Guid? EditingAddressId { get; set; }
        public List<AddressDto> ShippingAddressList { get; set; }
        [TempData]
        public string ErrorMessage { get; set; }
        [BindProperty]
        public CreateAddressDto NewShippingAddressInput { get; set; }
        public AddressesModel(IAddressAppService addressAppService)
        {
            _addressAppService = addressAppService;
        }

        public async Task OnGetAsync(Guid? editId)
        {
            await HydrateAddressInputAsync();

            if (editId.HasValue)
            {
                EditingAddressId = editId;
                var address = await _addressAppService.GetAsync(editId.Value);
                UpdateAddressInput = new UpdateShippingAddressDto
                {
                    FullAddress = address.FullAddress
                };

            }
        }

        public async Task<IActionResult> OnPostSetDefaultAsync(Guid id)
        {
            try
            {
                await _addressAppService.SetDefaultAsync(id);
                Alerts.Success(L["Account:SetDefaultSuccessfully"]);
                return RedirectToPage("/Account/Addresses");
            }
            catch (Exception ex)
            {
                ErrorMessage = ex.Message;
                await HydrateAddressInputAsync();
                return Page();
            }
        }
        public async Task<IActionResult> OnPostDeleteAddressAsync(Guid id)
        {
            try
            {
                await _addressAppService.DeleteAsync(id);
                Alerts.Success(L["Account:ProfileUpdatedSuccessfully"]);
                return RedirectToPage("/Account/Addresses");
            }
            catch (Exception ex)
            {
                ErrorMessage = ex.Message;
                await HydrateAddressInputAsync();
                return Page();
            }
        }
        public async Task<IActionResult> OnPostAddNewAddressAsync()
        {
            IgnoreModelStateErrors("add");
            if (!ModelState.IsValid)
            {
                Logger.LogWarning("--- BẮT ĐẦU KIỂM TRA MODELSTATE LỖI ---");
                foreach (var modelStateKey in ModelState.Keys)
                {
                    var modelStateVal = ModelState[modelStateKey];
                    foreach (var error in modelStateVal.Errors)
                    {
                        Logger.LogWarning($"Key: {modelStateKey} | Error: {error.ErrorMessage}");
                    }
                }
                Logger.LogWarning("--- KẾT THÚC KIỂM TRA MODELSTATE LỖI ---");
                await HydrateAddressInputAsync();
                return Page();

            }

            try
            {
                await _addressAppService.CreateAsync(NewShippingAddressInput);
                Alerts.Success(L["Account:ProfileUpdatedSuccessfully"]);
                return RedirectToPage("/Account/Addresses");
            }
            catch (Exception ex)
            {
                ErrorMessage = ex.Message;
                await HydrateAddressInputAsync();
                return Page();
            }
        }
        public async Task<IActionResult> OnPostUpdateAddressAsync(Guid id)
        {
            IgnoreModelStateErrors("update");
            if (!ModelState.IsValid)
            {
                Logger.LogWarning("--- BẮT ĐẦU KIỂM TRA MODELSTATE LỖI ---");
                foreach (var modelStateKey in ModelState.Keys)
                {
                    var modelStateVal = ModelState[modelStateKey];
                    foreach (var error in modelStateVal.Errors)
                    {
                        Logger.LogWarning($"Key: {modelStateKey} | Error: {error.ErrorMessage}");
                    }
                }
                Logger.LogWarning("--- KẾT THÚC KIỂM TRA MODELSTATE LỖI ---");
                await HydrateAddressInputAsync();
                EditingAddressId = id;
                return Page();
            }

            try
            {
                UpdateAddressInput.Id = id;
                await _addressAppService.UpdateAsync(UpdateAddressInput);
                Alerts.Success(L["Account:ProfileUpdatedSuccessfully"]);
                return RedirectToPage("/Account/Addresses");
            }
            catch (Exception ex)
            {
                ErrorMessage = ex.Message;
                await HydrateAddressInputAsync();
                EditingAddressId = id;
                return Page();
            }
        }
        private async Task HydrateAddressInputAsync()
        {
            var addressList = await _addressAppService.GetListAsync();
            ShippingAddressList = addressList;

        }
        private void IgnoreModelStateErrors(string method)
        {
            if (method != "update")
            {
                ModelState.Remove(nameof(NewShippingAddressInput.FullAddress));

            }
            if (method != "add")
            {
                ModelState.Remove(nameof(UpdateAddressInput.FullAddress));

            }
        }
    }
}
