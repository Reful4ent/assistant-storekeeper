using System;

namespace assistant_storekeeper_backend.DTOS.WarehouseStateRequestDTOs
{
    public class WarehouseStateRequestDTO
    {
        public int CompanyWarehouseId { get; set; }
        public DateTime? RequestDate { get; set; }
    }
}