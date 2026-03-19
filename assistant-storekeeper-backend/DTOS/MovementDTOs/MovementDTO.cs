using System.Collections.Generic;
using assistant_storekeeper_backend.Models;
using assistant_storekeeper_backend.DTOS.MovementDTOs;

namespace assistant_storekeeper_backend.DTOS.MovementDTOs
{
    public class MovementDTO
    {
        public int CompanyWarehouseFromId { get; set; }
        public int CompanyWarehouseToId { get; set; }
        public MovementStatus Status { get; set; }
        public List<NomenclatureInMovementDTO> Nomenclatures { get; set; } = new List<NomenclatureInMovementDTO>();
    }
}