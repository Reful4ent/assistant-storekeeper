using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using assistant_storekeeper_backend.Data;
using assistant_storekeeper_backend.Exceptions;
using assistant_storekeeper_backend.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query.SqlExpressions;
using System;

namespace assistant_storekeeper_backend.Repositories.CompanyWarehouses
{
    public class CompanyWarehouseRepository : ICompanyWarehouseRepository
    {
        private readonly ApplicationDbContext _context;

        public CompanyWarehouseRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<CompanyWarehouse?> GetCompanyWarehouseById(int id, CancellationToken cancellationToken = default)
        {
            return await _context.CompanyWarehouses
                .Include(c => c.CompanyWarehouseNomenclatures)
                .ThenInclude(c => c.Nomenclature)
                .FirstOrDefaultAsync(c => c.Id == id, cancellationToken);
        }

        public async Task<(IEnumerable<CompanyWarehouse> data, int total, int totalPages)> GetAllCompanyWarehouses(
            int? page = 1,
            int? pageSize = 10,
            string? search = null,
            bool? isAscending = true,
            string? sortBy = "Id",
            CancellationToken cancellationToken = default)
        {
            var query = _context.CompanyWarehouses
                .Include(c => c.CompanyWarehouseNomenclatures)
                .ThenInclude(c => c.Nomenclature)
                .AsQueryable();

            if (!string.IsNullOrEmpty(search))
            {
                query = query.Where(c => EF.Functions.ILike(c.Name, "%" + search.Trim() + "%"));
            }

            if (sortBy == "Name")
            {
                query = isAscending == true
                    ? query.OrderBy(c => c.Name)
                    : query.OrderByDescending(c => c.Name);
            }
            else
            {
                query = isAscending == true
                    ? query.OrderBy(c => c.Id)
                    : query.OrderByDescending(c => c.Id);
            }

            var total = await query.CountAsync(cancellationToken);
            var totalPages = (int)Math.Ceiling((double)total / (pageSize ?? 10));
            var data = await query
                .Skip(((page ?? 1) - 1) * (pageSize ?? 10))
                .Take(pageSize ?? 10)
                .ToListAsync(cancellationToken);

            return (data, total, totalPages);
        }

        public async Task<CompanyWarehouse> CreateCompanyWarehouse(CompanyWarehouse companyWarehouse, CancellationToken cancellationToken = default)
        {
            await _context.CompanyWarehouses.AddAsync(companyWarehouse, cancellationToken);
            await _context.SaveChangesAsync(cancellationToken);
            return companyWarehouse;
        }

        public async Task<CompanyWarehouse> UpdateCompanyWarehouse(CompanyWarehouse companyWarehouse, CancellationToken cancellationToken = default)
        {
            _context.CompanyWarehouses.Update(companyWarehouse);
            await _context.SaveChangesAsync(cancellationToken);
            return companyWarehouse;
        }

        public async Task DeleteCompanyWarehouse(CompanyWarehouse companyWarehouse, CancellationToken cancellationToken = default)
        {
            _context.CompanyWarehouses.Remove(companyWarehouse);
            await _context.SaveChangesAsync(cancellationToken);
        }
    }
}
