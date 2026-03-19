namespace assistant_storekeeper_backend.DTOS.MovementDTOs
{
    public class MovementNomenclatureDTO
    {
        public int Id { get; set; }
        public int NomenclatureId { get; set; }
        public string NomenclatureName { get; set; }
        public int Quantity { get; set; }
    }
}