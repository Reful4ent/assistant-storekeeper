using AutoMapper;
using assistant_storekeeper_backend.Models;
using assistant_storekeeper_backend.DTOS.MovementDTOs;

namespace assistant_storekeeper_backend.Mappers
{
    public class MovementMapper : Profile
    {
        public MovementMapper()
        {
            CreateMap<Movement, MovementResponseDTO>()
                .ForMember(dest => dest.CompanyWarehouseFromName, opt => opt.MapFrom(src => src.CompanyWarehouseFrom.Name))
                .ForMember(dest => dest.CompanyWarehouseToName, opt => opt.MapFrom(src => src.CompanyWarehouseTo.Name))
                .ForMember(dest => dest.Nomenclatures, opt => opt.MapFrom(src => src.MovementNomenclatures));
        }
    }
}