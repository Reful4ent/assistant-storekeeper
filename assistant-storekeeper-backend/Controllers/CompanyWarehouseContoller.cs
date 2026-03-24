using System.Threading.Tasks;
using assistant_storekeeper_backend.Models;
using assistant_storekeeper_backend.Services.CompanyWarehouses;
using Microsoft.AspNetCore.Mvc;
using assistant_storekeeper_backend.DTOS.CompanyWarehouseDTOs;
using AutoMapper;
using assistant_storekeeper_backend.Mappers;
using System.Collections.Generic;
using assistant_storekeeper_backend.DTOS.PagedResultDTOs;

namespace assistant_storekeeper_backend.Controllers
{
    [ApiController]
    [Route("api/company-warehouses")]
    public class CompanyWarehouseController : ControllerBase
    {
        private readonly ICompanyWarehouseService _companyWarehouseService;
        private readonly IMapper _mapper;
        public CompanyWarehouseController(ICompanyWarehouseService companyWarehouseService, IMapper mapper)
        {
            _companyWarehouseService = companyWarehouseService;
            _mapper = mapper;
        }

        [HttpGet("{id:int}")]
        public async Task<IActionResult> GetCompanyWarehouseById(int id)
        {
            var companyWarehouse = await _companyWarehouseService.GetCompanyWarehouseById(id);
            return Ok(_mapper.Map<CompanyWarehouseDTO>(companyWarehouse));
        }

        [HttpGet]
        public async Task<IActionResult> GetAllCompanyWarehouses(
            [FromQuery] int? page, 
            [FromQuery] int? pageSize, 
            [FromQuery] string? search, 
            [FromQuery] bool? isAscending,
            [FromQuery] string? sortBy)
        {
            var companyWarehouses = await _companyWarehouseService.GetAllCompanyWarehouses(page, pageSize, search, isAscending, sortBy);
            return Ok(new PagedResultDTO<CompanyWarehouseDTO>
            {
                Data = _mapper.Map<List<CompanyWarehouseDTO>>(companyWarehouses.data),
                Total = companyWarehouses.total,
                TotalPages = companyWarehouses.totalPages,
            });
        }

        [HttpPost]
        public async Task<IActionResult> CreateCompanyWarehouse([FromBody] CompanyWarehouse companyWarehouse)
        {
            var createdCompanyWarehouse = await _companyWarehouseService.CreateCompanyWarehouse(companyWarehouse);
            return CreatedAtAction(nameof(GetCompanyWarehouseById), new { id = createdCompanyWarehouse.Id }, _mapper.Map<CompanyWarehouseDTO>(createdCompanyWarehouse));
        }

        [HttpPut("{id:int}")]
        public async Task<IActionResult> UpdateCompanyWarehouse(int id, [FromBody] CompanyWarehouse companyWarehouse)
        {
            var updatedCompanyWarehouse = await _companyWarehouseService.UpdateCompanyWarehouse(id, companyWarehouse);
            return Ok(_mapper.Map<CompanyWarehouseDTO>(updatedCompanyWarehouse));
        }

        [HttpDelete("{id:int}")]
        public async Task<IActionResult> DeleteCompanyWarehouse(int id)
        {
            await _companyWarehouseService.DeleteCompanyWarehouse(id);
            return NoContent();
        }
    }
}