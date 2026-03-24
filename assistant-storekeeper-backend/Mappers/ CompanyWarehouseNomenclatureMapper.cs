using AutoMapper;
using assistant_storekeeper_backend.Models;
using assistant_storekeeper_backend.DTOS.CompanyWarehouseNomenclatureDTOs;

namespace assistant_storekeeper_backend.Mappers
{
    public class CompanyWarehouseNomenclatureMapper : Profile
    {
        public CompanyWarehouseNomenclatureMapper()
        {
            CreateMap<CompanyWarehouseNomenclature, CompanyWarehouseNomenclatureDTO>()
                .ForMember(dest => dest.NomenclatureName, opt => opt.MapFrom(src => src.Nomenclature.Name));
        }
    }
}