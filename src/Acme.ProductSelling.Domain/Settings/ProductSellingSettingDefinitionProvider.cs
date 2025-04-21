using Volo.Abp.Settings;

namespace Acme.ProductSelling.Settings;

public class ProductSellingSettingDefinitionProvider : SettingDefinitionProvider
{
    public override void Define(ISettingDefinitionContext context)
    {
        //Define your own settings here. Example:
        //context.Add(new SettingDefinition(ProductSellingSettings.MySetting1));
    }
}
