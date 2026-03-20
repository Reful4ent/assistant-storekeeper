using System.Collections.Generic;
using assistant_storekeeper_backend.DTOS.CompanyWarehouseNomenclatureDTOs;

namespace assistant_storekeeper_backend.DTOS.CompanyWarehouseDTOs
{
    public class CompanyWarehouseDTO
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public List<CompanyWarehouseNomenclatureDTO> CompanyWarehouseNomenclatures { get; set; } = new List<CompanyWarehouseNomenclatureDTO>();
    }
}