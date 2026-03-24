using System;
using System.Collections.Generic;
using assistant_storekeeper_backend.Models;
using assistant_storekeeper_backend.DTOS.MovementNomenclatureDTOs;

namespace assistant_storekeeper_backend.DTOS.MovementDTOs
{
    public class MovementResponseDTO
    {
        public int? Id { get; set; }
        public int? CompanyWarehouseFromId { get; set; }
        public int? CompanyWarehouseToId { get; set; }
        public string? CompanyWarehouseFromName { get; set; }
        public string? CompanyWarehouseToName { get; set; }
        public DateTime Date { get; set; }
        public MovementStatus Status { get; set; }
        public List<MovementNomenclatureDTO> Nomenclatures { get; set; } = new List<MovementNomenclatureDTO>();
    }
}