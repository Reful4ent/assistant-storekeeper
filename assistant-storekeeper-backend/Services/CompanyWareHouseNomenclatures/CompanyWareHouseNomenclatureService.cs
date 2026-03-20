using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using System.Linq;
using assistant_storekeeper_backend.Models;
using assistant_storekeeper_backend.Repositories.CompanyWareHouseNomenclatures;
using assistant_storekeeper_backend.Exceptions;
using assistant_storekeeper_backend.Services.CompanyWarehouses;
using assistant_storekeeper_backend.Services.Nomenclatures;

namespace assistant_storekeeper_backend.Services.CompanyWareHouseNomenclatures
{
    public class CompanyWareHouseNomenclatureService : ICompanyWareHouseNomenclatureService
    {
        private readonly ICompanyWareHouseNomenclatureRepository _companyWareHouseNomenclatureRepository;
        private readonly ICompanyWarehouseService _companyWarehouseService;
        private readonly INomenclatureService _nomenclatureService;

        public CompanyWareHouseNomenclatureService(
            ICompanyWareHouseNomenclatureRepository companyWareHouseNomenclatureRepository,
            ICompanyWarehouseService companyWarehouseService,
            INomenclatureService nomenclatureService)
        {
            _companyWareHouseNomenclatureRepository = companyWareHouseNomenclatureRepository;
            _companyWarehouseService = companyWarehouseService;
            _nomenclatureService = nomenclatureService;
        }

        public async Task<CompanyWarehouseNomenclature?> GetCompanyWarehouseNomenclatureById(
            int id, 
            CancellationToken cancellationToken = default)
        {
            var companyWarehouseNomenclature = await _companyWareHouseNomenclatureRepository.GetCompanyWarehouseNomenclatureById(id, cancellationToken);
            if (companyWarehouseNomenclature == null)
            {
                throw new NotFoundException("Company warehouse nomenclature not found");
            }
            return companyWarehouseNomenclature;
        }

        public async Task<CompanyWarehouseNomenclature?> GetCompanyWarehouseNomenclatureByCompanyWarehouseIdAndNomenclatureId(
            int companyWarehouseId,
            int nomenclatureId,
            CancellationToken cancellationToken = default)
        {
            await ValidateCompanyWarehouseNomenclature(companyWarehouseId, cancellationToken);
            await ValidateNomenclature(nomenclatureId, cancellationToken);

            return await _companyWareHouseNomenclatureRepository.GetCompanyWarehouseNomenclatureByCompanyWarehouseIdAndNomenclatureId(
                companyWarehouseId, 
                nomenclatureId, 
                cancellationToken);
        }
        
        public async Task<IEnumerable<CompanyWarehouseNomenclature>> GetAllCompanyWarehouseNomenclatures(
            int companyWarehouseId, 
            int? page, 
            int? pageSize, 
            string? search, 
            bool? isAscending, 
            string? sortBy, 
            CancellationToken cancellationToken = default)
        {
            await ValidateCompanyWarehouseNomenclature(companyWarehouseId, cancellationToken);

            return await _companyWareHouseNomenclatureRepository.GetAllCompanyWarehouseNomenclatures(companyWarehouseId, page, pageSize, search, isAscending, sortBy, cancellationToken);
        }

        public async Task<CompanyWarehouseNomenclature> CreateCompanyWarehouseNomenclature(
            CompanyWarehouseNomenclature companyWarehouseNomenclature, 
            CancellationToken cancellationToken = default)
        {
            await ValidateCompanyWarehouseNomenclature(companyWarehouseNomenclature.CompanyWarehouseId, cancellationToken);
            await ValidateNomenclature(companyWarehouseNomenclature.NomenclatureId, cancellationToken);
            await ValidateQuantity(companyWarehouseNomenclature.Quantity, cancellationToken);

            return await _companyWareHouseNomenclatureRepository.CreateCompanyWarehouseNomenclature(companyWarehouseNomenclature, cancellationToken);
        }

        public async Task<CompanyWarehouseNomenclature> UpdateCompanyWarehouseNomenclature(int id, CompanyWarehouseNomenclature companyWarehouseNomenclature, CancellationToken cancellationToken = default)
        {
            var existingCompanyWarehouseNomenclature = await _companyWareHouseNomenclatureRepository.GetCompanyWarehouseNomenclatureById(id, cancellationToken);
            if (existingCompanyWarehouseNomenclature == null)
            {
                throw new NotFoundException("Company warehouse nomenclature not found");
            }

            await ValidateCompanyWarehouseNomenclature(companyWarehouseNomenclature.CompanyWarehouseId, cancellationToken);
            await ValidateNomenclature(companyWarehouseNomenclature.NomenclatureId, cancellationToken);
            await ValidateQuantity(companyWarehouseNomenclature.Quantity, cancellationToken);

            existingCompanyWarehouseNomenclature.CompanyWarehouseId = companyWarehouseNomenclature.CompanyWarehouseId;
            existingCompanyWarehouseNomenclature.NomenclatureId = companyWarehouseNomenclature.NomenclatureId;
            existingCompanyWarehouseNomenclature.Quantity = companyWarehouseNomenclature.Quantity;
            return await _companyWareHouseNomenclatureRepository.UpdateCompanyWarehouseNomenclature(existingCompanyWarehouseNomenclature, cancellationToken);
        }

        public async Task DeleteCompanyWarehouseNomenclature(int id, CancellationToken cancellationToken = default)
        {
            var companyWarehouseNomenclature = await _companyWareHouseNomenclatureRepository.GetCompanyWarehouseNomenclatureById(id, cancellationToken);
            if (companyWarehouseNomenclature == null)
            {
                throw new NotFoundException("Company warehouse nomenclature not found");
            }
            await _companyWareHouseNomenclatureRepository.DeleteCompanyWarehouseNomenclature(companyWarehouseNomenclature, cancellationToken);
        }
        
        private async Task ValidateCompanyWarehouseNomenclature(int Id, CancellationToken cancellationToken = default) 
        {
            if (Id <= 0)
            {
                throw new BadRequestException("Id is required");
            }
            else
            {
                await _companyWarehouseService.GetCompanyWarehouseById(Id, cancellationToken);
            }
        }

        private async Task ValidateNomenclature(int Id, CancellationToken cancellationToken = default) 
        {
            if (Id <= 0)
            {
                throw new BadRequestException("Id is required");
            }
            else
            {
                await _nomenclatureService.GetNomenclatureById(Id, cancellationToken);
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