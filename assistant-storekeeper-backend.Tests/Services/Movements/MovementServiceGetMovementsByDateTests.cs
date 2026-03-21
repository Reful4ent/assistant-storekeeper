using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using assistant_storekeeper_backend.DTOS.CompanyWarehouseNomenclatureCalculateDTOs;
using assistant_storekeeper_backend.DTOS.WarehouseStateRequestDTOs;
using assistant_storekeeper_backend.Exceptions;
using assistant_storekeeper_backend.Models;
using assistant_storekeeper_backend.Repositories.Movements;
using assistant_storekeeper_backend.Services.CompanyWarehouses;
using assistant_storekeeper_backend.Services.CompanyWareHouseNomenclatures;
using assistant_storekeeper_backend.Services.MovementNomenclatures;
using assistant_storekeeper_backend.Services.Movements;
using Moq;
using Xunit;

namespace assistant_storekeeper_backend.Tests.Services.Movements
{
    public class MovementServiceGetMovementsByDateTests
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
        public async Task GetMovementsByDate_WhenCompanyWarehouseNotFound_ThrowsNotFoundException()
        {
            int companyWarehouseId = 1;
            DateTime requestDate = DateTime.UtcNow;

            Mock<ICompanyWarehouseService> mockCompanyWarehouse = new Mock<ICompanyWarehouseService>();
            mockCompanyWarehouse
                .Setup(s => s.GetCompanyWarehouseById(companyWarehouseId, It.IsAny<CancellationToken>()))
                .ThrowsAsync(new NotFoundException("Company warehouse not found"));

            MovementService sut = CreateSut(mockCompanyWarehouse: mockCompanyWarehouse);

            Func<Task> act = () => sut.GetMovementsByDate(
                new WarehouseStateRequestDTO { 
                    CompanyWarehouseId = companyWarehouseId, 
                    RequestDate = requestDate });

            var ex = await Assert.ThrowsAsync<NotFoundException>(act);
            Assert.Equal("Company warehouse not found", ex.Message);
        }

