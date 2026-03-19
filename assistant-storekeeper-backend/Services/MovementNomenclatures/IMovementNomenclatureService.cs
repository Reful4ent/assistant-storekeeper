using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using assistant_storekeeper_backend.Models;

namespace assistant_storekeeper_backend.Services.MovementNomenclatures
{
    public interface IMovementNomenclatureService
    {
        Task<IEnumerable<MovementNomenclature>> GetAllMovementNomenclatures(
            int movementId,
            int? page = 1,
            int? pageSize = 10,
            string? search = null,
            bool? isAscending = true,
            string? sortBy = null,
            CancellationToken cancellationToken = default);
        Task<MovementNomenclature> CreateMovementNomenclature(MovementNomenclature movementNomenclature, CancellationToken cancellationToken = default);
        Task<MovementNomenclature> UpdateMovementNomenclature(int id, MovementNomenclature movementNomenclature, CancellationToken cancellationToken = default);
        Task DeleteMovementNomenclature(int id, CancellationToken cancellationToken = default);
    }
}