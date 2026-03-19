using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using assistant_storekeeper_backend.Models;
using assistant_storekeeper_backend.Repositories.MovementNomenclatures;
using assistant_storekeeper_backend.Repositories.Movements;
using assistant_storekeeper_backend.Repositories.Nomenclatures;
using assistant_storekeeper_backend.Exceptions;

namespace assistant_storekeeper_backend.Services.MovementNomenclatures
{
    public class MovementNomenclatureService : IMovementNomenclatureService
    {
        private readonly IMovementNomenclatureRepository _movementNomenclatureRepository;
        private readonly IMovementRepository _movementRepository;
        private readonly INomenclatureRepository _nomenclatureRepository;
        public MovementNomenclatureService(
            IMovementNomenclatureRepository movementNomenclatureRepository,
            IMovementRepository movementRepository,
            INomenclatureRepository nomenclatureRepository)
        {
            _movementNomenclatureRepository = movementNomenclatureRepository;
            _movementRepository = movementRepository;
            _nomenclatureRepository = nomenclatureRepository;
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
            if (movementId <= 0)
            {
                throw new BadRequestException("Movement is required");
            }
            else
            {
                var movement = await _movementRepository.GetMovementById(movementId, cancellationToken);
                if (movement == null)
                {
                    throw new NotFoundException("Movement not found");
                }
            }
            return await _movementNomenclatureRepository.GetAllMovementNomenclatures(movementId, page, pageSize, search, isAscending, sortBy, cancellationToken);
        }

        public async Task<MovementNomenclature> CreateMovementNomenclature(MovementNomenclature movementNomenclature, CancellationToken cancellationToken = default)
        {
            if (movementNomenclature.MovementId <= 0)
            {
                throw new BadRequestException("Movement is required");
            }
            else
            {
                var movement = await _movementRepository.GetMovementById(movementNomenclature.MovementId, cancellationToken);
                if (movement == null)
                {
                    throw new NotFoundException("Movement not found");
                }
            }

            if (movementNomenclature.NomenclatureId <= 0)
            {
                throw new BadRequestException("Nomenclature is required");
            }
            else
            {
                var nomenclature = await _nomenclatureRepository.GetNomenclatureById(movementNomenclature.NomenclatureId, cancellationToken);
                if (nomenclature == null)
                {
                    throw new NotFoundException("Nomenclature not found");
                }
            }

            if (movementNomenclature.Quantity <= 0)
            {
                throw new BadRequestException("Quantity must be greater than 0");
            }

            return await _movementNomenclatureRepository.CreateMovementNomenclature(movementNomenclature, cancellationToken);
        }

        public async Task<MovementNomenclature> UpdateMovementNomenclature(int id, MovementNomenclature movementNomenclature, CancellationToken cancellationToken = default)
        {
            var existingMovementNomenclature = await _movementNomenclatureRepository.GetMovementNomenclatureById(id, cancellationToken);
            if (existingMovementNomenclature == null)
            {
                throw new NotFoundException("Movement nomenclature not found");
            }

            if (movementNomenclature.MovementId <= 0)
            {
                throw new BadRequestException("Movement is required");
            }
            else
            {
                var movement = await _movementRepository.GetMovementById(movementNomenclature.MovementId, cancellationToken);
                if (movement == null)
                {
                    throw new NotFoundException("Movement not found");
                }
            }

            if (movementNomenclature.NomenclatureId <= 0)
            {
                throw new BadRequestException("Nomenclature is required");
            }
            else
            {
                var nomenclature = await _nomenclatureRepository.GetNomenclatureById(movementNomenclature.NomenclatureId, cancellationToken);
                if (nomenclature == null)
                {
                    throw new NotFoundException("Nomenclature not found");
                }
            }

            if (movementNomenclature.Quantity <= 0)
            {
                throw new BadRequestException("Quantity must be greater than 0");
            }

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
    }
}