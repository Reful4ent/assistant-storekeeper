using AutoMapper;
using assistant_storekeeper_backend.Models;
using assistant_storekeeper_backend.DTOS.CompanyWarehouseDTOs;

namespace assistant_storekeeper_backend.Mappers
{
    public class CompanyWarehouseMapper : Profile
    {
        public CompanyWarehouseMapper()
        {
            CreateMap<CompanyWarehouse, CompanyWarehouseDTO>();
            
        }
    }
}