﻿using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RequestLocalization;
using Microsoft.Extensions.DependencyInjection;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using Volo.Abp.AspNetCore.Mvc;
using Volo.Abp.AspNetCore.Mvc.UI.Theme.LeptonXLite.Languages;
using Volo.Abp.Localization;
namespace Acme.ProductSelling.Web.Themes.LeptonXLite.Components.Toolbar.LanguageSwitch
{
    public class LanguageSwitchViewComponent : AbpViewComponent
    {
        protected ILanguageProvider LanguageProvider { get; }
        public LanguageSwitchViewComponent(ILanguageProvider languageProvider)
        {
            LanguageProvider = languageProvider;
        }
        public virtual async Task<IViewComponentResult> InvokeAsync()
        {
            var languages = await LanguageProvider.GetLanguagesAsync();
            var currentLanguage = languages.FindByCulture(
                CultureInfo.CurrentCulture.Name,
                CultureInfo.CurrentUICulture.Name
            );

            if (currentLanguage == null)
            {
                var abpRequestLocalizationOptionsProvider = HttpContext.RequestServices.GetRequiredService<IAbpRequestLocalizationOptionsProvider>();
                var localizationOptions = await abpRequestLocalizationOptionsProvider.GetLocalizationOptionsAsync();
                if (localizationOptions.DefaultRequestCulture != null)
                {
                    currentLanguage = new LanguageInfo(
                        localizationOptions.DefaultRequestCulture.Culture.Name,
                        localizationOptions.DefaultRequestCulture.UICulture.Name,
                        localizationOptions.DefaultRequestCulture.UICulture.DisplayName);
                }
                else
                {
                    currentLanguage = new LanguageInfo(
                        CultureInfo.CurrentCulture.Name,
                        CultureInfo.CurrentUICulture.Name,
                        CultureInfo.CurrentUICulture.DisplayName);
                }
            }
            var model = new ThemeLanguageInfo
            {
                CurrentLanguage = currentLanguage,
                Languages = languages.Where(l => l != currentLanguage).ToList(),
            };

            return View("~/Themes/LeptonXLite/Components/Toolbar/LanguageSwitch/Default.cshtml", model);
        }
    }

}
