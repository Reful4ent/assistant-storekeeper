using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using assistant_storekeeper_backend.Models;
using Microsoft.EntityFrameworkCore;
using assistant_storekeeper_backend.Data;
using System.Linq;
using System;
using Microsoft.EntityFrameworkCore.Query.SqlExpressions;

namespace assistant_storekeeper_backend.Repositories.Movements
{
    public class MovementRepository : IMovementRepository
    {
        private readonly ApplicationDbContext _context;

        public MovementRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<Movement?> GetMovementById(int id, CancellationToken cancellationToken = default)
        {
            return await _context.Movements
                .Include(m => m.CompanyWarehouseFrom)
                .Include(m => m.CompanyWarehouseTo)
                .Include(m => m.MovementNomenclatures)
                .ThenInclude(mn => mn.Nomenclature)
                .FirstOrDefaultAsync(c => c.Id == id, cancellationToken);
        }

        public async Task<(IEnumerable<Movement> data, int total, int totalPages)> GetAllMovements(
            int? page = 1,
            int? pageSize = 10,
            string? search = null,
            bool? isAscending = true,
            string? sortBy = "Id",
            CancellationToken cancellationToken = default)
        {
            var query = _context.Movements
                .Include(m => m.CompanyWarehouseFrom)
                .Include(m => m.CompanyWarehouseTo)
                .Include(m => m.MovementNomenclatures)
                .ThenInclude(mn => mn.Nomenclature)
                .AsQueryable();

            if (!string.IsNullOrEmpty(search))
            {
                query = query.Where(m => EF.Functions.ILike(m.Id.ToString(), "%" + search.Trim() + "%"));
            }

            if (sortBy == "CompanyWarehouseFrom")
            {
                query = isAscending == true 
                    ? query.OrderBy(m => m.CompanyWarehouseFrom.Name) 
                    : query.OrderByDescending(m => m.CompanyWarehouseFrom.Name);
            }
            else if (sortBy == "CompanyWarehouseTo")
            {
                query = isAscending == true 
                    ? query.OrderBy(m => m.CompanyWarehouseTo.Name) 
                    : query.OrderByDescending(m => m.CompanyWarehouseTo.Name);
            }
            else if(sortBy == "Id")
            {
                query = isAscending == true 
                ? query.OrderBy(m => m.Id) 
                : query.OrderByDescending(m => m.Id);
            }
            else if(sortBy == "Status")
            {
                query = isAscending == true 
                ? query.OrderBy(m => m.Status) 
                : query.OrderByDescending(m => m.Status);
            }
            else
            {
                query = isAscending == false 
                    ? query.OrderByDescending(m => m.Date) 
                    : query.OrderBy(m => m.Date);
            }


            var total = await query.CountAsync(cancellationToken);
            var totalPages = (int)Math.Ceiling((double)total / (pageSize ?? 10));
            var data = await query
                .Skip(((page ?? 1) - 1) * (pageSize ?? 10))
                .Take(pageSize ?? 10)
                .ToListAsync(cancellationToken);

            return (data, total, totalPages);
        }

        public async Task<Movement> CreateMovement(Movement movement, CancellationToken cancellationToken = default)
        {
            await _context.Movements.AddAsync(movement, cancellationToken);
            await _context.SaveChangesAsync(cancellationToken);
            return movement;
        }
        
        public async Task<Movement> UpdateMovement(Movement movement, CancellationToken cancellationToken = default)
        {
            _context.Movements.Update(movement);
            await _context.SaveChangesAsync(cancellationToken);
            return movement;
        }
        
        public async Task DeleteMovement(Movement movement, CancellationToken cancellationToken = default)
        {
            _context.Movements.Remove(movement);
            await _context.SaveChangesAsync(cancellationToken);
        }

        public async Task<IEnumerable<Movement>> GetMovementsByDate(int companyWarehouseId, DateTime date, CancellationToken cancellationToken = default)
        {
            return await _context.Movements
                .Include(m => m.CompanyWarehouseFrom)
                .Include(m => m.CompanyWarehouseTo)
                .Include(m => m.MovementNomenclatures)
                .ThenInclude(mn => mn.Nomenclature)
                .Where(m => (m.CompanyWarehouseFromId == companyWarehouseId || m.CompanyWarehouseToId == companyWarehouseId) && m.Date.Date <= date.Date)
                .ToListAsync(cancellationToken);
        }
    }
}