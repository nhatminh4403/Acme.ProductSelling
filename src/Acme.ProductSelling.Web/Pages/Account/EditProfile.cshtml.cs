using Acme.ProductSelling.Account.Dtos;
using Acme.ProductSelling.Account.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Threading.Tasks;

namespace Acme.ProductSelling.Web.Pages.Account
{
    [Authorize]
    public class EditProfileModel : ProductSellingPageModel
    {
        private readonly IProfileAppService _profileAppService;

        [BindProperty]
        public UpdateProfileDto ProfileInput { get; set; }

        [TempData]
        public string ErrorMessage { get; set; }

        public EditProfileModel(IProfileAppService profileAppService)
        {
            _profileAppService = profileAppService;
        }

        public async Task OnGetAsync()
        {
            await HydrateProfileInputAsync();
        }

        public async Task<IActionResult> OnPostAsync()
        {
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

                await HydrateProfileInputAsync();
                return Page();
            }

            try
            {
                await _profileAppService.UpdateAsync(ProfileInput);
                Alerts.Success(L["Account:ProfileUpdatedSuccessfully"]);
                return RedirectToPage("/Account/Index");
            }
            catch (Exception ex)
            {
                ErrorMessage = ex.Message;
                await HydrateProfileInputAsync();
                return Page();
            }
        }

        private async Task HydrateProfileInputAsync()
        {
            var profile = await _profileAppService.GetAsync();
            if (ProfileInput == null) 
            {
                ProfileInput = new UpdateProfileDto
                {
                    UserName = profile.UserName,
                    Email = profile.Email,
                    Name = profile.Name,
                    Surname = profile.Surname,
                    PhoneNumber = profile.PhoneNumber,
                    DateOfBirth = profile.DateOfBirth,
                    Gender = profile.Gender
                };
            }
        }
    }
}
