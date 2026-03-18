namespace assistant_storekeeper_backend.Models
{
    public class MovementNomenclature
    {
        public int Id { get; set; }
        public int MovementId { get; set; }
        public int NomenclatureId { get; set; }
        public int Quantity { get; set; }
    }
}