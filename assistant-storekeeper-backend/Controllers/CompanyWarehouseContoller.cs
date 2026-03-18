using System.Threading.Tasks;
using assistant_storekeeper_backend.Models;
using assistant_storekeeper_backend.Services.CompanyWarehouses;
using Microsoft.AspNetCore.Mvc;

namespace assistant_storekeeper_backend.Controllers
{
    [ApiController]
    [Route("api/company-warehouses")]
    public class CompanyWarehouseController : ControllerBase
    {
        private readonly ICompanyWarehouseService _companyWarehouseService;
        public CompanyWarehouseController(ICompanyWarehouseService companyWarehouseService)
        {
            _companyWarehouseService = companyWarehouseService;
        }

        [HttpGet("{id:int}")]
        public async Task<IActionResult> GetCompanyWarehouseById(int id)
        {
            var companyWarehouse = await _companyWarehouseService.GetCompanyWarehouseById(id);
            return Ok(companyWarehouse);
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
            return Ok(companyWarehouses);
        }

        [HttpPost]
        public async Task<IActionResult> CreateCompanyWarehouse([FromBody] CompanyWarehouse companyWarehouse)
        {
            var createdCompanyWarehouse = await _companyWarehouseService.CreateCompanyWarehouse(companyWarehouse);
            return CreatedAtAction(nameof(GetCompanyWarehouseById), new { id = createdCompanyWarehouse.Id }, createdCompanyWarehouse);
        }

        [HttpPut("{id:int}")]
        public async Task<IActionResult> UpdateCompanyWarehouse(int id, [FromBody] CompanyWarehouse companyWarehouse)
        {
            var updatedCompanyWarehouse = await _companyWarehouseService.UpdateCompanyWarehouse(id, companyWarehouse);
            return Ok(updatedCompanyWarehouse);
        }

        [HttpDelete("{id:int}")]
        public async Task<IActionResult> DeleteCompanyWarehouse(int id)
        {
            await _companyWarehouseService.DeleteCompanyWarehouse(id);
            return NoContent();
        }
    }
}