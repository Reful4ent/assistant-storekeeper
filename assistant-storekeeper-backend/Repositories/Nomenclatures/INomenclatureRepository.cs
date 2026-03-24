using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using assistant_storekeeper_backend.Models;

namespace assistant_storekeeper_backend.Repositories.Nomenclatures
{
    public interface INomenclatureRepository
    {
        Task<Nomenclature?> GetNomenclatureById(int id, CancellationToken cancellationToken = default);
        Task<(IEnumerable<Nomenclature> data, int total, int totalPages)> GetAllNomenclatures(
            int? page = 1,
            int? pageSize = 10,
            string? search = null,
            bool? isAscending = true,
            string? sortBy = "Id",
            CancellationToken cancellationToken = default);
        Task<Nomenclature> CreateNomenclature(Nomenclature nomenclature, CancellationToken cancellationToken = default);
        Task<Nomenclature> UpdateNomenclature(Nomenclature nomenclature, CancellationToken cancellationToken = default);
        Task DeleteNomenclature(Nomenclature nomenclature, CancellationToken cancellationToken = default);
    }
}