using Acme.ProductSelling.Account.Dtos;
using Acme.ProductSelling.Account.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;

namespace Acme.ProductSelling.Web.Pages.Account
{
    [Authorize]
    public class SecurityModel : ProductSellingPageModel
    {
        private readonly IProfileAppService _profileAppService;

        [BindProperty]
        public ChangePasswordDto PasswordInput { get; set; }

        [TempData]
        public string ErrorMessage { get; set; }

        public SecurityModel(IProfileAppService profileAppService)
        {
            _profileAppService = profileAppService;
        }

        public void OnGet()
        {
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            try
            {
                await _profileAppService.ChangePasswordAsync(PasswordInput);
                Alerts.Success(L["Account:PasswordChangedSuccessfully"]);
                return RedirectToPage("/Account/Index");
            }
            catch (Exception ex)
            {
                ErrorMessage = ex.Message;
                return Page();
            }
        }
    }
}
