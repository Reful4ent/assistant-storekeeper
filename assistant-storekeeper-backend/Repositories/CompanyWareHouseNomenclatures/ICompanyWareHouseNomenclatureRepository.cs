using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using assistant_storekeeper_backend.Models;

namespace assistant_storekeeper_backend.Repositories.CompanyWareHouseNomenclatures
{
    public interface ICompanyWareHouseNomenclatureRepository
    {
        Task<CompanyWarehouseNomenclature?> GetCompanyWarehouseNomenclatureById(
            int id, 
            CancellationToken cancellationToken = default);
        Task<IEnumerable<CompanyWarehouseNomenclature>> GetAllCompanyWarehouseNomenclatures(
            int companyWarehouseId,
            int? page = 1,
            int? pageSize = 10,
            string? search = null,
            bool? isAscending = true,
            string? sortBy = "NomenclatureName",
            CancellationToken cancellationToken = default);
        Task<CompanyWarehouseNomenclature> CreateCompanyWarehouseNomenclature(
            CompanyWarehouseNomenclature companyWarehouseNomenclature,
            CancellationToken cancellationToken = default);
        Task<CompanyWarehouseNomenclature> UpdateCompanyWarehouseNomenclature(
            CompanyWarehouseNomenclature companyWarehouseNomenclature,
            CancellationToken cancellationToken = default);
        Task DeleteCompanyWarehouseNomenclature(
            CompanyWarehouseNomenclature companyWarehouseNomenclature,
            CancellationToken cancellationToken = default);
    }
}