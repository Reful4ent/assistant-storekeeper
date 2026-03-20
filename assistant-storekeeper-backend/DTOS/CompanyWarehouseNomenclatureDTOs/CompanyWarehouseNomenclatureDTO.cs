namespace assistant_storekeeper_backend.DTOS.CompanyWarehouseNomenclatureDTOs
{
    public class CompanyWarehouseNomenclatureDTO
    {
        public int Id { get; set; }
        public int NomenclatureId { get; set; }
        public string NomenclatureName { get; set; }
        public int Quantity { get; set; }
    }
}