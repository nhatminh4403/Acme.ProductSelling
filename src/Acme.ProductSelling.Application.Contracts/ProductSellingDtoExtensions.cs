using Volo.Abp.Threading;
namespace Acme.ProductSelling;

public static class ProductSellingDtoExtensions
{
    private static readonly OneTimeRunner OneTimeRunner = new OneTimeRunner();
    public static void Configure()
    {
        OneTimeRunner.Run(() =>
        {
            /* You can add extension properties to DTOs
             * defined in the depended modules.
             *
             * Example:
             *
             * ObjectExtensionManager.Instance
             *   .AddOrUpdateProperty<IdentityRoleDto, string>("Title");
             *
             * See the documentation for more:
             * https://docs.abp.io/en/abp/latest/Object-Extensions
             */

            //ObjectExtensionManager.Instance.AddOrUpdateProperty<IdentityUserDto, DateTime?>(StaffExtraProperties.DateOfBirth);

            //ObjectExtensionManager.Instance.AddOrUpdateProperty<IdentityUserDto, UserGender>(StaffExtraProperties.Gender);

        });
    }
}
