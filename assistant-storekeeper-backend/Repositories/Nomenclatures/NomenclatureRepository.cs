using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using assistant_storekeeper_backend.Models;
using Microsoft.EntityFrameworkCore;
using assistant_storekeeper_backend.Data;
using System.Linq;
using Microsoft.EntityFrameworkCore.Query.SqlExpressions;
using System;

namespace assistant_storekeeper_backend.Repositories.Nomenclatures
{
    public class NomenclatureRepository : INomenclatureRepository
    {
        private readonly ApplicationDbContext _context;

        public NomenclatureRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<Nomenclature?> GetNomenclatureById(int id, CancellationToken cancellationToken = default)
        {
            return await _context.Nomenclatures.FirstOrDefaultAsync(c => c.Id == id, cancellationToken);
        }

        public async Task<(IEnumerable<Nomenclature> data, int total, int totalPages)> GetAllNomenclatures(
            int? page = 1,
            int? pageSize = 10,
            string? search = null,
            bool? isAscending = true,
            string? sortBy = "Id",
            CancellationToken cancellationToken = default)
        {
            var query = _context.Nomenclatures.AsQueryable();

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

        public async Task<Nomenclature> CreateNomenclature(Nomenclature nomenclature, CancellationToken cancellationToken = default)
        {
            await _context.Nomenclatures.AddAsync(nomenclature, cancellationToken);
            await _context.SaveChangesAsync(cancellationToken);
            return nomenclature;
        }

        public async Task<Nomenclature> UpdateNomenclature(Nomenclature nomenclature, CancellationToken cancellationToken = default)
        {
            _context.Nomenclatures.Update(nomenclature);
            await _context.SaveChangesAsync(cancellationToken);
            return nomenclature;
        }

        public async Task DeleteNomenclature(Nomenclature nomenclature, CancellationToken cancellationToken = default)
        {
            _context.Nomenclatures.Remove(nomenclature);
            await _context.SaveChangesAsync(cancellationToken);
        }

    }
}