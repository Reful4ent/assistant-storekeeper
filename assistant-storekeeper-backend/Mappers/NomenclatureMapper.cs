using AutoMapper;
using assistant_storekeeper_backend.Models;
using assistant_storekeeper_backend.DTOS.NomenclatureDTOs;

namespace assistant_storekeeper_backend.Mappers
{
    public class NomenclatureMapper : Profile
    {
        public NomenclatureMapper()
        {
            CreateMap<Nomenclature, NomenclatureDTO>();
            CreateMap<NomenclatureDTO, Nomenclature>()
                .ForMember(dest => dest.MovementNomenclatures, opt => opt.Ignore())
                .ForMember(dest => dest.CompanyWarehouseNomenclatures, opt => opt.Ignore());;
        }
    }
}