using Acme.ProductSelling.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Text;
using System.Threading.Tasks;
using Volo.Abp;
using Volo.Abp.Domain.Repositories.EntityFrameworkCore;
using Volo.Abp.EntityFrameworkCore;
using Volo.Abp.Users;

namespace Acme.ProductSelling.Users
{
    public class CustomerRepository : EfCoreRepository<ProductSellingDbContext, Customer, Guid>, ICustomerRepository
    {
        private readonly ICurrentUser _currentUser;
        public CustomerRepository(IDbContextProvider<ProductSellingDbContext> dbContextProvider, ICurrentUser currentUser) : base(dbContextProvider)
        {
            _currentUser = currentUser;
        }

        public async Task<Customer> GetCurrentCustomerAsync()
        {
            var queryable = await GetQueryableAsync();
            var customer = await AsyncExecuter.FirstOrDefaultAsync(
                queryable
                    .Include(c => c.ShippingAddresses)
                    .Where(c => c.AppUserId == _currentUser.GetId()));

            return customer ?? throw new UserFriendlyException("Customer not found.");
        }
    }
}
