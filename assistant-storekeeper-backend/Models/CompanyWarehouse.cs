using System.Collections.Generic;
using assistant_storekeeper_backend.Models;

namespace assistant_storekeeper_backend.Models
{
    public class CompanyWarehouse
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public List<CompanyWarehouseNomenclature> CompanyWarehouseNomenclatures { get; set; } = new List<CompanyWarehouseNomenclature>();
    }
}