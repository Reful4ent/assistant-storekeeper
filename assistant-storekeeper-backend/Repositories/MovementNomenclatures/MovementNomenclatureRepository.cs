namespace assistant_storekeeper_backend.Repositories.MovementNomenclatures
{
    public class MovementNomenclatureRepository : IMovementNomenclatureRepository
    {
        private readonly ApplicationDbContext _context;

        public MovementNomenclatureRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<MovementNomenclature>> GetAllMovementNomenclatures(
            int? page = 1,
            int? pageSize = 10,
            string? search = null,
            bool? isAscending = true,
            string? sortBy = "MovementName",
            CancellationToken cancellationToken = default)
        {
            var query = _context.MovementNomenclatures
                .Include(m => m.Movement)
                .Include(m => m.Nomenclature)
                .AsQueryable();

            if (!string.IsNullOrEmpty(search))
            {
                query = query.Where(m => m.Movement.Name.Contains(search));
            }

            if (sortBy == "MovementName")
            {
                query = isAscending == true 
                    ? query.OrderBy(m => m.Movement.Name) 
                    : query.OrderByDescending(m => m.Movement.Name);
            }
            else if (sortBy == "Quantity")
            {
                query = isAscending == true 
                    ? query.OrderBy(m => m.Quantity) 
                    : query.OrderByDescending(m => m.Quantity);
            }
            else
            {
                query = isAscending == true 
                    ? query.OrderBy(m => m.Id) 
                    : query.OrderByDescending(m => m.Id);
            }

            return await query
                .Skip(((page ?? 1) - 1) * (pageSize ?? 10))
                .Take(pageSize ?? 10)
                .ToListAsync(cancellationToken);
        }

        public async Task<MovementNomenclature> CreateMovementNomenclature(MovementNomenclature movementNomenclature, CancellationToken cancellationToken = default)
        {
            await _context.MovementNomenclatures.AddAsync(movementNomenclature, cancellationToken);
            await _context.SaveChangesAsync(cancellationToken);
            return movementNomenclature;
        }

        public async Task<MovementNomenclature> UpdateMovementNomenclature(MovementNomenclature movementNomenclature, CancellationToken cancellationToken = default)
        {
            _context.MovementNomenclatures.Update(movementNomenclature);
            await _context.SaveChangesAsync(cancellationToken);
            return movementNomenclature;
        }

        public async Task DeleteMovementNomenclature(MovementNomenclature movementNomenclature, CancellationToken cancellationToken = default)
        {
            _context.MovementNomenclatures.Remove(movementNomenclature);
            await _context.SaveChangesAsync(cancellationToken);
        }
    }
}