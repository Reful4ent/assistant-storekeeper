using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using assistant_storekeeper_backend.Models;

namespace assistant_storekeeper_backend.Services.CompanyWareHouseNomenclatures
{
    public interface ICompanyWareHouseNomenclatureService
    {
        Task<CompanyWarehouseNomenclature?> GetCompanyWarehouseNomenclatureById(
            int id, 
            CancellationToken cancellationToken = default);
        Task<CompanyWarehouseNomenclature?> GetCompanyWarehouseNomenclatureByCompanyWarehouseIdAndNomenclatureId(
            int companyWarehouseId,
            int nomenclatureId,
            CancellationToken cancellationToken = default);
        Task<IEnumerable<CompanyWarehouseNomenclature>> GetAllCompanyWarehouseNomenclatures(
            int companyWarehouseId, 
            int? page, 
            int? pageSize, 
            string? search, 
            bool? isAscending, 
            string? sortBy, 
            CancellationToken cancellationToken = default);
        Task<CompanyWarehouseNomenclature> CreateCompanyWarehouseNomenclature(
            CompanyWarehouseNomenclature companyWarehouseNomenclature, 
            CancellationToken cancellationToken = default);
        Task<CompanyWarehouseNomenclature> UpdateCompanyWarehouseNomenclature(int id, 
            CompanyWarehouseNomenclature companyWarehouseNomenclature,
            CancellationToken cancellationToken = default);
        Task DeleteCompanyWarehouseNomenclature(
            int id,
            CancellationToken cancellationToken = default);
    }
}