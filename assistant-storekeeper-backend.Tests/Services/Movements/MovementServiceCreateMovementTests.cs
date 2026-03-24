using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using assistant_storekeeper_backend.DTOS.MovementDTOs;
using assistant_storekeeper_backend.DTOS.MovementNomenclatureDTOs;
using assistant_storekeeper_backend.Exceptions;
using assistant_storekeeper_backend.Models;
using assistant_storekeeper_backend.Repositories.Movements;
using assistant_storekeeper_backend.Services.CompanyWareHouseNomenclatures;
using assistant_storekeeper_backend.Services.CompanyWarehouses;
using assistant_storekeeper_backend.Services.MovementNomenclatures;
using assistant_storekeeper_backend.Services.Movements;
using assistant_storekeeper_backend.Services.Nomenclatures;
using Moq;
using Xunit;

namespace assistant_storekeeper_backend.Tests.Services.Movements
{
    public class MovementServiceCreateMovementTests
    {
        private static MovementService CreateSut(
            Mock<IMovementRepository>? mockMovementRepo = null,
            Mock<ICompanyWarehouseService>? mockCompanyWarehouse = null,
            Mock<ICompanyWareHouseNomenclatureService>? mockCwn = null,
            Mock<IMovementNomenclatureService>? mockMn = null)
        {
            mockMovementRepo ??= new Mock<IMovementRepository>();
            mockCompanyWarehouse ??= new Mock<ICompanyWarehouseService>();
            mockCwn ??= new Mock<ICompanyWareHouseNomenclatureService>();
            mockMn ??= new Mock<IMovementNomenclatureService>();

            return new MovementService(
                mockMovementRepo.Object,
                mockCompanyWarehouse.Object,
                mockCwn.Object,
                mockMn.Object);
        }


        [Fact]
        public async Task CreateMovement_WhenBothWarehousesNull_ThrowsBadRequestException()
        {
            MovementService sut = CreateSut();

            MovementDTO dto = new MovementDTO
            {
                CompanyWarehouseFromId = null,
                CompanyWarehouseToId = null,
                Status = MovementStatus.Coming,
                Nomenclatures = new List<NomenclatureInMovementDTO>()
            };

            Func<Task> act = () => sut.CreateMovement(dto);

            var ex = await Assert.ThrowsAsync<BadRequestException>(act);
            Assert.Equal("Company warehouse from or to is required", ex.Message);
        }

        [Fact]
        public async Task CreateMovement_WhenCompanyWarehouseNotNullAndNotFound_ThrowNotFoundException(){
            int companyWarehouseIdNotExists = 123;

            Mock<ICompanyWarehouseService> companyWarehouseServiceMock = new Mock<ICompanyWarehouseService>();
            companyWarehouseServiceMock
                .Setup(s => s.GetCompanyWarehouseById(companyWarehouseIdNotExists, It.IsAny<CancellationToken>()))
                .ThrowsAsync(new NotFoundException("Company warehouse not found"));

            MovementService sut = CreateSut(mockCompanyWarehouse: companyWarehouseServiceMock);

            MovementDTO dto = new MovementDTO
            {
                CompanyWarehouseFromId = companyWarehouseIdNotExists,
                CompanyWarehouseToId = null,
                Status = MovementStatus.Consumption,
                Nomenclatures = new List<NomenclatureInMovementDTO>()
            };

            Func<Task> act = () => sut.CreateMovement(dto);

            var ex = await Assert.ThrowsAsync<NotFoundException>(act);
            Assert.Equal("Company warehouse not found", ex.Message);
        }

        [Theory]
        [InlineData(MovementStatus.Coming)]
        [InlineData(MovementStatus.Moving)]
        public async Task CreateMovement_WhenCompanyWarehouseFromNotNullAndCompanyWarehouseToNullAndStatusNotConsumption_ThrowBadRequestException(
            MovementStatus status)
        {
            MovementService sut = CreateSut();
            MovementDTO dto = new MovementDTO
            {
                CompanyWarehouseFromId = 1,
                CompanyWarehouseToId = null,
                Status = status,
                Nomenclatures = new List<NomenclatureInMovementDTO>()
            };
            Func<Task> act = () => sut.CreateMovement(dto);
            var ex = await Assert.ThrowsAsync<BadRequestException>(act);
            Assert.Equal("Status must be consumption", ex.Message);
        }

