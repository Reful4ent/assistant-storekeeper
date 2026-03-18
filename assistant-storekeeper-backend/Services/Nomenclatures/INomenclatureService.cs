using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using assistant_storekeeper_backend.Models;

namespace assistant_storekeeper_backend.Services.Nomenclatures
{
    public interface INomenclatureService
    {
        Task<Nomenclature> GetNomenclatureById(int id, CancellationToken cancellationToken = default);
        Task<IEnumerable<Nomenclature>> GetAllNomenclatures(
            int? page, 
            int? pageSize, 
            string? search, 
            bool? isAscending, 
            string? sortBy, 
            CancellationToken cancellationToken = default);
        Task<Nomenclature> CreateNomenclature(Nomenclature nomenclature, CancellationToken cancellationToken = default);
        Task<Nomenclature> UpdateNomenclature(int id, Nomenclature nomenclature, CancellationToken cancellationToken = default);
        Task DeleteNomenclature(int id, CancellationToken cancellationToken = default);
    }  
}