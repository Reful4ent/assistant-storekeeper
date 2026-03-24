using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using assistant_storekeeper_backend.Models;
using assistant_storekeeper_backend.DTOS.MovementDTOs;
using assistant_storekeeper_backend.Services.CompanyWareHouseNomenclatures;
using assistant_storekeeper_backend.Services.MovementNomenclatures;
using assistant_storekeeper_backend.DTOS.MovementDTOs;
using assistant_storekeeper_backend.DTOS.WarehouseStateRequestDTOs;

namespace assistant_storekeeper_backend.Services.Movements
{
    public interface IMovementService
    {
        Task<Movement?> GetMovementById(int id, CancellationToken cancellationToken = default);
        Task<(IEnumerable<Movement> data, int total, int totalPages)> GetAllMovements(
            int? page,
            int? pageSize,
            string? search,
            bool? isAscending,
            string? sortBy,
            CancellationToken cancellationToken = default);
        Task<Movement> CreateMovement(MovementDTO movementDTO, CancellationToken cancellationToken = default);
        Task DeleteMovement(int id, CancellationToken cancellationToken = default);
        Task<WarehouseStateResponseDTO> GetMovementsByDate(WarehouseStateRequestDTO warehouseStateRequestDTO, CancellationToken cancellationToken = default);
    }
}