        [Theory]
        [InlineData(MovementStatus.Consumption)]
        [InlineData(MovementStatus.Moving)]
        public async Task CreateMovement_WhenCompanyWarehouseToNotNullAndCompanyWarehouseFromNullAndStatusNotComing_ThrowBadRequestException(
            MovementStatus status)
        {
            MovementService sut = CreateSut();
            MovementDTO dto = new MovementDTO
            {
                CompanyWarehouseFromId = null,
                CompanyWarehouseToId = 1,
                Status = status,
                Nomenclatures = new List<NomenclatureInMovementDTO>()
            };
            Func<Task> act = () => sut.CreateMovement(dto);
            var ex = await Assert.ThrowsAsync<BadRequestException>(act);
            Assert.Equal("Status must be coming", ex.Message);
        }

        [Theory]
        [InlineData(MovementStatus.Consumption)]
        [InlineData(MovementStatus.Coming)]
        public async Task CreateMovement_WhenCompanyWarehouseFromNotNullAndCompanyWarehouseToNotNullAndStatusNotMoving_ThrowBadRequestException(
            MovementStatus status)
        {
            MovementService sut = CreateSut();
            MovementDTO dto = new MovementDTO
            {
                CompanyWarehouseFromId = 2,
                CompanyWarehouseToId = 1,
                Status = status,
                Nomenclatures = new List<NomenclatureInMovementDTO>()
            };
            Func<Task> act = () => sut.CreateMovement(dto);
            var ex = await Assert.ThrowsAsync<BadRequestException>(act);
            Assert.Equal("Status must be moving", ex.Message);
        }


        [Fact]
        public async Task CreateMovement_WhenStatusMovingAndCompanyWarehouseFromEqualsCompanyWarehouseTo_ThrowBadRequestException()
        {
            MovementService sut = CreateSut();
            MovementDTO dto = new MovementDTO
            {
                CompanyWarehouseFromId = 1,
                CompanyWarehouseToId = 1,
                Status = MovementStatus.Moving,
                Nomenclatures = new List<NomenclatureInMovementDTO>()
            };
            Func<Task> act = () => sut.CreateMovement(dto);
            var ex = await Assert.ThrowsAsync<BadRequestException>(act);
            Assert.Equal("Company warehouse from and to cannot be the same", ex.Message);
        }

        [Theory]
        [InlineData(MovementStatus.Consumption, 1, null)]
        [InlineData(MovementStatus.Coming, null, 1)]
        [InlineData(MovementStatus.Moving, 2, 1)]
        public async Task CreateMovement_WhenNomenclaturesEmpty_ThrowBadRequestException(
            MovementStatus status,
            int? fromWarehouseId,
            int? toWarehouseId)
        {
            MovementService sut = CreateSut();
            MovementDTO dto = new MovementDTO
            {
                CompanyWarehouseFromId = fromWarehouseId,
                CompanyWarehouseToId = toWarehouseId,
                Status = status,
                Nomenclatures = new List<NomenclatureInMovementDTO>()
            };

            Func<Task> act = () => sut.CreateMovement(dto);
            var ex = await Assert.ThrowsAsync<BadRequestException>(act);
            Assert.Equal("Nomenclatures are required", ex.Message);
        }


