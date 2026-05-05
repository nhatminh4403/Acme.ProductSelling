using Acme.ProductSelling.Account.Dtos;
using Acme.ProductSelling.Account.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Serilog;
using System;
using System.Linq;
using System.Threading.Tasks;
using Volo.Abp.Identity;

namespace Acme.ProductSelling.Web.Pages.Account
{
    [Authorize]
    public class IndexModel : ProductSellingPageModel
    {
        private readonly IdentityUserManager _userManager;
        private readonly IProfileAppService _profileAppService;

        public CustomerProfileDto CustomerProfile { get; set; }
        
        [BindProperty(Name = "ProfileInput")]   
        public UpdateProfileDto ProfileInput { get; set; }

        [TempData]
        public string SuccessMessage { get; set; }

        [TempData]
        public string ErrorMessage { get; set; }

        public IndexModel(IdentityUserManager userManager, IProfileAppService profileAppService)
        {
            _userManager = userManager;
            _profileAppService = profileAppService;
        }

        public async Task OnGetAsync()
        {
            await HydrateProfileInputAsync();
        }

        public async Task<IActionResult> OnPostUpdateProfileAsync()
        {
            if (!ModelState.IsValid)
            {
                await HydrateProfileInputAsync();
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
                await HydrateProfileInputAsync();
                return Page();
            }

            return RedirectToPage();
        }

        private async Task HydrateProfileInputAsync()
        {
            var profile = await _profileAppService.GetAsync();
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
