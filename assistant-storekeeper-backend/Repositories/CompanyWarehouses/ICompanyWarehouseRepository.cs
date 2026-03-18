using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using assistant_storekeeper_backend.Models;

namespace assistant_storekeeper_backend.Repositories.CompanyWarehouses
{
    public interface ICompanyWarehouseRepository
    {
        Task<CompanyWarehouse?> GetCompanyWarehouseById(int id, CancellationToken cancellationToken = default);

        Task<IEnumerable<CompanyWarehouse>> GetAllCompanyWarehouses(
            int? page = 1,
            int? pageSize = 10,
            string? search = null,
            bool? isAscending = true,
            string? sortBy = "Id",
            CancellationToken cancellationToken = default);

        Task<CompanyWarehouse> CreateCompanyWarehouse(CompanyWarehouse companyWarehouse, CancellationToken cancellationToken = default);
        Task<CompanyWarehouse> UpdateCompanyWarehouse(CompanyWarehouse companyWarehouse, CancellationToken cancellationToken = default);
        Task DeleteCompanyWarehouse(CompanyWarehouse companyWarehouse, CancellationToken cancellationToken = default);
    }
}
