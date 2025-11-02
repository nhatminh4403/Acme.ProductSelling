using Acme.ProductSelling.Account;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;
using Volo.Abp.Identity;

namespace Acme.ProductSelling.Web.Pages.Account
{
    [Authorize]
    public class IndexModel : ProductSellingPageModel
    {
        private readonly IdentityUserManager _userManager;
        private readonly ProductSelling.Account.IProfileAppService _profileAppService;
        [BindProperty]
        public ProductSelling.Account.UpdateProfileDto ProfileInput { get; set; }

        [BindProperty]
        public ChangePasswordDto PasswordInput { get; set; }

        [TempData]
        public string SuccessMessage { get; set; }

        [TempData]
        public string ErrorMessage { get; set; }


        public IndexModel(IdentityUserManager userManager, ProductSelling.Account.IProfileAppService iProfileAppService)
        {
            _userManager = userManager;
            _profileAppService = iProfileAppService;
        }

        public async Task OnGetAsync()
        {
            ProfileInput = await _profileAppService.GetAsync();
        }

        public async Task<IActionResult> OnPostUpdateProfileAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            try
            {
                await _profileAppService.UpdateAsync(ProfileInput);
                Alerts.Success(L["Account:ProfileUpdatedSuccessfully"]);
            }
            catch (Exception ex)
            {
                ErrorMessage = ex.Message;
            }

            await _profileAppService.UpdateAsync(ProfileInput);

            return RedirectToPage();
        }
        public async Task<IActionResult> OnPostChangePasswordAsync()
        {
            // Clear profile validation errors
            ModelState.Remove("ProfileInput.UserName");
            ModelState.Remove("ProfileInput.Email");
            // ... clear other profile fields

            if (!ModelState.IsValid)
            {
                ProfileInput = await _profileAppService.GetAsync();
                return Page();
            }

            try
            {
                await _profileAppService.ChangePasswordAsync(PasswordInput);
                Alerts.Success(L["Account:PasswordChangedSuccessfully"]);
            }
            catch (Exception ex)
            {
                ErrorMessage = ex.Message;
                ProfileInput = await _profileAppService.GetAsync();
                return Page();
            }

            return RedirectToPage();
        }
    }
}
