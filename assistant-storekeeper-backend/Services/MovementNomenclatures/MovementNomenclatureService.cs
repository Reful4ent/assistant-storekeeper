using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using assistant_storekeeper_backend.Models;
using assistant_storekeeper_backend.Repositories.MovementNomenclatures;
using assistant_storekeeper_backend.Exceptions;
using assistant_storekeeper_backend.Services.Movements;
using assistant_storekeeper_backend.Services.Nomenclatures;
using assistant_storekeeper_backend.Repositories.Movements;


namespace assistant_storekeeper_backend.Services.MovementNomenclatures
{
    public class MovementNomenclatureService : IMovementNomenclatureService
    {
        private readonly IMovementNomenclatureRepository _movementNomenclatureRepository;
        private readonly IMovementRepository _movementRepository;
        private readonly INomenclatureService _nomenclatureService;

        public MovementNomenclatureService(
            IMovementNomenclatureRepository movementNomenclatureRepository,
            IMovementRepository movementRepository,
            INomenclatureService nomenclatureService)
        {
            _movementNomenclatureRepository = movementNomenclatureRepository;
            _movementRepository = movementRepository;
            _nomenclatureService = nomenclatureService;
        }

        public async Task<IEnumerable<MovementNomenclature>> GetAllMovementNomenclatures(
            int movementId,
            int? page = 1,
            int? pageSize = 10,
            string? search = null,
            bool? isAscending = true,
            string? sortBy = null,
            CancellationToken cancellationToken = default)
        {
            await ValidateMovement(movementId, cancellationToken);
            return await _movementNomenclatureRepository.GetAllMovementNomenclatures(movementId, page, pageSize, search, isAscending, sortBy, cancellationToken);
        }

        public async Task<MovementNomenclature> CreateMovementNomenclature(MovementNomenclature movementNomenclature, CancellationToken cancellationToken = default)
        {
            await ValidateMovement(movementNomenclature.MovementId, cancellationToken);
            await ValidateNomenclature(movementNomenclature.NomenclatureId, cancellationToken);
            await ValidateQuantity(movementNomenclature.Quantity, cancellationToken);

            return await _movementNomenclatureRepository.CreateMovementNomenclature(movementNomenclature, cancellationToken);
        }

        public async Task<MovementNomenclature> UpdateMovementNomenclature(int id, MovementNomenclature movementNomenclature, CancellationToken cancellationToken = default)
        {
            var existingMovementNomenclature = await _movementNomenclatureRepository.GetMovementNomenclatureById(id, cancellationToken);
            if (existingMovementNomenclature == null)
            {
                throw new NotFoundException("Movement nomenclature not found");
            }
            await ValidateMovement(movementNomenclature.MovementId, cancellationToken);
            await ValidateNomenclature(movementNomenclature.NomenclatureId, cancellationToken);
            await ValidateQuantity(movementNomenclature.Quantity, cancellationToken);

            existingMovementNomenclature.MovementId = movementNomenclature.MovementId;
            existingMovementNomenclature.NomenclatureId = movementNomenclature.NomenclatureId;
            existingMovementNomenclature.Quantity = movementNomenclature.Quantity;

            return await _movementNomenclatureRepository.UpdateMovementNomenclature(existingMovementNomenclature, cancellationToken);
        }

        public async Task DeleteMovementNomenclature(int id, CancellationToken cancellationToken = default)
        {
            var movementNomenclature = await _movementNomenclatureRepository.GetMovementNomenclatureById(id, cancellationToken);
            if (movementNomenclature == null)
            {
                throw new NotFoundException("Movement nomenclature not found");
            }
            await _movementNomenclatureRepository.DeleteMovementNomenclature(movementNomenclature, cancellationToken);
        }

        private async Task ValidateMovement(int id, CancellationToken cancellationToken = default)
        {
            if (id <= 0)
            {
                throw new BadRequestException("Id is required");
            }
            else
            {
                var movement = await _movementRepository.GetMovementById(id, cancellationToken);
                if (movement == null)
                {
                    throw new NotFoundException("Movement not found");
                }
            }
        }

        private async Task ValidateNomenclature(int id, CancellationToken cancellationToken = default)
        {
            if (id <= 0)
            {
                throw new BadRequestException("Id is required");
            }
            else
            {
                await _nomenclatureService.GetNomenclatureById(id, cancellationToken);
            }
        }

        private async Task ValidateQuantity(int Quantity, CancellationToken cancellationToken = default) 
        {
            if (Quantity < 0)
            {
                throw new BadRequestException("Quantity must be greater than or equal to 0");
            }
        }
    }
}