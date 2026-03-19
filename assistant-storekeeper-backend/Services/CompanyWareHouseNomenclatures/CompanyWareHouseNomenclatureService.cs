using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using System.Linq;
using assistant_storekeeper_backend.Models;
using assistant_storekeeper_backend.Repositories.CompanyWareHouseNomenclatures;
using assistant_storekeeper_backend.Repositories.CompanyWarehouses;
using assistant_storekeeper_backend.Repositories.Nomenclatures;
using assistant_storekeeper_backend.Exceptions;

namespace assistant_storekeeper_backend.Services.CompanyWareHouseNomenclatures
{
    public class CompanyWareHouseNomenclatureService : ICompanyWareHouseNomenclatureService
    {
        private readonly ICompanyWareHouseNomenclatureRepository _companyWareHouseNomenclatureRepository;
        private readonly ICompanyWarehouseRepository _companyWarehouseRepository;
        private readonly INomenclatureRepository _nomenclatureRepository;

        public CompanyWareHouseNomenclatureService(
            ICompanyWareHouseNomenclatureRepository companyWareHouseNomenclatureRepository,
            ICompanyWarehouseRepository companyWarehouseRepository,
            INomenclatureRepository nomenclatureRepository)
        {
            _companyWareHouseNomenclatureRepository = companyWareHouseNomenclatureRepository;
            _companyWarehouseRepository = companyWarehouseRepository;
            _nomenclatureRepository = nomenclatureRepository;
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
            if (companyWarehouseId <= 0)
            {
                throw new BadRequestException("Company warehouse is required");
            }
            else
            {
                var companyWarehouse = await _companyWarehouseRepository.GetCompanyWarehouseById(companyWarehouseId, cancellationToken);
                if (companyWarehouse == null)
                {
                    throw new NotFoundException("Company warehouse not found");
                }
            }
            return await _companyWareHouseNomenclatureRepository.GetAllCompanyWarehouseNomenclatures(companyWarehouseId, page, pageSize, search, isAscending, sortBy, cancellationToken);
        }

        public async Task<CompanyWarehouseNomenclature> CreateCompanyWarehouseNomenclature(
            CompanyWarehouseNomenclature companyWarehouseNomenclature, 
            CancellationToken cancellationToken = default)
        {
            if (companyWarehouseNomenclature.CompanyWarehouseId <= 0)
            {
                throw new BadRequestException("Company warehouse is required");
            }
            else
            {
                var companyWarehouse = await _companyWarehouseRepository.GetCompanyWarehouseById(companyWarehouseNomenclature.CompanyWarehouseId, cancellationToken);
                if (companyWarehouse == null)
                {
                    throw new NotFoundException("Company warehouse not found");
                }
            }
            if (companyWarehouseNomenclature.NomenclatureId <= 0)
            {
                throw new BadRequestException("Nomenclature is required");
            }
            else
            {
                var nomenclature = await _nomenclatureRepository.GetNomenclatureById(companyWarehouseNomenclature.NomenclatureId, cancellationToken);
                if (nomenclature == null)
                {
                    throw new NotFoundException("Nomenclature not found");
                }
            }

            if (companyWarehouseNomenclature.Quantity <= 0)
            {
                throw new BadRequestException("Quantity is required");
            }

            return await _companyWareHouseNomenclatureRepository.CreateCompanyWarehouseNomenclature(companyWarehouseNomenclature, cancellationToken);
        }
        public async Task<CompanyWarehouseNomenclature> UpdateCompanyWarehouseNomenclature(int id, CompanyWarehouseNomenclature companyWarehouseNomenclature, CancellationToken cancellationToken = default)
        {
            var existingCompanyWarehouseNomenclature = await _companyWareHouseNomenclatureRepository.GetCompanyWarehouseNomenclatureById(id, cancellationToken);
            if (existingCompanyWarehouseNomenclature == null)
            {
                throw new NotFoundException("Company warehouse nomenclature not found");
            }

            if (companyWarehouseNomenclature.CompanyWarehouseId <= 0)
            {
                throw new BadRequestException("Company warehouse is required");
            }
            else
            {
                var companyWarehouse = await _companyWarehouseRepository.GetCompanyWarehouseById(companyWarehouseNomenclature.CompanyWarehouseId, cancellationToken);
                if (companyWarehouse == null)
                {
                    throw new NotFoundException("Company warehouse not found");
                }
            }
            if (companyWarehouseNomenclature.NomenclatureId <= 0)
            {
                throw new BadRequestException("Nomenclature is required");
            }
            else
            {
                var nomenclature = await _nomenclatureRepository.GetNomenclatureById(companyWarehouseNomenclature.NomenclatureId, cancellationToken);
                if (nomenclature == null)
                {
                    throw new NotFoundException("Nomenclature not found");
                }
            }

            if (companyWarehouseNomenclature.Quantity <= 0)
            {
                throw new BadRequestException("Quantity is required");
            }

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
    }
}