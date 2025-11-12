using System;
using Volo.Abp.Identity;
using Volo.Abp.Users;

namespace Acme.ProductSelling.Users
{
    public class AppUser : IdentityUser, IUser
    {
        // Store assignment for staff (sellers, cashiers, warehouse)
        public Guid? AssignedStoreId { get; protected set; }

        // Customer relationship (only for customer accounts)
        public virtual Customer Customer { get; protected set; }

        protected AppUser() { }

        public AppUser(Guid id, string userName, string email, Guid? tenantId = null)
           : base(id, userName, email, tenantId)
        {
        }

        // Store assignment methods (for staff)
        public void AssignToStore(Guid? storeId)
        {
            AssignedStoreId = storeId;
        }

        public void UnassignFromStore()
        {
            AssignedStoreId = null;
        }

        public bool IsAssignedToStore(Guid storeId)
        {
            return AssignedStoreId.HasValue && AssignedStoreId.Value == storeId;
        }

        public bool IsStaffMember()
        {
            return AssignedStoreId.HasValue;
        }

        // Customer relationship methods
        public void SetCustomer(Customer customer)
        {
            if (customer == null)
                throw new ArgumentNullException(nameof(customer));

            if (AssignedStoreId.HasValue)
                throw new InvalidOperationException("Staff users cannot have customer profiles");

            Customer = customer;
        }

        public void RemoveCustomer()
        {
            Customer = null;
        }

        public bool IsCustomer()
        {
            return Customer != null;
        }

        // User type determination
        public UserType GetUserType()
        {
            if (Customer != null)
                return UserType.Customer;
            if (AssignedStoreId.HasValue)
                return UserType.Staff;
            return UserType.Admin;
        }
    }

    public enum UserType
    {
        Admin,
        Staff,
        Customer
    }
}