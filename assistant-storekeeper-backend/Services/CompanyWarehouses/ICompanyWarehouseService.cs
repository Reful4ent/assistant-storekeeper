using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using assistant_storekeeper_backend.Models;

namespace assistant_storekeeper_backend.Services.CompanyWarehouses
{
    public interface ICompanyWarehouseService
    {
        Task<CompanyWarehouse> GetCompanyWarehouseById(int id, CancellationToken cancellationToken = default);
        Task<IEnumerable<CompanyWarehouse>> GetAllCompanyWarehouses(
            int? page, 
            int? pageSize, 
            string? search, 
            bool? isAscending, 
            string? sortBy, 
            CancellationToken cancellationToken = default);
        Task<CompanyWarehouse> CreateCompanyWarehouse(CompanyWarehouse companyWarehouse, CancellationToken cancellationToken = default);
        Task<CompanyWarehouse> UpdateCompanyWarehouse(int id, CompanyWarehouse companyWarehouse, CancellationToken cancellationToken = default);
        Task DeleteCompanyWarehouse(int id, CancellationToken cancellationToken = default);
    }
}
