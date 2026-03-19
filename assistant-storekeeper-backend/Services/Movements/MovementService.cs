using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using assistant_storekeeper_backend.Models;
using assistant_storekeeper_backend.Repositories.Movements;
using assistant_storekeeper_backend.Exceptions;
using assistant_storekeeper_backend.Services.CompanyWarehouses;
using assistant_storekeeper_backend.Repositories.CompanyWarehouses;
using assistant_storekeeper_backend.Services.Movements;


namespace assistant_storekeeper_backend.Services.Movements
{
    public class MovementService : IMovementService
    {
        private readonly IMovementRepository _movementRepository;
        private readonly ICompanyWarehouseRepository _companyWarehouseRepository;

        public MovementService(IMovementRepository movementRepository, ICompanyWarehouseRepository companyWarehouseRepository)
        {
            _movementRepository = movementRepository;
            _companyWarehouseRepository = companyWarehouseRepository;
        }

        public async Task<Movement?> GetMovementById(int id, CancellationToken cancellationToken = default)
        {
            var movement = await _movementRepository.GetMovementById(id, cancellationToken);
            if (movement == null) {
                throw new NotFoundException("Movement not found");
            }
            return movement;
        }

        public async Task<IEnumerable<Movement>> GetAllMovements(
            int? page,
            int? pageSize,
            string? search,
            bool? isAscending,
            string? sortBy,
            CancellationToken cancellationToken = default)
        {
            return await _movementRepository.GetAllMovements(page, pageSize, search, isAscending, sortBy, cancellationToken);
        }

        public async Task<Movement> CreateMovement(Movement movement, CancellationToken cancellationToken = default)
        {
            if (movement.CompanyWarehouseFromId == null && movement.CompanyWarehouseToId == null) {
                throw new BadRequestException("Company warehouse from or to is required");
            }

            if (movement.CompanyWarehouseFromId != null) {
                int companyWarehouseFromId = movement.CompanyWarehouseFromId.Value;
                var companyWarehouseFrom = await _companyWarehouseRepository.GetCompanyWarehouseById(companyWarehouseFromId, cancellationToken);
                if (companyWarehouseFrom == null) {
                    throw new NotFoundException("Company warehouse from not found");
                }
            }

            if (movement.CompanyWarehouseToId != null) {
                int companyWarehouseToId = movement.CompanyWarehouseToId.Value;
                var companyWarehouseTo = await _companyWarehouseRepository.GetCompanyWarehouseById(companyWarehouseToId, cancellationToken);
                if (companyWarehouseTo == null) {
                    throw new NotFoundException("Company warehouse to not found");
                }
            }

            // Если со склада (from) ушло не на наш склад а в другое место, то статус должен быть расход (сonsumption)
            if (movement.CompanyWarehouseFromId != null && movement.CompanyWarehouseToId == null && movement.Status != MovementStatus.Consumption) {
                throw new BadRequestException("Status must be consumption");
            }

            // Если на склад (to) пришло не с другого склада а из другого места, то статус должен быть приход (coming)
            if (movement.CompanyWarehouseFromId == null && movement.CompanyWarehouseToId != null && movement.Status != MovementStatus.Coming) {
                throw new BadRequestException("Status must be coming");
            }

            // Если перемещаем со склада на склад то перемещение (moving)  
            if (movement.CompanyWarehouseFromId != null && movement.CompanyWarehouseToId != null && movement.Status != MovementStatus.Moving) {
                throw new BadRequestException("Status must be moving");
            }

            movement.Date = DateTime.UtcNow;
            return await _movementRepository.CreateMovement(movement, cancellationToken);
        }

        /*На данном этапе нет необходимости в обновлении перемещения, тк нет какой либо ролевки, а это один из главных документов
        public async Task<Movement> UpdateMovement(int id, Movement movement, CancellationToken cancellationToken = default)
        {
            var existingMovement = await _movementRepository.GetMovementById(id, cancellationToken);
            if (existingMovement == null) {
                throw new NotFoundException("Movement not found");
            }


                        if (movement.CompanyWarehouseFromId == null && movement.CompanyWarehouseToId == null) {
                throw new BadRequestException("Company warehouse from or to is required");
            }

            if (movement.CompanyWarehouseFromId != null) {
                int companyWarehouseFromId = movement.CompanyWarehouseFromId.Value;
                var companyWarehouseFrom = await _companyWarehouseRepository.GetCompanyWarehouseById(companyWarehouseFromId, cancellationToken);
                if (companyWarehouseFrom == null) {
                    throw new NotFoundException("Company warehouse from not found");
                }
            }

            if (movement.CompanyWarehouseToId != null) {
                int companyWarehouseToId = movement.CompanyWarehouseToId.Value;
                var companyWarehouseTo = await _companyWarehouseRepository.GetCompanyWarehouseById(companyWarehouseToId, cancellationToken);
                if (companyWarehouseTo == null) {
                    throw new NotFoundException("Company warehouse to not found");
                }
            }

            // Если со склада (from) ушло не на наш склад а в другое место, то статус должен быть расход (сonsumption)
            if (movement.CompanyWarehouseFromId != null && movement.CompanyWarehouseToId == null && movement.Status != MovementStatus.Consumption) {
                throw new BadRequestException("Status must be consumption");
            }

            // Если на склад (to) пришло не с другого склада а из другого места, то статус должен быть приход (coming)
            if (movement.CompanyWarehouseFromId == null && movement.CompanyWarehouseToId != null && movement.Status != MovementStatus.Coming) {
                throw new BadRequestException("Status must be coming");
            }

            // Если перемещаем со склада на склад то перемещение (moving)  
            if (movement.CompanyWarehouseFromId != null && movement.CompanyWarehouseToId != null && movement.Status != MovementStatus.Moving) {
                throw new BadRequestException("Status must be moving");
            }

            return await _movementRepository.UpdateMovement(id, movement, cancellationToken);
        }

        public async Task DeleteMovement(int id, CancellationToken cancellationToken = default)
        {
            var existingMovement = await _movementRepository.GetMovementById(id, cancellationToken);
            if (existingMovement == null) {
                throw new NotFoundException("Movement not found");
            }
            await _movementRepository.DeleteMovement(existingMovement, cancellationToken);
        }
        */
    }
}