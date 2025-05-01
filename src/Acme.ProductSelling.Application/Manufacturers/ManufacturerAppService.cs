using Acme.ProductSelling.Permissions;
using Microsoft.AspNetCore.Authorization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Application.Services;
using Volo.Abp.Domain.Repositories;

namespace Acme.ProductSelling.Manufacturers
{
    public class ManufacturerAppService : CrudAppService<Manufacturer, ManufacturerDto,
        Guid, PagedAndSortedResultRequestDto, CreateUpdateManufacturerDto>, IManufacturerAppService
    {

        private readonly IRepository<Manufacturer, Guid> _manufacturerRepository;
        public ManufacturerAppService(IRepository<Manufacturer, Guid> manufacturerRepository) : base(manufacturerRepository)
        {
            _manufacturerRepository = manufacturerRepository;
            GetPolicyName = ProductSellingPermissions.Manufacturers.Default;
            CreatePolicyName = ProductSellingPermissions.Manufacturers.Create;
            UpdatePolicyName = ProductSellingPermissions.Manufacturers.Edit;
            DeletePolicyName = ProductSellingPermissions.Manufacturers.Delete;
        }

        [AllowAnonymous]
        public async Task<ListResultDto<ManufacturerLookupDto>> GetManufacturerLookupAsync()
        {
            var manufacturers = await _manufacturerRepository.GetListAsync();
            return new ListResultDto<ManufacturerLookupDto>(
                ObjectMapper.Map<List<Manufacturer>, List<ManufacturerLookupDto>>(manufacturers)
            );
        }
    }
}