        /// <summary>
        /// Проверяет, что при Consumption или Moving с не найденной номенклатурой на складе выбрасывается NotFoundException.
        /// </summary>
        [Theory]
        [InlineData(MovementStatus.Consumption, 1, null)]
        [InlineData(MovementStatus.Moving, 1, 2)]
        public async Task CreateMovement_WhenCompanyWarehouseNomenclatureNotFoundAndStatusNotComing_ThrowNotFoundException(
            MovementStatus status,
            int fromWarehouseId,
            int? toWarehouseId)
        {
            int nomenclatureId = 1;
            //Склад существует
            Mock <ICompanyWarehouseService> companyWarehouseServiceMock = new Mock<ICompanyWarehouseService>();
            companyWarehouseServiceMock
                .Setup(s => s.GetCompanyWarehouseById(fromWarehouseId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(new CompanyWarehouse { Id = fromWarehouseId, Name = "Warehouse" });


            //Номенклатура не найдена на складе
            Mock <ICompanyWareHouseNomenclatureService> companyWareHouseNomenclatureServiceMock = new Mock<ICompanyWareHouseNomenclatureService>();
            companyWareHouseNomenclatureServiceMock
                .Setup(s => s.GetCompanyWarehouseNomenclatureByCompanyWarehouseIdAndNomenclatureId(fromWarehouseId, nomenclatureId, It.IsAny<CancellationToken>()))
                .ReturnsAsync((CompanyWarehouseNomenclature?)null);
            
            MovementService sut = CreateSut(mockCompanyWarehouse: companyWarehouseServiceMock, mockCwn: companyWareHouseNomenclatureServiceMock);
            MovementDTO dto = new MovementDTO
            {
                CompanyWarehouseFromId = fromWarehouseId,
                CompanyWarehouseToId = toWarehouseId,
                Status = status,
                Nomenclatures = new List<NomenclatureInMovementDTO>
                {
                    new NomenclatureInMovementDTO { Id = nomenclatureId, Quantity = 32 }
                }
            };
            Func<Task> act = () => sut.CreateMovement(dto);

            var ex = await Assert.ThrowsAsync<NotFoundException>(act);
            Assert.Equal("Company warehouse nomenclature not found", ex.Message);
        }


        /// <summary>
        /// Проверяет, что при Consumption или Moving с найденной номенклатурой на складе, но количество которой меньше, чем количество указанное в перемещении выбрасывается BadRequestException.
        /// </summary>
        [Theory]
        [InlineData(MovementStatus.Consumption, 1, null)]
        [InlineData(MovementStatus.Moving, 1, 2)]
        public async Task CreateMovement_WhenCompanyWarehouseNomenclatureFoundButLowerQuantityThanNomenclatureQuantity_ThrowBadRequestException(
            MovementStatus status,
            int fromWarehouseId,
            int? toWarehouseId)
        {
            int nomenclatureId = 1;
            //Склад существует
            Mock <ICompanyWarehouseService> companyWarehouseServiceMock = new Mock<ICompanyWarehouseService>();
            companyWarehouseServiceMock
                .Setup(s => s.GetCompanyWarehouseById(fromWarehouseId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(new CompanyWarehouse { Id = fromWarehouseId, Name = "Warehouse" });

  
            //Номенклатура найдена на складе, но количество меньше, чем количество указанное в перемещении
            Mock <ICompanyWareHouseNomenclatureService> companyWareHouseNomenclatureServiceMock = new Mock<ICompanyWareHouseNomenclatureService>();
            companyWareHouseNomenclatureServiceMock
                .Setup(s => s.GetCompanyWarehouseNomenclatureByCompanyWarehouseIdAndNomenclatureId(fromWarehouseId, nomenclatureId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(new CompanyWarehouseNomenclature { 
                    Id = 1, 
                    CompanyWarehouseId = fromWarehouseId, 
                    NomenclatureId = nomenclatureId, 
                    Quantity = 15 });
            
            MovementService sut = CreateSut(mockCompanyWarehouse: companyWarehouseServiceMock, mockCwn: companyWareHouseNomenclatureServiceMock);
            MovementDTO dto = new MovementDTO
            {
                CompanyWarehouseFromId = fromWarehouseId,
                CompanyWarehouseToId = toWarehouseId,
                Status = status,
                Nomenclatures = new List<NomenclatureInMovementDTO>
                {
                    new NomenclatureInMovementDTO { Id = nomenclatureId, Quantity = 32 }
                }
            };
            Func<Task> act = () => sut.CreateMovement(dto);

            var ex = await Assert.ThrowsAsync<BadRequestException>(act);
            Assert.Equal("Can't write more items than are in stock.", ex.Message);
        }


        /// <summary>
        /// Проверяет, что при Coming с не найденной номенклатурой на складе создается перемещение с номенклатурами.
        /// </summary>
        [Fact]
        public async Task CreateMovement_WhenStatusComingAndCompanyWarehouseNomenclatureNotFound_MovementWithMovementNomenclaturesCreated()
        {
            int companyWarehouseId = 1;
            int nomenclatureId1 = 1;
            int quantity = 32;
            int nomenclatureId2 = 21;
            int quantity2 = 15;
            int movementId = 100;

            Mock<ICompanyWarehouseService> companyWarehouseServiceMock = new Mock<ICompanyWarehouseService>();
            companyWarehouseServiceMock
                .Setup(s => s.GetCompanyWarehouseById(companyWarehouseId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(new CompanyWarehouse { Id = companyWarehouseId, Name = "Warehouse" });

            Mock<ICompanyWareHouseNomenclatureService> companyWareHouseNomenclatureServiceMock = new Mock<ICompanyWareHouseNomenclatureService>();
            companyWareHouseNomenclatureServiceMock
                .Setup(s => s.GetCompanyWarehouseNomenclatureByCompanyWarehouseIdAndNomenclatureId(companyWarehouseId, nomenclatureId1, It.IsAny<CancellationToken>()))
                .ReturnsAsync((CompanyWarehouseNomenclature?)null);
            companyWareHouseNomenclatureServiceMock
                .Setup(s => s.GetCompanyWarehouseNomenclatureByCompanyWarehouseIdAndNomenclatureId(companyWarehouseId, nomenclatureId2, It.IsAny<CancellationToken>()))
                .ReturnsAsync((CompanyWarehouseNomenclature?)null);
            companyWareHouseNomenclatureServiceMock
                .Setup(s => s.CreateCompanyWarehouseNomenclature(It.IsAny<CompanyWarehouseNomenclature>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync((CompanyWarehouseNomenclature cwn, CancellationToken _) => cwn);

            Movement createdMovement = new Movement
            {
                Id = movementId,
                CompanyWarehouseFromId = null,
                CompanyWarehouseToId = companyWarehouseId,
                Status = MovementStatus.Coming,
                Date = DateTime.UtcNow
            };
            Mock<IMovementRepository> mockMovementRepo = new Mock<IMovementRepository>();
            mockMovementRepo
                .Setup(r => r.CreateMovement(It.IsAny<Movement>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(createdMovement);
            mockMovementRepo
                .Setup(r => r.GetMovementById(movementId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(createdMovement);

            Mock<IMovementNomenclatureService> mockMovementNomenclatureServiceMock = new Mock<IMovementNomenclatureService>();
            mockMovementNomenclatureServiceMock
                .Setup(s => s.CreateMovementNomenclature(It.IsAny<MovementNomenclature>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync((MovementNomenclature mn, CancellationToken _) => mn);

            MovementService sut = CreateSut(
                mockMovementRepo: mockMovementRepo,
                mockCompanyWarehouse: companyWarehouseServiceMock,
                mockCwn: companyWareHouseNomenclatureServiceMock,
                mockMn: mockMovementNomenclatureServiceMock);
            MovementDTO dto = new MovementDTO
            {
                CompanyWarehouseFromId = null,
                CompanyWarehouseToId = companyWarehouseId,
                Status = MovementStatus.Coming,
                Nomenclatures = new List<NomenclatureInMovementDTO>
                {
                    new NomenclatureInMovementDTO { Id = nomenclatureId1, Quantity = quantity },
                    new NomenclatureInMovementDTO { Id = nomenclatureId2, Quantity = quantity2 }
                }
            };

            Movement result = await sut.CreateMovement(dto);

            Assert.NotNull(result);
            Assert.Equal(movementId, result.Id);

            mockMovementNomenclatureServiceMock.Verify(
                s => s.CreateMovementNomenclature(
                    It.Is<MovementNomenclature>(mn =>
                        mn.MovementId == movementId &&
                        mn.NomenclatureId == nomenclatureId1 &&
                        mn.Quantity == quantity),
                    It.IsAny<CancellationToken>()),
                Times.Once);
            mockMovementNomenclatureServiceMock.Verify(
                s => s.CreateMovementNomenclature(
                    It.Is<MovementNomenclature>(mn =>
                        mn.MovementId == movementId &&
                        mn.NomenclatureId == nomenclatureId2 &&
                        mn.Quantity == quantity2),
                    It.IsAny<CancellationToken>()),
                Times.Once);

            companyWareHouseNomenclatureServiceMock.Verify(
                s => s.CreateCompanyWarehouseNomenclature(
                    It.Is<CompanyWarehouseNomenclature>(cwn =>
                        cwn.CompanyWarehouseId == companyWarehouseId &&
                        cwn.NomenclatureId == nomenclatureId1 &&
                        cwn.Quantity == quantity),
                    It.IsAny<CancellationToken>()),
                Times.Once);
            companyWareHouseNomenclatureServiceMock.Verify(
                s => s.CreateCompanyWarehouseNomenclature(
                    It.Is<CompanyWarehouseNomenclature>(cwn =>
                        cwn.CompanyWarehouseId == companyWarehouseId &&
                        cwn.NomenclatureId == nomenclatureId2 &&
                        cwn.Quantity == quantity2),
                    It.IsAny<CancellationToken>()),
                Times.Once);
        }
        


        [Fact]
        public async Task CreateMovement_WhenStatusComingAndCompanyWarehouseNomenclatureWasFound_MovementWithMovementNomenclaturesCreated()
        {
            int companyWarehouseId = 1;
            int nomenclatureId = 1;
            int quantity = 32;
            int currentQuantity = 10;
            int movementId = 100;
            int companyWarehouseNomenclatureId = 1;

            Mock<ICompanyWarehouseService> companyWarehouseServiceMock = new Mock<ICompanyWarehouseService>();
            companyWarehouseServiceMock
                .Setup(s => s.GetCompanyWarehouseById(companyWarehouseId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(new CompanyWarehouse { Id = companyWarehouseId, Name = "Warehouse" });

            Mock<ICompanyWareHouseNomenclatureService> companyWareHouseNomenclatureServiceMock = new Mock<ICompanyWareHouseNomenclatureService>();
            companyWareHouseNomenclatureServiceMock
                .Setup(s => s.GetCompanyWarehouseNomenclatureByCompanyWarehouseIdAndNomenclatureId(companyWarehouseId, nomenclatureId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(new CompanyWarehouseNomenclature { 
                    Id = 1, 
                    CompanyWarehouseId = companyWarehouseId, 
                    NomenclatureId = nomenclatureId, 
                    Quantity = currentQuantity });

            Movement createdMovement = new Movement
            {
                Id = movementId,
                CompanyWarehouseFromId = null,
                CompanyWarehouseToId = companyWarehouseId,
                Status = MovementStatus.Coming,
                Date = DateTime.UtcNow
            };
            Mock<IMovementRepository> mockMovementRepo = new Mock<IMovementRepository>();
            mockMovementRepo
                .Setup(r => r.CreateMovement(It.IsAny<Movement>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(createdMovement);
            mockMovementRepo
                .Setup(r => r.GetMovementById(movementId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(createdMovement);

            Mock<IMovementNomenclatureService> mockMovementNomenclatureServiceMock = new Mock<IMovementNomenclatureService>();
            mockMovementNomenclatureServiceMock
                .Setup(s => s.CreateMovementNomenclature(It.IsAny<MovementNomenclature>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync((MovementNomenclature mn, CancellationToken _) => mn);

            MovementService sut = CreateSut(
                mockMovementRepo: mockMovementRepo,
                mockCompanyWarehouse: companyWarehouseServiceMock,
                mockCwn: companyWareHouseNomenclatureServiceMock,
                mockMn: mockMovementNomenclatureServiceMock);
            MovementDTO dto = new MovementDTO
            {
                CompanyWarehouseFromId = null,
                CompanyWarehouseToId = companyWarehouseId,
                Status = MovementStatus.Coming,
                Nomenclatures = new List<NomenclatureInMovementDTO>
                {
                    new NomenclatureInMovementDTO { Id = nomenclatureId, Quantity = quantity },
                }
            };

            Movement result = await sut.CreateMovement(dto);

            Assert.NotNull(result);
            Assert.Equal(movementId, result.Id);

            mockMovementNomenclatureServiceMock.Verify(
                s => s.CreateMovementNomenclature(
                    It.Is<MovementNomenclature>(mn =>
                        mn.MovementId == movementId &&
                        mn.NomenclatureId == nomenclatureId &&
                        mn.Quantity == quantity),
                    It.IsAny<CancellationToken>()),
                Times.Once);

            companyWareHouseNomenclatureServiceMock.Verify(
                s => s.UpdateCompanyWarehouseNomenclature(
                    It.Is<int>(id => id == companyWarehouseNomenclatureId),
                    It.Is<CompanyWarehouseNomenclature>(cwn =>
                        cwn.CompanyWarehouseId == companyWarehouseId &&
                        cwn.NomenclatureId == nomenclatureId &&
                        cwn.Quantity == quantity + currentQuantity),
                    It.IsAny<CancellationToken>()),
                Times.Once);
        }

        [Fact]
        public async Task CreateMovement_WhenStatusConsumption_MovementWithMovementNomenclaturesCreated()
        {
            int companyWarehouseId = 1;
            int nomenclatureId = 1;
            int quantity = 2;
            int currentQuantity = 10;
            int movementId = 100;
            int companyWarehouseNomenclatureId = 1;

            Mock<ICompanyWarehouseService> companyWarehouseServiceMock = new Mock<ICompanyWarehouseService>();
            companyWarehouseServiceMock
                .Setup(s => s.GetCompanyWarehouseById(companyWarehouseId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(new CompanyWarehouse { Id = companyWarehouseId, Name = "Warehouse" });
            
            Mock<ICompanyWareHouseNomenclatureService> companyWareHouseNomenclatureServiceMock = new Mock<ICompanyWareHouseNomenclatureService>();
            companyWareHouseNomenclatureServiceMock
                .Setup(s => s.GetCompanyWarehouseNomenclatureByCompanyWarehouseIdAndNomenclatureId(companyWarehouseId, nomenclatureId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(new CompanyWarehouseNomenclature { 
                    Id = 1, 
                    CompanyWarehouseId = companyWarehouseId, 
                    NomenclatureId = nomenclatureId, 
                    Quantity = currentQuantity });

            Movement createdMovement = new Movement
            {
                Id = movementId,
                CompanyWarehouseFromId = companyWarehouseId,
                CompanyWarehouseToId = null,
                Status = MovementStatus.Consumption,
                Date = DateTime.UtcNow
            };
            Mock<IMovementRepository> mockMovementRepo = new Mock<IMovementRepository>();
            mockMovementRepo
                .Setup(r => r.CreateMovement(It.IsAny<Movement>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(createdMovement);
            mockMovementRepo
                .Setup(r => r.GetMovementById(movementId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(createdMovement);
                
            Mock<IMovementNomenclatureService> mockMovementNomenclatureServiceMock = new Mock<IMovementNomenclatureService>();
            mockMovementNomenclatureServiceMock
                .Setup(s => s.CreateMovementNomenclature(It.IsAny<MovementNomenclature>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync((MovementNomenclature mn, CancellationToken _) => mn);

            MovementService sut = CreateSut(
                mockMovementRepo: mockMovementRepo,
                mockCompanyWarehouse: companyWarehouseServiceMock,
                mockCwn: companyWareHouseNomenclatureServiceMock,
                mockMn: mockMovementNomenclatureServiceMock);
            MovementDTO dto = new MovementDTO
            {
                CompanyWarehouseFromId = companyWarehouseId,
                CompanyWarehouseToId = null,
                Status = MovementStatus.Consumption,
                Nomenclatures = new List<NomenclatureInMovementDTO>
                {
                    new NomenclatureInMovementDTO { Id = nomenclatureId, Quantity = quantity },
                }
            };

            Movement result = await sut.CreateMovement(dto);

            Assert.NotNull(result);
            Assert.Equal(movementId, result.Id);

            mockMovementNomenclatureServiceMock.Verify(
                s => s.CreateMovementNomenclature(
                    It.Is<MovementNomenclature>(mn =>
                        mn.MovementId == movementId &&
                        mn.NomenclatureId == nomenclatureId &&
                        mn.Quantity == quantity),
                    It.IsAny<CancellationToken>()),
                Times.Once);

            companyWareHouseNomenclatureServiceMock.Verify(
                s => s.UpdateCompanyWarehouseNomenclature(
                    It.Is<int>(id => id == companyWarehouseNomenclatureId),
                    It.Is<CompanyWarehouseNomenclature>(cwn =>
                        cwn.CompanyWarehouseId == companyWarehouseId &&
                        cwn.NomenclatureId == nomenclatureId &&
                        cwn.Quantity == currentQuantity - quantity),
                    It.IsAny<CancellationToken>()),
                Times.Once);
        }

        [Fact]
        public async Task CreateMovement_WhenStatusMovingAndCompanyWarehouseNomenclatureToNotFound_MovementWithMovementNomenclaturesCreated()
        {
            int companyWarehouseFromId = 1;
            int companyWarehouseToId = 2;
            int nomenclatureId = 1;
            int quantity = 3;
            int currentQuantityFrom = 10;
            int movementId = 100;

            Mock<ICompanyWarehouseService> companyWarehouseServiceMock = new Mock<ICompanyWarehouseService>();
            companyWarehouseServiceMock
                .Setup(s => s.GetCompanyWarehouseById(companyWarehouseFromId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(new CompanyWarehouse { Id = companyWarehouseFromId, Name = "Warehouse From" });
            companyWarehouseServiceMock
                .Setup(s => s.GetCompanyWarehouseById(companyWarehouseToId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(new CompanyWarehouse { Id = companyWarehouseToId, Name = "Warehouse To" });

            Mock<ICompanyWareHouseNomenclatureService> companyWareHouseNomenclatureServiceMock = new Mock<ICompanyWareHouseNomenclatureService>();
            companyWareHouseNomenclatureServiceMock
                .Setup(s => s.GetCompanyWarehouseNomenclatureByCompanyWarehouseIdAndNomenclatureId(companyWarehouseToId, nomenclatureId, It.IsAny<CancellationToken>()))
                .ReturnsAsync((CompanyWarehouseNomenclature?)null);
            companyWareHouseNomenclatureServiceMock
                .Setup(s => s.GetCompanyWarehouseNomenclatureByCompanyWarehouseIdAndNomenclatureId(companyWarehouseFromId, nomenclatureId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(new CompanyWarehouseNomenclature {
                    Id = 1,
                    CompanyWarehouseId = companyWarehouseFromId,
                    NomenclatureId = nomenclatureId,
                    Quantity = currentQuantityFrom });
            companyWareHouseNomenclatureServiceMock
                .Setup(s => s.CreateCompanyWarehouseNomenclature(It.IsAny<CompanyWarehouseNomenclature>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync((CompanyWarehouseNomenclature cwn, CancellationToken _) => cwn);
            Mock<IMovementNomenclatureService> mockMovementNomenclatureServiceMock = new Mock<IMovementNomenclatureService>();
            mockMovementNomenclatureServiceMock
                .Setup(s => s.CreateMovementNomenclature(It.IsAny<MovementNomenclature>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync((MovementNomenclature mn, CancellationToken _) => mn);

            Movement createdMovement = new Movement
            {
                Id = movementId,
                CompanyWarehouseFromId = companyWarehouseFromId,
                CompanyWarehouseToId = companyWarehouseToId,
                Status = MovementStatus.Moving,
                Date = DateTime.UtcNow
            };
            Mock<IMovementRepository> mockMovementRepo = new Mock<IMovementRepository>();
            mockMovementRepo
                .Setup(r => r.CreateMovement(It.IsAny<Movement>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(createdMovement);
            mockMovementRepo
                .Setup(r => r.GetMovementById(movementId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(createdMovement);

            MovementService sut = CreateSut(
                mockMovementRepo: mockMovementRepo,
                mockCompanyWarehouse: companyWarehouseServiceMock,
                mockCwn: companyWareHouseNomenclatureServiceMock,
                mockMn: mockMovementNomenclatureServiceMock);
            MovementDTO dto = new MovementDTO
            {
                CompanyWarehouseFromId = companyWarehouseFromId,
                CompanyWarehouseToId = companyWarehouseToId,
                Status = MovementStatus.Moving,
                Nomenclatures = new List<NomenclatureInMovementDTO>
                {
                    new NomenclatureInMovementDTO { Id = nomenclatureId, Quantity = quantity },
                }
            };

            Movement result = await sut.CreateMovement(dto);

            Assert.NotNull(result);
            Assert.Equal(movementId, result.Id);

            mockMovementNomenclatureServiceMock.Verify(
                s => s.CreateMovementNomenclature(
                    It.Is<MovementNomenclature>(mn =>
                        mn.MovementId == movementId &&
                        mn.NomenclatureId == nomenclatureId &&
                        mn.Quantity == quantity),
                    It.IsAny<CancellationToken>()),
                Times.Once);

            companyWareHouseNomenclatureServiceMock.Verify(
                s => s.UpdateCompanyWarehouseNomenclature(
                    It.Is<int>(id => id == 1),
                    It.Is<CompanyWarehouseNomenclature>(cwn =>
                        cwn.CompanyWarehouseId == companyWarehouseFromId &&
                        cwn.NomenclatureId == nomenclatureId &&
                        cwn.Quantity == currentQuantityFrom - quantity),
                    It.IsAny<CancellationToken>()),
                Times.Once);

            companyWareHouseNomenclatureServiceMock.Verify(
                s => s.CreateCompanyWarehouseNomenclature(
                    It.Is<CompanyWarehouseNomenclature>(cwn =>
                        cwn.CompanyWarehouseId == companyWarehouseToId &&
                        cwn.NomenclatureId == nomenclatureId &&
                        cwn.Quantity == quantity),
                    It.IsAny<CancellationToken>()),
                Times.Once);
        }

        [Fact]
        public async Task CreateMovement_WhenStatusMovingAndCompanyWarehouseNomenclatureToWasFound_MovementWithMovementNomenclaturesCreated()
        {
            int companyWarehouseFromId = 1;
            int companyWarehouseToId = 2;
            int currentQuantityTo = 10;
            int currentQuantityFrom = 15;
            int quantity = 3;
            int nomenclatureId1 = 1;
            int companyWarehouseNomenclatureIdTo = 1;
            int companyWarehouseNomenclatureIdFrom = 2;
            int movementId = 100;

            Mock<ICompanyWarehouseService> companyWarehouseServiceMock = new Mock<ICompanyWarehouseService>();
            companyWarehouseServiceMock
                .Setup(s => s.GetCompanyWarehouseById(companyWarehouseFromId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(new CompanyWarehouse { Id = companyWarehouseFromId, Name = "Warehouse From" });
            companyWarehouseServiceMock
                .Setup(s => s.GetCompanyWarehouseById(companyWarehouseToId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(new CompanyWarehouse { Id = companyWarehouseToId, Name = "Warehouse To" });

            Mock<ICompanyWareHouseNomenclatureService> companyWareHouseNomenclatureServiceMock = new Mock<ICompanyWareHouseNomenclatureService>();
            companyWareHouseNomenclatureServiceMock
                .Setup(s => s.GetCompanyWarehouseNomenclatureByCompanyWarehouseIdAndNomenclatureId(companyWarehouseToId, nomenclatureId1, It.IsAny<CancellationToken>()))
                .ReturnsAsync(new CompanyWarehouseNomenclature {
                    Id = companyWarehouseNomenclatureIdTo,
                    CompanyWarehouseId = companyWarehouseToId,
                    NomenclatureId = nomenclatureId1,
                    Quantity = currentQuantityTo });
            companyWareHouseNomenclatureServiceMock
                .Setup(s => s.GetCompanyWarehouseNomenclatureByCompanyWarehouseIdAndNomenclatureId(companyWarehouseFromId, nomenclatureId1, It.IsAny<CancellationToken>()))
                .ReturnsAsync(new CompanyWarehouseNomenclature {
                    Id = companyWarehouseNomenclatureIdFrom,
                    CompanyWarehouseId = companyWarehouseFromId,
                    NomenclatureId = nomenclatureId1,
                    Quantity = currentQuantityFrom });
            companyWareHouseNomenclatureServiceMock
                .Setup(s => s.CreateCompanyWarehouseNomenclature(It.IsAny<CompanyWarehouseNomenclature>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync((CompanyWarehouseNomenclature cwn, CancellationToken _) => cwn);
            Mock<IMovementNomenclatureService> mockMovementNomenclatureServiceMock = new Mock<IMovementNomenclatureService>();
            mockMovementNomenclatureServiceMock
                .Setup(s => s.CreateMovementNomenclature(It.IsAny<MovementNomenclature>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync((MovementNomenclature mn, CancellationToken _) => mn);

            Movement createdMovement = new Movement
            {
                Id = movementId,
                CompanyWarehouseFromId = companyWarehouseFromId,
                CompanyWarehouseToId = companyWarehouseToId,
                Status = MovementStatus.Moving,
                Date = DateTime.UtcNow
            };
            Mock<IMovementRepository> mockMovementRepo = new Mock<IMovementRepository>();
            mockMovementRepo
                .Setup(r => r.CreateMovement(It.IsAny<Movement>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(createdMovement);
            mockMovementRepo
                .Setup(r => r.GetMovementById(movementId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(createdMovement);

            MovementService sut = CreateSut(
                mockMovementRepo: mockMovementRepo,
                mockCompanyWarehouse: companyWarehouseServiceMock,
                mockCwn: companyWareHouseNomenclatureServiceMock,
                mockMn: mockMovementNomenclatureServiceMock);
            MovementDTO dto = new MovementDTO
            {
                CompanyWarehouseFromId = companyWarehouseFromId,
                CompanyWarehouseToId = companyWarehouseToId,
                Status = MovementStatus.Moving,
                Nomenclatures = new List<NomenclatureInMovementDTO>
                {
                    new NomenclatureInMovementDTO { Id = nomenclatureId1, Quantity = quantity },
                }
            };

            Movement result = await sut.CreateMovement(dto);

            Assert.NotNull(result);
            Assert.Equal(movementId, result.Id);

            mockMovementNomenclatureServiceMock.Verify(
                s => s.CreateMovementNomenclature(
                    It.Is<MovementNomenclature>(mn =>
                        mn.MovementId == movementId &&
                        mn.NomenclatureId == nomenclatureId1 &&
                        mn.Quantity == quantity),
                    It.IsAny<CancellationToken>()),
                Times.Once);

            companyWareHouseNomenclatureServiceMock.Verify(
                s => s.UpdateCompanyWarehouseNomenclature(
                    It.Is<int>(id => id == companyWarehouseNomenclatureIdFrom),
                    It.Is<CompanyWarehouseNomenclature>(cwn =>
                        cwn.CompanyWarehouseId == companyWarehouseFromId &&
                        cwn.NomenclatureId == nomenclatureId1 &&
                        cwn.Quantity == currentQuantityFrom - quantity),
                    It.IsAny<CancellationToken>()),
                Times.Once);

            companyWareHouseNomenclatureServiceMock.Verify(
                s => s.UpdateCompanyWarehouseNomenclature(
                    It.Is<int>(id => id == companyWarehouseNomenclatureIdTo),
                    It.Is<CompanyWarehouseNomenclature>(cwn =>
                        cwn.CompanyWarehouseId == companyWarehouseToId &&
                        cwn.NomenclatureId == nomenclatureId1 &&
                        cwn.Quantity == currentQuantityTo + quantity),
                    It.IsAny<CancellationToken>()),
                Times.Once);
        }
    }
}
