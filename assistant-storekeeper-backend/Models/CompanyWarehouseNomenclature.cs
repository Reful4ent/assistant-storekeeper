namespace assistant_storekeeper_backend.Models
{
    public class CompanyWarehouseNomenclature
    {
        public int Id { get; set; }
        public int CompanyWarehouseId { get; set; }
        public int NomenclatureId { get; set; }
        public int Quantity { get; set; }
        public CompanyWarehouse? CompanyWarehouse { get; set; }
        public Nomenclature? Nomenclature { get; set; }
    }
}