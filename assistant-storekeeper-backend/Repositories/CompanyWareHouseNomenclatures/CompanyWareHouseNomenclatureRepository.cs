using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using assistant_storekeeper_backend.Models;
using Microsoft.EntityFrameworkCore;
using assistant_storekeeper_backend.Data;
using Microsoft.EntityFrameworkCore.Query;
using System.Linq;
using Microsoft.EntityFrameworkCore.Query.SqlExpressions;

namespace assistant_storekeeper_backend.Repositories.CompanyWareHouseNomenclatures
{
    public class CompanyWareHouseNomenclatureRepository : ICompanyWareHouseNomenclatureRepository
    {
        private readonly ApplicationDbContext _context;

        public CompanyWareHouseNomenclatureRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<CompanyWarehouseNomenclature?> GetCompanyWarehouseNomenclatureById(
            int id, 
            CancellationToken cancellationToken = default)
        {
            return await _context.CompanyWarehouseNomenclatures.FirstOrDefaultAsync(c => c.Id == id, cancellationToken);
        }

        public async Task<CompanyWarehouseNomenclature?> GetCompanyWarehouseNomenclatureByCompanyWarehouseIdAndNomenclatureId(
            int companyWarehouseId,
            int nomenclatureId,
            CancellationToken cancellationToken = default)
        {
            return await _context.CompanyWarehouseNomenclatures.FirstOrDefaultAsync(
                c => c.CompanyWarehouseId == companyWarehouseId && c.NomenclatureId == nomenclatureId, 
                cancellationToken);
        }

        public async Task<IEnumerable<CompanyWarehouseNomenclature>> GetAllCompanyWarehouseNomenclatures(
            int companyWarehouseId, 
            int? page = 1, 
            int? pageSize = 10, 
            string? search = null, 
            bool? isAscending = true, 
            string? sortBy = "NomenclatureName", 
            CancellationToken cancellationToken = default)
        {
            var query = _context.CompanyWarehouseNomenclatures
                .Include(c => c.CompanyWarehouse)
                .Include(c => c.Nomenclature)
                .AsQueryable();

            query = query.Where(c => c.CompanyWarehouseId == companyWarehouseId);

            if (!string.IsNullOrEmpty(search))
            {
                query = query.Where(c => EF.Functions.ILike(c.Nomenclature.Name, "%" + search.Trim() + "%"));
            }
            if (sortBy == "NomenclatureName")
            {
                query = isAscending == true 
                    ? query.OrderBy(c => c.Nomenclature.Name) 
                    : query.OrderByDescending(c => c.Nomenclature.Name);
            }
            else if (sortBy == "Quantity")
            {
                query = isAscending == true 
                    ? query.OrderBy(c => c.Quantity) 
                    : query.OrderByDescending(c => c.Quantity);
            }
            else
            {
                query = query.OrderBy(c => c.Id);
            }

            return await query
                .Skip(((page ?? 1) - 1) * (pageSize ?? 10))
                .Take(pageSize ?? 10)
                .ToListAsync(cancellationToken);
        }

        public async Task<CompanyWarehouseNomenclature> CreateCompanyWarehouseNomenclature(
            CompanyWarehouseNomenclature companyWarehouseNomenclature, 
            CancellationToken cancellationToken = default)
        {
            await _context.CompanyWarehouseNomenclatures.AddAsync(companyWarehouseNomenclature, cancellationToken);
            await _context.SaveChangesAsync(cancellationToken);
            return companyWarehouseNomenclature;
        }

        public async Task<CompanyWarehouseNomenclature> UpdateCompanyWarehouseNomenclature(
            CompanyWarehouseNomenclature companyWarehouseNomenclature, 
            CancellationToken cancellationToken = default)
        {
            _context.CompanyWarehouseNomenclatures.Update(companyWarehouseNomenclature);
            await _context.SaveChangesAsync(cancellationToken);
            return companyWarehouseNomenclature;
        }

        public async Task DeleteCompanyWarehouseNomenclature(
            CompanyWarehouseNomenclature companyWarehouseNomenclature, 
            CancellationToken cancellationToken = default)
        {
            _context.CompanyWarehouseNomenclatures.Remove(companyWarehouseNomenclature);
            await _context.SaveChangesAsync(cancellationToken);
        }
    }
}