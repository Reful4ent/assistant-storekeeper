using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using assistant_storekeeper_backend.Models;

namespace assistant_storekeeper_backend.Repositories.MovementNomenclatures
{
    public interface IMovementNomenclatureRepository
    {
        Task<MovementNomenclature?> GetMovementNomenclatureById(int id, CancellationToken cancellationToken = default);
        Task<IEnumerable<MovementNomenclature>> GetAllMovementNomenclatures(
            int movementId,
            int? page = 1,
            int? pageSize = 10,
            string? search = null,
            bool? isAscending = true,
            string? sortBy = "NomenclatureName",
            CancellationToken cancellationToken = default);
        Task<MovementNomenclature> CreateMovementNomenclature(MovementNomenclature movementNomenclature, CancellationToken cancellationToken = default);
        Task<MovementNomenclature> UpdateMovementNomenclature(MovementNomenclature movementNomenclature, CancellationToken cancellationToken = default);
        Task DeleteMovementNomenclature(MovementNomenclature movementNomenclature, CancellationToken cancellationToken = default);
    }
}