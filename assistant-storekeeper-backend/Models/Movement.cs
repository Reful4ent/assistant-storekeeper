using System;
using assistant_storekeeper_backend.Models;
using System.Collections.Generic;

namespace assistant_storekeeper_backend.Models
{
    public class Movement
    {
        public int Id { get; set; }
        public int? CompanyWarehouseFromId { get; set; }
        public int? CompanyWarehouseToId { get; set; }
        public CompanyWarehouse? CompanyWarehouseFrom { get; set; }
        public CompanyWarehouse? CompanyWarehouseTo { get; set; }
        public DateTime Date { get; set; }
        public MovementStatus Status { get; set; }
        public List<MovementNomenclature> MovementNomenclatures { get; set; } = new List<MovementNomenclature>();
    }
}
