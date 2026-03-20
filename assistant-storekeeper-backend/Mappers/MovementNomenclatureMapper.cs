using AutoMapper;
using assistant_storekeeper_backend.Models;
using assistant_storekeeper_backend.DTOS.MovementNomenclatureDTOs;

namespace assistant_storekeeper_backend.Mappers
{
    public class MovementNomenclatureMapper : Profile
    {
        public MovementNomenclatureMapper()
        {
            CreateMap<MovementNomenclature, MovementNomenclatureDTO>()
                .ForMember(dest => dest.NomenclatureName, opt => opt.MapFrom(src => src.Nomenclature.Name));
        }
    }
}