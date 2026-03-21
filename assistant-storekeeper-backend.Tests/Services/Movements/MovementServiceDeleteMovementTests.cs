using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using assistant_storekeeper_backend.Exceptions;
using assistant_storekeeper_backend.Models;
using assistant_storekeeper_backend.Repositories.Movements;
using assistant_storekeeper_backend.Services.CompanyWareHouseNomenclatures;
using assistant_storekeeper_backend.Services.CompanyWarehouses;
using assistant_storekeeper_backend.Services.MovementNomenclatures;
using assistant_storekeeper_backend.Services.Movements;
using Moq;
using Xunit;

namespace assistant_storekeeper_backend.Tests.Services.Movements
{
    public class MovementServiceDeleteMovementTests
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
        public async Task DeleteMovement_WhenMovementNotFound_ThrowsNotFoundException()
        {
            int movementId = 1;
            Mock<IMovementRepository> mockMovementRepo = new Mock<IMovementRepository>();
            mockMovementRepo
                .Setup(r => r.GetMovementById(movementId, It.IsAny<CancellationToken>()))
                .ReturnsAsync((Movement?)null);

            MovementService sut = CreateSut(mockMovementRepo: mockMovementRepo);

            Func<Task> act = () => sut.DeleteMovement(movementId);

            var ex = await Assert.ThrowsAsync<NotFoundException>(act);
            Assert.Equal("Movement not found", ex.Message);
        }

