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
using assistant_storekeeper_backend.Services.CompanyWareHouseNomenclatures;
using assistant_storekeeper_backend.Services.MovementNomenclatures;
using assistant_storekeeper_backend.DTOS.MovementDTOs;



namespace assistant_storekeeper_backend.Services.Movements
{
    public class MovementService : IMovementService
    {
        private readonly IMovementRepository _movementRepository;
        private readonly ICompanyWareHouseNomenclatureService _companyWareHouseNomenclatureService;
        private readonly IMovementNomenclatureService _movementNomenclatureService;
        private readonly ICompanyWarehouseService _companyWarehouseService;

        public MovementService(
            IMovementRepository movementRepository,
            ICompanyWarehouseService companyWarehouseService,
            ICompanyWareHouseNomenclatureService companyWareHouseNomenclatureService,
            IMovementNomenclatureService movementNomenclatureService)
        {
            _movementRepository = movementRepository;
            _companyWareHouseNomenclatureService = companyWareHouseNomenclatureService;
            _movementNomenclatureService = movementNomenclatureService; 
            _companyWarehouseService = companyWarehouseService;
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

        public async Task<Movement> CreateMovement(MovementDTO movementDTO, CancellationToken cancellationToken = default)
        {
            if (movementDTO.CompanyWarehouseFromId == null && movementDTO.CompanyWarehouseToId == null) {
                throw new BadRequestException("Company warehouse from or to is required");
            }

            if (movementDTO.CompanyWarehouseFromId != null) {
                await _companyWarehouseService.GetCompanyWarehouseById(movementDTO.CompanyWarehouseFromId.Value, cancellationToken);
            }

            if (movementDTO.CompanyWarehouseToId != null) {
                await _companyWarehouseService.GetCompanyWarehouseById(movementDTO.CompanyWarehouseToId.Value, cancellationToken);
            }

            // Если со склада (from) ушло не на наш склад а в другое место, то статус должен быть расход (сonsumption)
            if (movementDTO.CompanyWarehouseFromId != null && movementDTO.CompanyWarehouseToId == null && movementDTO.Status != MovementStatus.Consumption) {
                throw new BadRequestException("Status must be consumption");
            }

            // Если на склад (to) пришло не с другого склада а из другого места, то статус должен быть приход (coming)
            if (movementDTO.CompanyWarehouseFromId == null && movementDTO.CompanyWarehouseToId != null && movementDTO.Status != MovementStatus.Coming) {
                throw new BadRequestException("Status must be coming");
            }

            // Если перемещаем со склада на склад то перемещение (moving)  
            if (movementDTO.CompanyWarehouseFromId != null && movementDTO.CompanyWarehouseToId != null && movementDTO.Status != MovementStatus.Moving) {
                throw new BadRequestException("Status must be moving");
            }

            var movement = new Movement
            {
                CompanyWarehouseFromId = movementDTO.CompanyWarehouseFromId,
                CompanyWarehouseToId = movementDTO.CompanyWarehouseToId,
                Status = movementDTO.Status,
                Date = DateTime.UtcNow,
            };

            Movement resultMovement = await _movementRepository.CreateMovement(movement, cancellationToken);

            foreach (var nomenclature in movementDTO.Nomenclatures) {
                var movementNomenclature = new MovementNomenclature
                {
                    MovementId = resultMovement.Id,
                    NomenclatureId = nomenclature.Id,
                    Quantity = nomenclature.Quantity,
                };
                await _movementNomenclatureService.CreateMovementNomenclature(movementNomenclature, cancellationToken);
                switch (movementDTO.Status) {
                    case MovementStatus.Moving:
                        var companyWarehouseNomenclatureFromMoving = await _companyWareHouseNomenclatureService.GetCompanyWarehouseNomenclatureByCompanyWarehouseIdAndNomenclatureId(
                            movementDTO.CompanyWarehouseFromId.Value, 
                            nomenclature.Id, 
                            cancellationToken);
                        var companyWarehouseNomenclatureToMoving = await _companyWareHouseNomenclatureService.GetCompanyWarehouseNomenclatureByCompanyWarehouseIdAndNomenclatureId(
                            movementDTO.CompanyWarehouseToId.Value, 
                            nomenclature.Id, 
                            cancellationToken);

                        if (companyWarehouseNomenclatureFromMoving == null) {
                            throw new NotFoundException("Company warehouse nomenclature not found");
                        }
                        if (companyWarehouseNomenclatureToMoving == null) {
                            companyWarehouseNomenclatureToMoving = new CompanyWarehouseNomenclature
                            {
                                CompanyWarehouseId = movementDTO.CompanyWarehouseToId.Value,
                                NomenclatureId = nomenclature.Id,
                                Quantity = nomenclature.Quantity,
                            };
                            companyWarehouseNomenclatureFromMoving.Quantity -= nomenclature.Quantity;
                            await _companyWareHouseNomenclatureService.UpdateCompanyWarehouseNomenclature(companyWarehouseNomenclatureFromMoving.Id, companyWarehouseNomenclatureFromMoving, cancellationToken);
                            await _companyWareHouseNomenclatureService.CreateCompanyWarehouseNomenclature(companyWarehouseNomenclatureToMoving, cancellationToken);
                        } else {
                            companyWarehouseNomenclatureToMoving.Quantity += nomenclature.Quantity;
                            await _companyWareHouseNomenclatureService.UpdateCompanyWarehouseNomenclature(companyWarehouseNomenclatureToMoving.Id, companyWarehouseNomenclatureToMoving, cancellationToken);
                            companyWarehouseNomenclatureFromMoving.Quantity -= nomenclature.Quantity;
                            await _companyWareHouseNomenclatureService.UpdateCompanyWarehouseNomenclature(companyWarehouseNomenclatureFromMoving.Id, companyWarehouseNomenclatureFromMoving, cancellationToken);
                        }                    
                        break;
                    case MovementStatus.Consumption:
                        var companyWarehouseNomenclatureConsumption = await _companyWareHouseNomenclatureService.GetCompanyWarehouseNomenclatureByCompanyWarehouseIdAndNomenclatureId(
                            movementDTO.CompanyWarehouseFromId.Value, 
                            nomenclature.Id, 
                            cancellationToken);
                        if (companyWarehouseNomenclatureConsumption == null) {
                            throw new NotFoundException("Company warehouse nomenclature not found");
                        }
                        companyWarehouseNomenclatureConsumption.Quantity -= nomenclature.Quantity;
                        await _companyWareHouseNomenclatureService.UpdateCompanyWarehouseNomenclature(companyWarehouseNomenclatureConsumption.Id, companyWarehouseNomenclatureConsumption, cancellationToken);
                        break;
                    case MovementStatus.Coming:
                        var companyWarehouseNomenclatureComing = await _companyWareHouseNomenclatureService.GetCompanyWarehouseNomenclatureByCompanyWarehouseIdAndNomenclatureId(
                            movementDTO.CompanyWarehouseToId.Value, 
                            nomenclature.Id, 
                            cancellationToken);
                        if (companyWarehouseNomenclatureComing == null) {
                            companyWarehouseNomenclatureComing = new CompanyWarehouseNomenclature
                            {
                                CompanyWarehouseId = movementDTO.CompanyWarehouseToId.Value,
                                NomenclatureId = nomenclature.Id,
                                Quantity = nomenclature.Quantity,
                            };
                            await _companyWareHouseNomenclatureService.CreateCompanyWarehouseNomenclature(companyWarehouseNomenclatureComing, cancellationToken);
                        } else {
                            companyWarehouseNomenclatureComing.Quantity += nomenclature.Quantity;
                            await _companyWareHouseNomenclatureService.UpdateCompanyWarehouseNomenclature(companyWarehouseNomenclatureComing.Id, companyWarehouseNomenclatureComing, cancellationToken);
                        }
                        break;
                }
            }

            return await _movementRepository.GetMovementById(movement.Id, cancellationToken);
        }

        
        public async Task<Movement> UpdateMovement(int id, MovementDTO movementDTO, CancellationToken cancellationToken = default)
        {
            var existingMovement = await _movementRepository.GetMovementById(id, cancellationToken);
            if (existingMovement == null) {
                throw new NotFoundException("Movement not found");
            }


            if (movementDTO.CompanyWarehouseFromId == null && movementDTO.CompanyWarehouseToId == null) {
                throw new BadRequestException("Company warehouse from or to is required");
            }

            if (movementDTO.CompanyWarehouseFromId != null) {
                await _companyWarehouseService.GetCompanyWarehouseById(movementDTO.CompanyWarehouseFromId.Value, cancellationToken);
            }

            if (movementDTO.CompanyWarehouseToId != null) {
                await _companyWarehouseService.GetCompanyWarehouseById(movementDTO.CompanyWarehouseToId.Value, cancellationToken);
            }

            // Если со склада (from) ушло не на наш склад а в другое место, то статус должен быть расход (сonsumption)
            if (movementDTO.CompanyWarehouseFromId != null && movementDTO.CompanyWarehouseToId == null && movementDTO.Status != MovementStatus.Consumption) {
                throw new BadRequestException("Status must be consumption");
            }

            // Если на склад (to) пришло не с другого склада а из другого места, то статус должен быть приход (coming)
            if (movementDTO.CompanyWarehouseFromId == null && movementDTO.CompanyWarehouseToId != null && movementDTO.Status != MovementStatus.Coming) {
                throw new BadRequestException("Status must be coming");
            }

            // Если перемещаем со склада на склад то перемещение (moving)  
            if (movementDTO.CompanyWarehouseFromId != null && movementDTO.CompanyWarehouseToId != null && movementDTO.Status != MovementStatus.Moving) {
                throw new BadRequestException("Status must be moving");
            }

            existingMovement.CompanyWarehouseFromId = movementDTO.CompanyWarehouseFromId;
            existingMovement.CompanyWarehouseToId = movementDTO.CompanyWarehouseToId;
            existingMovement.Status = movementDTO.Status;
            


            return await _movementRepository.UpdateMovement(existingMovement, cancellationToken);
        }

        public async Task DeleteMovement(int id, CancellationToken cancellationToken = default)
        {
            var existingMovement = await _movementRepository.GetMovementById(id, cancellationToken);
            if (existingMovement == null) {
                throw new NotFoundException("Movement not found");
            }
            await _movementRepository.DeleteMovement(existingMovement, cancellationToken);
        }
    }
}