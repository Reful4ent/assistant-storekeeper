using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using assistant_storekeeper_backend.Models;
using System.Linq;
using System;

namespace assistant_storekeeper_backend.Repositories.Movements
{
    public interface IMovementRepository
    {
        Task<Movement?> GetMovementById(int id, CancellationToken cancellationToken = default);
        Task<IEnumerable<Movement>> GetAllMovements(
            int? page = 1,
            int? pageSize = 10,
            string? search = null,
            bool? isAscending = true,
            string? sortBy = "Id",
            CancellationToken cancellationToken = default);
        Task<Movement> CreateMovement(Movement movement, CancellationToken cancellationToken = default);
        Task<Movement> UpdateMovement(Movement movement, CancellationToken cancellationToken = default);
        Task DeleteMovement(Movement movement, CancellationToken cancellationToken = default);
        Task <IEnumerable<Movement>> GetMovementsByDate(int companyWarehouseId, DateTime date, CancellationToken cancellationToken = default);
    }
}