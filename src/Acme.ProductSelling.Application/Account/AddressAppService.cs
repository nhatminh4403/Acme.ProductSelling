using Acme.ProductSelling.Account.Dtos;
using Acme.ProductSelling.Account.Services;
using Acme.ProductSelling.Mappings;
using Acme.ProductSelling.Users;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Text;
using System.Threading.Tasks;
using Volo.Abp;
using Volo.Abp.Users;

namespace Acme.ProductSelling.Account
{
    public class AddressAppService : ProductSellingAppService, IAddressAppService
    {
        private readonly ICustomerRepository _customerRepository;
        private readonly ICurrentUser _currentUser;
        private readonly AddressToAddressDtoMapper _mapper;

        public AddressAppService(ICustomerRepository customerRepository, ICurrentUser currentUser, AddressToAddressDtoMapper mapper)
        {
            _customerRepository = customerRepository;
            _currentUser = currentUser;
            _mapper = mapper;
        }

        public async Task<AddressDto> CreateAsync(CreateAddressDto input)
        {
            var customer = await _customerRepository.GetCurrentCustomerAsync();

            var address = customer.AddAddress(id: GuidGenerator.Create(), fullAddress: input.FullAddress);
            await _customerRepository.UpdateAsync(customer, autoSave: true);
            return _mapper.Map(address);
        }

        public async Task DeleteAsync(Guid id)
        {
            var customer = await _customerRepository.GetCurrentCustomerAsync();
            customer.RemoveAddress(id);
            await _customerRepository.UpdateAsync(customer, autoSave: true);
        }

        public async Task<List<AddressDto>> GetListAsync()
        {
            var customer = await _customerRepository.GetCurrentCustomerAsync();
            return customer.ShippingAddresses
                .Select(a => _mapper.Map(a))
                .ToList();
        }

        public async Task SetDefaultAsync(Guid id)
        {
            var customer = await _customerRepository.GetCurrentCustomerAsync();
            customer.SetDefaultAddress(id);
            await _customerRepository.UpdateAsync(customer, autoSave: true);
        }

        public async Task<AddressDto> UpdateAsync(CreateAddressDto input)
        {
            var customer = await _customerRepository.GetCurrentCustomerAsync();
            var address = customer.ShippingAddresses.FirstOrDefault(a => a.Id == input.Id)
                ?? throw new UserFriendlyException("Address not found.");

            address.UpdateAddress(input.FullAddress, address.IsDefaultAddress);
            //address.Update(input.Label, input.FullAddress);
            await _customerRepository.UpdateAsync(customer, autoSave: true);
            return _mapper.Map(address);
        }

    }
}
