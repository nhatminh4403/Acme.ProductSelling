using System;
using Volo.Abp.Identity;
using Volo.Abp.Users;

namespace Acme.ProductSelling.Users
{
    public class AppUser : IdentityUser, IUser
    {
        public DateTime? DateOfBirth { get; set; }
        public UserGender Gender { get; set; }
        public Guid? AssignedStoreId { get; set; }

        protected AppUser() { }

        public AppUser(Guid id, string userName, string email, Guid? tenantId = null)
           : base(id, userName, email, tenantId)
        {
        }

        public void AssignToStore(Guid? storeId)
        {
            AssignedStoreId = storeId;
        }
    }
}