        [Fact]
        public async Task GetMovementsByDate_WhenRequestDateIsNotSpecified_ThrowsBadRequestException()
        {
            int companyWarehouseId = 1;

            Mock<ICompanyWarehouseService> mockCompanyWarehouse = new Mock<ICompanyWarehouseService>();
            mockCompanyWarehouse
                .Setup(s => s.GetCompanyWarehouseById(companyWarehouseId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(new CompanyWarehouse { Id = companyWarehouseId, Name = "Warehouse" });
                
            MovementService sut = CreateSut(mockCompanyWarehouse: mockCompanyWarehouse);

            Func<Task> act = () => sut.GetMovementsByDate(
                new WarehouseStateRequestDTO { 
                    CompanyWarehouseId = companyWarehouseId, 
                    RequestDate = null 
                });

            var ex = await Assert.ThrowsAsync<BadRequestException>(act);
            Assert.Equal("Request date is required", ex.Message);
        }


        private static List<Movement> CreateTestMovements()
        {
            return new List<Movement>
            {
                new Movement
                {
                    Id = 1,
                    CompanyWarehouseFromId = null,
                    CompanyWarehouseToId = 1,
                    Status = MovementStatus.Coming,
                    Date = new DateTime(2026, 3, 20),
                    MovementNomenclatures = new List<MovementNomenclature>
                    {
                        new MovementNomenclature
                        {
                            Id = 1,
                            NomenclatureId = 1,
                            Quantity = 10,
                            Nomenclature = new Nomenclature { 
                                Id = 1, 
                                Name = "Product A"
                            }
                        }
                    }
                },
                new Movement
                {
                    Id = 2,
                    CompanyWarehouseFromId = null,
                    CompanyWarehouseToId = 1,
                    Status = MovementStatus.Coming,
                    Date = new DateTime(2026, 3, 20),
                    MovementNomenclatures = new List<MovementNomenclature>
                    {
                        new MovementNomenclature
                        {
                            Id = 2,
                            NomenclatureId = 2,
                            Quantity = 20,
                            Nomenclature = new Nomenclature { 
                                Id = 2, 
                                Name = "Product B"
                            }
                        }
                    }
                },
                new Movement
                {
                    Id = 3,
                    CompanyWarehouseFromId = 1,
                    CompanyWarehouseToId = 2,
                    Status = MovementStatus.Moving,
                    Date = new DateTime(2026, 3, 20),
                    MovementNomenclatures = new List<MovementNomenclature>
                    {
                        new MovementNomenclature
                        {
                            Id = 3,
                            NomenclatureId = 2,
                            Quantity = 5,
                            Nomenclature = new Nomenclature { 
                                Id = 2, 
                                Name = "Product B"
                            }
                        }
                    }
                },
                new Movement
                {
                    Id = 4,
                    CompanyWarehouseFromId = 1,
                    CompanyWarehouseToId = 2,
                    Status = MovementStatus.Moving,
                    Date = new DateTime(2026, 3, 21),
                    MovementNomenclatures = new List<MovementNomenclature>
                    {
                        new MovementNomenclature
                        {
                            Id = 4,
                            NomenclatureId = 1,
                            Quantity = 9,
                            Nomenclature = new Nomenclature { 
                                Id = 1, 
                                Name = "Product A" 
                            }
                        }
                    }
                },
                new Movement
                {
                    Id = 5,
                    CompanyWarehouseFromId = 1,
                    CompanyWarehouseToId = null,
                    Status = MovementStatus.Consumption,
                    Date = new DateTime(2026, 3, 22),
                    MovementNomenclatures = new List<MovementNomenclature>
                    {
                        new MovementNomenclature
                        {
                            Id = 5,
                            NomenclatureId = 1,
                            Quantity = 1,
                            Nomenclature = new Nomenclature { 
                                Id = 1, 
                                Name = "Product A" 
                            }
                        }
                    }
                }
            };
        }

        public static IEnumerable<object[]> GetMovementsByDateTestData()
        {
            return new List<object[]>
            {
                new object[] { new DateTime(2026, 3, 20), 10, 15 },
                new object[] { new DateTime(2026, 3, 21), 1, 15 },
                new object[] { new DateTime(2026, 3, 22), 0, 15 },
            };
        }

        [Theory]
        [MemberData(nameof(GetMovementsByDateTestData))]
        public async Task GetMovementsByDate_WhenMovementsExist_ReturnsCorrectStockForDate(
            DateTime requestDate,
            int expectedNom1Quantity,
            int expectedNom2Quantity)
        {
            int companyWarehouseId = 1;
            List<Movement> movements = CreateTestMovements();

            Mock<ICompanyWarehouseService> mockCompanyWarehouse = new Mock<ICompanyWarehouseService>();
            mockCompanyWarehouse
                .Setup(s => s.GetCompanyWarehouseById(companyWarehouseId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(new CompanyWarehouse { Id = companyWarehouseId, Name = "Warehouse" });

            Mock<IMovementRepository> mockMovementRepository = new Mock<IMovementRepository>();
            var movementsForDate = movements.Where(m => m.Date.Date <= requestDate.Date).ToList();
            mockMovementRepository
                .Setup(s => s.GetMovementsByDate(companyWarehouseId, requestDate, It.IsAny<CancellationToken>()))
                .ReturnsAsync(movementsForDate);

            MovementService sut = CreateSut(
                mockMovementRepo: mockMovementRepository,
                mockCompanyWarehouse: mockCompanyWarehouse);

            var result = await sut.GetMovementsByDate(new WarehouseStateRequestDTO
            {
                CompanyWarehouseId = companyWarehouseId,
                RequestDate = requestDate
            });

            Assert.NotNull(result);
            Assert.Equal(2, result.CompanyWarehouseNomenclatures.Count);

            var nom1 = result.CompanyWarehouseNomenclatures.FirstOrDefault(n => n.NomenclatureId == 1);
            Assert.NotNull(nom1);
            Assert.Equal(expectedNom1Quantity, nom1.Quantity);
            Assert.Equal("Product A", nom1.NomenclatureName);

            var nom2 = result.CompanyWarehouseNomenclatures.FirstOrDefault(n => n.NomenclatureId == 2);
            Assert.NotNull(nom2);
            Assert.Equal(expectedNom2Quantity, nom2.Quantity);
            Assert.Equal("Product B", nom2.NomenclatureName);
        }
    }
}
