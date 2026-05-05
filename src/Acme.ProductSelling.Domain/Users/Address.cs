using Microsoft.AspNetCore.Server.IISIntegration;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;
using Volo.Abp;
using Volo.Abp.Domain.Entities;

namespace Acme.ProductSelling.Users
{
    public class Address : Entity<Guid>
    {
        public Guid CustomerId { get; set; }

        public string FullAddress { get; set; }
        public bool IsDefaultAddress { get; set; }

        protected Address()
        {
        }

        public Address(Guid id, Guid customerId, string fullAddress, bool isDefaultAddress) : base(id)
        {
            CustomerId = customerId;
            FullAddress = fullAddress;
            IsDefaultAddress = isDefaultAddress;
        }

        public void UpdateAddress(string fullAddress, bool isDefaultAddress)
        {
            FullAddress = Check.NotNullOrWhiteSpace(fullAddress,FullAddress,maxLength:AddressConsts.MaxAddressLength);
            IsDefaultAddress = isDefaultAddress;
        }
        public void SetAsDefault() => IsDefaultAddress = true;
        public void UnsetDefault() => IsDefaultAddress = false;
    }
}
