using System;

namespace assistant_storekeeper_backend.Models
{
    public class Movement
    {
        public int Id { get; set; }
        public int? CompanyWarehouseFromId { get; set; }
        public int? CompanyWarehouseToId { get; set; }
        public DateTime Date { get; set; }
        public MovementStatus Status { get; set; }
    }
}
