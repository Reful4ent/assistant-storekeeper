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
using assistant_storekeeper_backend.DTOS.WarehouseStateRequestDTOs;
using assistant_storekeeper_backend.DTOS.CompanyWarehouseNomenclatureCalculateDTOs;



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

        public async Task<(IEnumerable<Movement> data, int total, int totalPages)> GetAllMovements(
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

            // Если перемещаем со склада на склад то склады не могут быть одинаковыми
            if (movementDTO.Status == MovementStatus.Moving && movementDTO.CompanyWarehouseFromId == movementDTO.CompanyWarehouseToId) {
                throw new BadRequestException("Company warehouse from and to cannot be the same");
            }

            var movement = new Movement
            {
                CompanyWarehouseFromId = movementDTO.CompanyWarehouseFromId,
                CompanyWarehouseToId = movementDTO.CompanyWarehouseToId,
                Status = movementDTO.Status,
                Date = DateTime.UtcNow,
            };


            // ВАЖНО: Проверяем, что номенклатуры существуют и количество на складе достаточно перед созданием перемещения
            await ValidateNomenclaturesStock(movementDTO, cancellationToken);

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

            return await _movementRepository.GetMovementById(resultMovement.Id, cancellationToken);
        }


        public async Task DeleteMovement(int id, CancellationToken cancellationToken = default)
        {
            var existingMovement = await _movementRepository.GetMovementById(id, cancellationToken);
            if (existingMovement == null) {
                throw new NotFoundException("Movement not found");
            }
            var existingMovementNomenclatures = await _movementNomenclatureService.GetAllMovementNomenclatures(existingMovement.Id, null, null, null, null, null, cancellationToken);
            foreach (var nomenclature in existingMovementNomenclatures) {
                switch (existingMovement.Status) {
                    case MovementStatus.Moving:
                        var companyWarehouseNomenclatureFromMoving = await _companyWareHouseNomenclatureService.GetCompanyWarehouseNomenclatureByCompanyWarehouseIdAndNomenclatureId(
                            existingMovement.CompanyWarehouseFromId.Value, 
                            nomenclature.NomenclatureId, 
                            cancellationToken);
                        var companyWarehouseNomenclatureToMoving = await _companyWareHouseNomenclatureService.GetCompanyWarehouseNomenclatureByCompanyWarehouseIdAndNomenclatureId(
                            existingMovement.CompanyWarehouseToId.Value, 
                            nomenclature.NomenclatureId, 
                            cancellationToken);
                        if (companyWarehouseNomenclatureFromMoving == null) {
                            throw new NotFoundException("Company warehouse nomenclature from not found");
                        }
                        if (companyWarehouseNomenclatureToMoving == null) {
                            throw new NotFoundException("Company warehouse nomenclature to not found");
                        }
                        companyWarehouseNomenclatureFromMoving.Quantity += nomenclature.Quantity;
                        await _companyWareHouseNomenclatureService.UpdateCompanyWarehouseNomenclature(companyWarehouseNomenclatureFromMoving.Id, companyWarehouseNomenclatureFromMoving, cancellationToken);
                        companyWarehouseNomenclatureToMoving.Quantity -= nomenclature.Quantity;
                        await _companyWareHouseNomenclatureService.UpdateCompanyWarehouseNomenclature(companyWarehouseNomenclatureToMoving.Id, companyWarehouseNomenclatureToMoving, cancellationToken);
                        break;
                    case MovementStatus.Consumption:
                        var companyWarehouseNomenclatureConsumption = await _companyWareHouseNomenclatureService.GetCompanyWarehouseNomenclatureByCompanyWarehouseIdAndNomenclatureId(
                            existingMovement.CompanyWarehouseFromId.Value, 
                            nomenclature.NomenclatureId, 
                            cancellationToken);
                        if (companyWarehouseNomenclatureConsumption == null) {
                            throw new NotFoundException("Company warehouse nomenclature not found");
                        }
                        companyWarehouseNomenclatureConsumption.Quantity += nomenclature.Quantity;
                        await _companyWareHouseNomenclatureService.UpdateCompanyWarehouseNomenclature(companyWarehouseNomenclatureConsumption.Id, companyWarehouseNomenclatureConsumption, cancellationToken);
                        break;
                    case MovementStatus.Coming:
                        var companyWarehouseNomenclatureComing = await _companyWareHouseNomenclatureService.GetCompanyWarehouseNomenclatureByCompanyWarehouseIdAndNomenclatureId(
                            existingMovement.CompanyWarehouseToId.Value, 
                            nomenclature.NomenclatureId, 
                            cancellationToken);
                        if (companyWarehouseNomenclatureComing == null) {
                            throw new NotFoundException("Company warehouse nomenclature not found");
                        }
                        companyWarehouseNomenclatureComing.Quantity -= nomenclature.Quantity;
                        await _companyWareHouseNomenclatureService.UpdateCompanyWarehouseNomenclature(companyWarehouseNomenclatureComing.Id, companyWarehouseNomenclatureComing, cancellationToken);
                        break;
                }
                await _movementNomenclatureService.DeleteMovementNomenclature(nomenclature.Id, cancellationToken);
            }
            await _movementRepository.DeleteMovement(existingMovement, cancellationToken);
        }


        /// <summary>
        /// Получает список перемещений за дату
        /// <param name="warehouseStateRequestDTO">DTO запроса</param>
        /// <param name="cancellationToken">Токен отмены</param>
        /// <returns>Список перемещений за дату</returns>
        /// <exception cref="BadRequestException">Если дата запроса не указана</exception>
        /// <exception cref="NotFoundException">Если склад не найден</exception>
        /// </summary>
        public async Task<WarehouseStateResponseDTO> GetMovementsByDate(WarehouseStateRequestDTO warehouseStateRequestDTO, CancellationToken cancellationToken = default)
        {
            await _companyWarehouseService.GetCompanyWarehouseById(warehouseStateRequestDTO.CompanyWarehouseId, cancellationToken);
            if (warehouseStateRequestDTO.RequestDate == null) {
                throw new BadRequestException("Request date is required");
            }
            var movements = await _movementRepository.GetMovementsByDate(warehouseStateRequestDTO.CompanyWarehouseId, warehouseStateRequestDTO.RequestDate.Value, cancellationToken);
            var companyWarehouseNomenclaturesDictionary = new Dictionary<int, (int quantity, string nomenclatureName)>();
            foreach (var movement in movements) {
                foreach (var movementNomenclatures in movement.MovementNomenclatures) {
                    if (companyWarehouseNomenclaturesDictionary.ContainsKey(movementNomenclatures.NomenclatureId)) {
                        int currentQuantity = CalculateQuantity(
                            movement.Status, 
                            companyWarehouseNomenclaturesDictionary[movementNomenclatures.NomenclatureId].quantity, 
                            movementNomenclatures.Quantity, 
                            movement.CompanyWarehouseFromId,
                            warehouseStateRequestDTO.CompanyWarehouseId,
                            cancellationToken);
                        companyWarehouseNomenclaturesDictionary[movementNomenclatures.NomenclatureId] = (currentQuantity, movementNomenclatures.Nomenclature.Name);
                    } else {
                        int currentQuantity = CalculateQuantity(
                            movement.Status, 
                            0, 
                            movementNomenclatures.Quantity,
                            movement.CompanyWarehouseFromId,
                            warehouseStateRequestDTO.CompanyWarehouseId,
                            cancellationToken);
                        companyWarehouseNomenclaturesDictionary[movementNomenclatures.NomenclatureId] = (currentQuantity, movementNomenclatures.Nomenclature.Name);
                    }
                }
            }
            List<CompanyWarehouseNomenclatureCalculateDTO> companyWarehouseNomenclatures = new List<CompanyWarehouseNomenclatureCalculateDTO>();
            foreach (var nomenclature in companyWarehouseNomenclaturesDictionary) {
                companyWarehouseNomenclatures.Add(new CompanyWarehouseNomenclatureCalculateDTO
                {
                    NomenclatureId = nomenclature.Key,
                    Quantity = nomenclature.Value.quantity,
                    NomenclatureName = nomenclature.Value.nomenclatureName
                });
            }
            return new WarehouseStateResponseDTO
            {
                CompanyWarehouseNomenclatures = companyWarehouseNomenclatures
            };
        }

        /// <summary>
        /// Рассчитывает количество номенклатуры на складе после перемещения
        /// <param name="status">Статус перемещения</param>
        /// <param name="currentQuantity">Текущее количество номенклатуры на складе</param>
        /// <param name="movementQuantity">Количество номенклатуры в перемещении</param>
        /// <param name="companyWarehouseFromId">ID склада откуда перемещаем</param>
        /// <param name="companyWarehouseId">ID склада куда перемещаем</param>
        /// <param name="cancellationToken">Токен отмены</param>
        /// <returns>Количество номенклатуры на складе после перемещения</returns>
        /// </summary>
        private int CalculateQuantity(
            MovementStatus status, 
            int currentQuantity, 
            int movementQuantity,
            int? companyWarehouseFromId,
            int companyWarehouseId,
            CancellationToken cancellationToken = default)
        {
            switch (status) {
                case MovementStatus.Moving:
                    if (companyWarehouseFromId != null && companyWarehouseId == companyWarehouseFromId.Value) {
                        return currentQuantity - movementQuantity;
                    } 
                    return currentQuantity + movementQuantity;
                case MovementStatus.Consumption:
                    return currentQuantity - movementQuantity;
                case MovementStatus.Coming:
                    return currentQuantity + movementQuantity;
            }
            return currentQuantity;
        }

        /// <summary>
        /// Проверяет, что номенклатуры существуют и количество на складе достаточно перед созданием перемещения
        /// <param name="movementDTO">DTO перемещения</param>
        /// <param name="cancellationToken">Токен отмены</param>
        /// <exception cref="BadRequestException">Если номенклатуры не существуют</exception>
        /// <exception cref="NotFoundException">Если номенклатура не найдена на складе</exception>
        /// <exception cref="BadRequestException">Если количество на складе меньше, чем количество указанное в перемещении</exception>
        /// </summary>
        private async Task ValidateNomenclaturesStock(MovementDTO movementDTO, CancellationToken cancellationToken)
        {
            if (movementDTO.Nomenclatures.Count == 0)
                throw new BadRequestException("Nomenclatures are required");

            if (movementDTO.Status != MovementStatus.Consumption && movementDTO.Status != MovementStatus.Moving)
                return;

            var warehouseId = movementDTO.CompanyWarehouseFromId!.Value;
            foreach (var nomenclature in movementDTO.Nomenclatures)
            {
                var companyWareHouseNomenclatureService = await _companyWareHouseNomenclatureService.GetCompanyWarehouseNomenclatureByCompanyWarehouseIdAndNomenclatureId(
                        warehouseId, 
                        nomenclature.Id, 
                        cancellationToken);

                if (companyWareHouseNomenclatureService == null)
                    throw new NotFoundException("Company warehouse nomenclature not found");

                if (companyWareHouseNomenclatureService.Quantity < nomenclature.Quantity)
                    throw new BadRequestException("Can't write more items than are in stock.");
            }
        }
    }
}