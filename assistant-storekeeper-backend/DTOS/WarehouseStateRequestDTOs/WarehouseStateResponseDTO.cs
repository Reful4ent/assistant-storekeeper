using System.Collections.Generic;
using assistant_storekeeper_backend.DTOS.CompanyWarehouseNomenclatureCalculateDTOs;

namespace assistant_storekeeper_backend.DTOS.WarehouseStateRequestDTOs
{
    public class WarehouseStateResponseDTO
    {
        public List<CompanyWarehouseNomenclatureCalculateDTO> CompanyWarehouseNomenclatures { get; set; } = new List<CompanyWarehouseNomenclatureCalculateDTO>();
    }
}