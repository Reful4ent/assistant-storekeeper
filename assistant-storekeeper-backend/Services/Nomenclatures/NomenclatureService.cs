using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using System.Linq;
using assistant_storekeeper_backend.Models;
using assistant_storekeeper_backend.Repositories.Nomenclatures;
using assistant_storekeeper_backend.Exceptions;

namespace assistant_storekeeper_backend.Services.Nomenclatures
{
    public class NomenclatureService : INomenclatureService
    {
        private readonly INomenclatureRepository _nomenclatureRepository;

        public NomenclatureService(INomenclatureRepository nomenclatureRepository)
        {
            _nomenclatureRepository = nomenclatureRepository;
        }

        public async Task<Nomenclature> GetNomenclatureById(int id, CancellationToken cancellationToken = default)
        {
            var nomenclature = await _nomenclatureRepository.GetNomenclatureById(id, cancellationToken);
            if (nomenclature == null)
            {
                throw new NotFoundException("Nomenclature not found");
            }
            return nomenclature;
        }

        public async Task<IEnumerable<Nomenclature>> GetAllNomenclatures(
            int? page, 
            int? pageSize, 
            string? search, 
            bool? isAscending, 
            string? sortBy, 
            CancellationToken cancellationToken = default)
        {
            return await _nomenclatureRepository.GetAllNomenclatures(page, pageSize, search, isAscending, sortBy, cancellationToken);
        }

        public async Task<Nomenclature> CreateNomenclature(Nomenclature nomenclature, CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrWhiteSpace(nomenclature.Name))
            {
                throw new BadRequestException("Name is required and cannot be empty");
            }

            nomenclature.Name = nomenclature.Name.Trim();

            return await _nomenclatureRepository.CreateNomenclature(nomenclature, cancellationToken);
        }

        public async Task<Nomenclature> UpdateNomenclature(int id, Nomenclature nomenclature, CancellationToken cancellationToken = default)
        {
            var existingNomenclature = await _nomenclatureRepository.GetNomenclatureById(id, cancellationToken);
            if (existingNomenclature == null)
            {
                throw new NotFoundException("Nomenclature not found");
            }
            
            if (string.IsNullOrWhiteSpace(nomenclature.Name))
            {
                throw new BadRequestException("Name is required and cannot be empty");
            }

            existingNomenclature.Name = nomenclature.Name.Trim();

            return await _nomenclatureRepository.UpdateNomenclature(existingNomenclature, cancellationToken);
        }

        public async Task DeleteNomenclature(int id, CancellationToken cancellationToken = default)
        {
            var existingNomenclature = await _nomenclatureRepository.GetNomenclatureById(id, cancellationToken);
            if (existingNomenclature == null)
            {
                throw new NotFoundException("Nomenclature not found");
            }

            await _nomenclatureRepository.DeleteNomenclature(existingNomenclature, cancellationToken);
        }
    }
}