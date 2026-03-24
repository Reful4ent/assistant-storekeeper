using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using assistant_storekeeper_backend.Exceptions;
using assistant_storekeeper_backend.Models;
using assistant_storekeeper_backend.Repositories.CompanyWarehouses;

namespace assistant_storekeeper_backend.Services.CompanyWarehouses
{
    public class CompanyWarehouseService : ICompanyWarehouseService
    {
        private readonly ICompanyWarehouseRepository _companyWarehouseRepository;

        public CompanyWarehouseService(ICompanyWarehouseRepository companyWarehouseRepository)
        {
            _companyWarehouseRepository = companyWarehouseRepository;
        }

        public async Task<CompanyWarehouse> GetCompanyWarehouseById(int id, CancellationToken cancellationToken = default)
        {
            var companyWarehouse = await _companyWarehouseRepository.GetCompanyWarehouseById(id, cancellationToken);
            if (companyWarehouse == null)
            {
                throw new NotFoundException("Company warehouse not found");
            }
            return companyWarehouse;
        }

        public async Task<(IEnumerable<CompanyWarehouse> data, int total, int totalPages)> GetAllCompanyWarehouses(
            int? page, 
            int? pageSize, 
            string? search, 
            bool? isAscending, 
            string? sortBy, 
            CancellationToken cancellationToken = default)
        {
            return await _companyWarehouseRepository.GetAllCompanyWarehouses(page, pageSize, search, isAscending, sortBy, cancellationToken);
        }

        public async Task<CompanyWarehouse> CreateCompanyWarehouse(CompanyWarehouse companyWarehouse, CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrWhiteSpace(companyWarehouse.Name))
            {
                throw new BadRequestException("Name is required and cannot be empty");
            }

            companyWarehouse.Name = companyWarehouse.Name.Trim();
            
            return await _companyWarehouseRepository.CreateCompanyWarehouse(companyWarehouse, cancellationToken);
        }

        public async Task<CompanyWarehouse> UpdateCompanyWarehouse(int id, CompanyWarehouse companyWarehouse, CancellationToken cancellationToken = default)
        {
            var existingCompanyWarehouse = await _companyWarehouseRepository.GetCompanyWarehouseById(id, cancellationToken);
            if (existingCompanyWarehouse == null)
            {
                throw new NotFoundException("Company warehouse not found");
            }
            
            if (string.IsNullOrWhiteSpace(companyWarehouse.Name))
            {
                throw new BadRequestException("Name is required and cannot be empty");
            }

            existingCompanyWarehouse.Name = companyWarehouse.Name.Trim();

            return await _companyWarehouseRepository.UpdateCompanyWarehouse(existingCompanyWarehouse, cancellationToken);
        }

        public async Task DeleteCompanyWarehouse(int id, CancellationToken cancellationToken = default)
        {
            var existingCompanyWarehouse = await _companyWarehouseRepository.GetCompanyWarehouseById(id, cancellationToken);
            if (existingCompanyWarehouse == null)
            {
                throw new NotFoundException("Company warehouse not found");
            }

            await _companyWarehouseRepository.DeleteCompanyWarehouse(existingCompanyWarehouse, cancellationToken);
        }
    }
}