        [Fact]
        public async Task DeleteMovement_WhenStatusComingAndCompanyWarehouseNomenclatureNotFound_ThrowsNotFoundException()
        {
            int movementId = 1;
            int companyWarehouseToId = 1;
            int nomenclatureId = 10;
            int movementNomenclatureId = 100;
            var movementNomenclatures = new List<MovementNomenclature>
            {
                new MovementNomenclature { Id = movementNomenclatureId, MovementId = movementId, NomenclatureId = nomenclatureId, Quantity = 5 }
            };
            var existingMovement = new Movement
            {
                Id = movementId,
                CompanyWarehouseFromId = null,
                CompanyWarehouseToId = companyWarehouseToId,
                Status = MovementStatus.Coming,
                Date = DateTime.UtcNow
            };

            Mock<IMovementRepository> mockMovementRepo = new Mock<IMovementRepository>();
            mockMovementRepo
                .Setup(r => r.GetMovementById(movementId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(existingMovement);

            Mock<IMovementNomenclatureService> mockMovementNomenclatureService = new Mock<IMovementNomenclatureService>();
            mockMovementNomenclatureService
                .Setup(s => s.GetAllMovementNomenclatures(movementId, null, null, null, null, null, It.IsAny<CancellationToken>()))
                .ReturnsAsync(movementNomenclatures);

            Mock<ICompanyWareHouseNomenclatureService> mockCompanyWareHouseNomenclatureService = new Mock<ICompanyWareHouseNomenclatureService>();
            mockCompanyWareHouseNomenclatureService
                .Setup(s => s.GetCompanyWarehouseNomenclatureByCompanyWarehouseIdAndNomenclatureId(companyWarehouseToId, nomenclatureId, It.IsAny<CancellationToken>()))
                .ReturnsAsync((CompanyWarehouseNomenclature?)null);

            MovementService sut = CreateSut(
                mockMovementRepo: mockMovementRepo, 
                mockCwn: mockCompanyWareHouseNomenclatureService, 
                mockMn: mockMovementNomenclatureService);

            Func<Task> act = () => sut.DeleteMovement(movementId);

            var ex = await Assert.ThrowsAsync<NotFoundException>(act);
            Assert.Equal("Company warehouse nomenclature not found", ex.Message);
        }

        [Fact]
        public async Task DeleteMovement_WhenStatusConsumptionAndCompanyWarehouseNomenclatureNotFound_ThrowsNotFoundException()
        {
            int movementId = 1;
            int companyWarehouseFromId = 1;
            int nomenclatureId = 10;
            int movementNomenclatureId = 100;
            var movementNomenclatures = new List<MovementNomenclature>
            {
                new MovementNomenclature { Id = movementNomenclatureId, MovementId = movementId, NomenclatureId = nomenclatureId, Quantity = 5 }
            };
            var existingMovement = new Movement
            {
                Id = movementId,
                CompanyWarehouseFromId = companyWarehouseFromId,
                CompanyWarehouseToId = null,
                Status = MovementStatus.Consumption,
                Date = DateTime.UtcNow
            };

            Mock<IMovementRepository> mockMovementRepo = new Mock<IMovementRepository>();
            mockMovementRepo
                .Setup(r => r.GetMovementById(movementId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(existingMovement);

            Mock<IMovementNomenclatureService> mockMovementNomenclatureService = new Mock<IMovementNomenclatureService>();
            mockMovementNomenclatureService
                .Setup(s => s.GetAllMovementNomenclatures(movementId, null, null, null, null, null, It.IsAny<CancellationToken>()))
                .ReturnsAsync(movementNomenclatures);

            Mock<ICompanyWareHouseNomenclatureService> mockCompanyWareHouseNomenclatureService = new Mock<ICompanyWareHouseNomenclatureService>();
            mockCompanyWareHouseNomenclatureService
                .Setup(s => s.GetCompanyWarehouseNomenclatureByCompanyWarehouseIdAndNomenclatureId(companyWarehouseFromId, nomenclatureId, It.IsAny<CancellationToken>()))
                .ReturnsAsync((CompanyWarehouseNomenclature?)null);

            MovementService sut = CreateSut(
                mockMovementRepo: mockMovementRepo, 
                mockCwn: mockCompanyWareHouseNomenclatureService, 
                mockMn: mockMovementNomenclatureService);

            Func<Task> act = () => sut.DeleteMovement(movementId);

            var ex = await Assert.ThrowsAsync<NotFoundException>(act);
            Assert.Equal("Company warehouse nomenclature not found", ex.Message);
        }

        [Fact]
        public async Task DeleteMovement_WhenStatusMovingAndCompanyWarehouseNomenclatureFromNotFound_ThrowsNotFoundException()
        {
            int movementId = 1;
            int companyWarehouseFromId = 1;
            int companyWarehouseToId = 2;
            int nomenclatureId = 10;
            int movementNomenclatureId = 100;
            var movementNomenclatures = new List<MovementNomenclature>
            {
                new MovementNomenclature { Id = movementNomenclatureId, MovementId = movementId, NomenclatureId = nomenclatureId, Quantity = 5 }
            };
            var existingMovement = new Movement
            {
                Id = movementId,
                CompanyWarehouseFromId = companyWarehouseFromId,
                CompanyWarehouseToId = companyWarehouseToId,
                Status = MovementStatus.Moving,
                Date = DateTime.UtcNow
            };

            Mock<IMovementRepository> mockMovementRepo = new Mock<IMovementRepository>();
            mockMovementRepo
                .Setup(r => r.GetMovementById(movementId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(existingMovement);

            Mock<IMovementNomenclatureService> mockMovementNomenclatureService = new Mock<IMovementNomenclatureService>();
            mockMovementNomenclatureService
                .Setup(s => s.GetAllMovementNomenclatures(movementId, null, null, null, null, null, It.IsAny<CancellationToken>()))
                .ReturnsAsync(movementNomenclatures);

            Mock<ICompanyWareHouseNomenclatureService> mockCompanyWareHouseNomenclatureService = new Mock<ICompanyWareHouseNomenclatureService>();
            mockCompanyWareHouseNomenclatureService
                .Setup(s => s.GetCompanyWarehouseNomenclatureByCompanyWarehouseIdAndNomenclatureId(companyWarehouseFromId, nomenclatureId, It.IsAny<CancellationToken>()))
                .ReturnsAsync((CompanyWarehouseNomenclature?)null);

            MovementService sut = CreateSut(
                mockMovementRepo: mockMovementRepo, 
                mockCwn: mockCompanyWareHouseNomenclatureService, 
                mockMn: mockMovementNomenclatureService);

            Func<Task> act = () => sut.DeleteMovement(movementId);

            var ex = await Assert.ThrowsAsync<NotFoundException>(act);
            Assert.Equal("Company warehouse nomenclature from not found", ex.Message);
        }

        [Fact]
        public async Task DeleteMovement_WhenStatusMovingAndCompanyWarehouseNomenclatureToNotFound_ThrowsNotFoundException()
        {
            int movementId = 1;
            int companyWarehouseFromId = 1;
            int companyWarehouseToId = 2;
            int nomenclatureId = 10;
            int movementNomenclatureId = 100;
            int cwnFromId = 1;
            var movementNomenclatures = new List<MovementNomenclature>
            {
                new MovementNomenclature { Id = movementNomenclatureId, MovementId = movementId, NomenclatureId = nomenclatureId, Quantity = 5 }
            };
            var existingMovement = new Movement
            {
                Id = movementId,
                CompanyWarehouseFromId = companyWarehouseFromId,
                CompanyWarehouseToId = companyWarehouseToId,
                Status = MovementStatus.Moving,
                Date = DateTime.UtcNow
            };
            var companyWarehouseNomenclatureFrom = new CompanyWarehouseNomenclature { 
                Id = cwnFromId, 
                CompanyWarehouseId = companyWarehouseFromId, 
                NomenclatureId = nomenclatureId, 
                Quantity = 10 };

            Mock<IMovementRepository> mockMovementRepo = new Mock<IMovementRepository>();
            mockMovementRepo
                .Setup(r => r.GetMovementById(movementId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(existingMovement);

            Mock<IMovementNomenclatureService> mockMovementNomenclatureService = new Mock<IMovementNomenclatureService>();
            mockMovementNomenclatureService
                .Setup(s => s.GetAllMovementNomenclatures(movementId, null, null, null, null, null, It.IsAny<CancellationToken>()))
                .ReturnsAsync(movementNomenclatures);

            Mock<ICompanyWareHouseNomenclatureService> mockCompanyWareHouseNomenclatureService = new Mock<ICompanyWareHouseNomenclatureService>();
            mockCompanyWareHouseNomenclatureService
                .Setup(s => s.GetCompanyWarehouseNomenclatureByCompanyWarehouseIdAndNomenclatureId(companyWarehouseFromId, nomenclatureId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(companyWarehouseNomenclatureFrom);
            mockCompanyWareHouseNomenclatureService
                .Setup(s => s.GetCompanyWarehouseNomenclatureByCompanyWarehouseIdAndNomenclatureId(companyWarehouseToId, nomenclatureId, It.IsAny<CancellationToken>()))
                .ReturnsAsync((CompanyWarehouseNomenclature?)null);

            MovementService sut = CreateSut(
                mockMovementRepo: mockMovementRepo, 
                mockCwn: mockCompanyWareHouseNomenclatureService, 
                mockMn: mockMovementNomenclatureService);

            Func<Task> act = () => sut.DeleteMovement(movementId);

            var ex = await Assert.ThrowsAsync<NotFoundException>(act);
            Assert.Equal("Company warehouse nomenclature to not found", ex.Message);
        }

        [Fact]
        public async Task DeleteMovement_WhenStatusComing_Success()
        {
            int movementId = 1;
            int companyWarehouseToId = 1;
            int nomenclatureId = 10;
            int movementNomenclatureId = 100;
            int cwnId = 1;
            int quantity = 5;
            int currentQuantity = 20;
            var movementNomenclatures = new List<MovementNomenclature>
            {
                new MovementNomenclature { Id = movementNomenclatureId, MovementId = movementId, NomenclatureId = nomenclatureId, Quantity = quantity }
            };
            var existingMovement = new Movement
            {
                Id = movementId,
                CompanyWarehouseFromId = null,
                CompanyWarehouseToId = companyWarehouseToId,
                Status = MovementStatus.Coming,
                Date = DateTime.UtcNow
            };
            var companyWarehouseNomenclatureTo = new CompanyWarehouseNomenclature { 
                Id = cwnId, 
                CompanyWarehouseId = companyWarehouseToId, 
                NomenclatureId = nomenclatureId, 
                Quantity = currentQuantity };

            Mock<IMovementRepository> mockMovementRepo = new Mock<IMovementRepository>();
            mockMovementRepo
                .Setup(r => r.GetMovementById(movementId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(existingMovement);

            Mock<IMovementNomenclatureService> mockMovementNomenclatureService = new Mock<IMovementNomenclatureService>();
            mockMovementNomenclatureService
                .Setup(s => s.GetAllMovementNomenclatures(movementId, null, null, null, null, null, It.IsAny<CancellationToken>()))
                .ReturnsAsync(movementNomenclatures);

            Mock<ICompanyWareHouseNomenclatureService> mockCompanyWareHouseNomenclatureService = new Mock<ICompanyWareHouseNomenclatureService>();
            mockCompanyWareHouseNomenclatureService
                .Setup(s => s.GetCompanyWarehouseNomenclatureByCompanyWarehouseIdAndNomenclatureId(companyWarehouseToId, nomenclatureId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(cwn);

            MovementService sut = CreateSut(mockMovementRepo: mockMovementRepo, 
                mockCwn: mockCompanyWareHouseNomenclatureService, 
                mockMn: mockMovementNomenclatureService);

            await sut.DeleteMovement(movementId);

            mockCompanyWareHouseNomenclatureService.Verify(
                s => s.UpdateCompanyWarehouseNomenclature(
                    It.Is<int>(id => id == cwnId),
                    It.Is<CompanyWarehouseNomenclature>(c => c.Quantity == currentQuantity - quantity),
                    It.IsAny<CancellationToken>()),
                Times.Once);
                
            mockMovementNomenclatureService.Verify(
                s => s.DeleteMovementNomenclature(
                    movementNomenclatureId, It.IsAny<CancellationToken>()),
                     Times.Once);

            mockMovementRepository.Verify(
                r => r.DeleteMovement(existingMovement, It.IsAny<CancellationToken>()),
                Times.Once);
        }

        [Fact]
        public async Task DeleteMovement_WhenStatusConsumption_Success()
        {
            int movementId = 1;
            int companyWarehouseFromId = 1;
            int nomenclatureId = 10;
            int movementNomenclatureId = 100;
            int cwnId = 1;
            int quantity = 5;
            int currentQuantity = 20;
            var movementNomenclatures = new List<MovementNomenclature>
            {
                new MovementNomenclature { Id = movementNomenclatureId, MovementId = movementId, NomenclatureId = nomenclatureId, Quantity = quantity }
            };
            var existingMovement = new Movement
            {
                Id = movementId,
                CompanyWarehouseFromId = companyWarehouseFromId,
                CompanyWarehouseToId = null,
                Status = MovementStatus.Consumption,
                Date = DateTime.UtcNow
            };
            var companyWarehouseNomenclatureFrom = new CompanyWarehouseNomenclature { 
                Id = cwnId, 
                CompanyWarehouseId = companyWarehouseFromId, 
                NomenclatureId = nomenclatureId, 
                Quantity = currentQuantity };

            Mock<IMovementRepository> mockMovementRepo = new Mock<IMovementRepository>();
            mockMovementRepo
                .Setup(r => r.GetMovementById(movementId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(existingMovement);

            Mock<IMovementNomenclatureService> mockMovementNomenclatureService = new Mock<IMovementNomenclatureService>();
            mockMovementNomenclatureService
                .Setup(s => s.GetAllMovementNomenclatures(movementId, null, null, null, null, null, It.IsAny<CancellationToken>()))
                .ReturnsAsync(movementNomenclatures);

            Mock<ICompanyWareHouseNomenclatureService> mockCompanyWareHouseNomenclatureService = new Mock<ICompanyWareHouseNomenclatureService>();
            mockCompanyWarehouseNomenclatureService
                .Setup(s => s.GetCompanyWarehouseNomenclatureByCompanyWarehouseIdAndNomenclatureId(companyWarehouseFromId, nomenclatureId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(cwn);

            MovementService sut = CreateSut(
                mockMovementRepo: mockMovementRepo, 
                mockCwn: mockCompanyWareHouseNomenclatureService, 
                mockMn: mockMovementNomenclatureService);

            await sut.DeleteMovement(movementId);

            mockCompanyWareHouseNomenclatureService.Verify(
                s => s.UpdateCompanyWarehouseNomenclature(
                    It.Is<int>(id => id == cwnId),
                    It.Is<CompanyWarehouseNomenclature>(c => c.Quantity == currentQuantity + quantity),
                    It.IsAny<CancellationToken>()),
                Times.Once);

            mockMovementNomenclatureService.Verify(
                s => s.DeleteMovementNomenclature(
                    movementNomenclatureId, It.IsAny<CancellationToken>()),
                     Times.Once);

            mockMovementRepository.Verify(
                r => r.DeleteMovement(existingMovement, It.IsAny<CancellationToken>()),
                Times.Once);
        }

        [Fact]
        public async Task DeleteMovement_WhenStatusMoving_Success()
        {
            int movementId = 1;
            int companyWarehouseFromId = 1;
            int companyWarehouseToId = 2;
            int nomenclatureId = 10;
            int movementNomenclatureId = 100;
            int cwnFromId = 1;
            int cwnToId = 2;
            int quantity = 5;
            int currentQuantityFrom = 15;
            int currentQuantityTo = 10;
            var movementNomenclatures = new List<MovementNomenclature>
            {
                new MovementNomenclature { Id = movementNomenclatureId, MovementId = movementId, NomenclatureId = nomenclatureId, Quantity = quantity }
            };
            var existingMovement = new Movement
            {
                Id = movementId,
                CompanyWarehouseFromId = companyWarehouseFromId,
                CompanyWarehouseToId = companyWarehouseToId,
                Status = MovementStatus.Moving,
                Date = DateTime.UtcNow
            };
            var companyWarehouseNomenclatureFrom = new CompanyWarehouseNomenclature { 
                Id = cwnFromId, 
                CompanyWarehouseId = companyWarehouseFromId, 
                NomenclatureId = nomenclatureId, 
                Quantity = currentQuantityFrom };
            var companyWarehouseNomenclatureTo = new CompanyWarehouseNomenclature { 
                Id = cwnToId, 
                CompanyWarehouseId = companyWarehouseToId, 
                NomenclatureId = nomenclatureId, 
                Quantity = currentQuantityTo };

            Mock<IMovementRepository> mockMovementRepo = new Mock<IMovementRepository>();
            mockMovementRepo
                .Setup(r => r.GetMovementById(movementId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(existingMovement);

            Mock<IMovementNomenclatureService> mockMovementNomenclatureService = new Mock<IMovementNomenclatureService>();
            mockMovementNomenclatureService
                .Setup(s => s.GetAllMovementNomenclatures(movementId, null, null, null, null, null, It.IsAny<CancellationToken>()))
                .ReturnsAsync(movementNomenclatures);

            Mock<ICompanyWareHouseNomenclatureService> mockCompanyWareHouseNomenclatureService = new Mock<ICompanyWareHouseNomenclatureService>();
            mockCompanyWarehouseNomenclatureService
                .Setup(s => s.GetCompanyWarehouseNomenclatureByCompanyWarehouseIdAndNomenclatureId(companyWarehouseFromId, nomenclatureId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(cwnFrom);
            mockCwn
                .Setup(s => s.GetCompanyWarehouseNomenclatureByCompanyWarehouseIdAndNomenclatureId(companyWarehouseToId, nomenclatureId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(cwnTo);

            MovementService sut = CreateSut(mockMovementRepo: mockMovementRepo, 
                mockCwn: mockCompanyWareHouseNomenclatureService, 
                mockMn: mockMovementNomenclatureService);

            await sut.DeleteMovement(movementId);

            mockCompanyWareHouseNomenclatureService.Verify(
                s => s.UpdateCompanyWarehouseNomenclature(
                    It.Is<int>(id => id == cwnFromId),
                    It.Is<CompanyWarehouseNomenclature>(c => c.Quantity == currentQuantityFrom + quantity),
                    It.IsAny<CancellationToken>()),
                Times.Once);

            mockCompanyWareHouseNomenclatureService.Verify(
                s => s.UpdateCompanyWarehouseNomenclature(
                    It.Is<int>(id => id == cwnToId),
                    It.Is<CompanyWarehouseNomenclature>(c => c.Quantity == currentQuantityTo - quantity),
                    It.IsAny<CancellationToken>()),
                Times.Once);

            mockMovementNomenclatureService.Verify(
                s => s.DeleteMovementNomenclature(
                    movementNomenclatureId, It.IsAny<CancellationToken>()),
                     Times.Once);

            mockMovementRepository.Verify(
                r => r.DeleteMovement(existingMovement, It.IsAny<CancellationToken>()),
                Times.Once);
        }
    }
}
