using assistant_storekeeper_backend.Services.CompanyWareHouseNomenclatures;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using System.Threading;
using assistant_storekeeper_backend.Models;

namespace assistant_storekeeper_backend.Controllers
{
    [ApiController]
    [Route("api/company-warehouse-nomenclatures")]
    public class CompanyWarehouseNomenclatureController : ControllerBase
    {
        private readonly ICompanyWareHouseNomenclatureService _companyWareHouseNomenclatureService;

        public CompanyWarehouseNomenclatureController(ICompanyWareHouseNomenclatureService companyWareHouseNomenclatureService)
        {
            _companyWareHouseNomenclatureService = companyWareHouseNomenclatureService;
        }

        [HttpGet]
        public async Task<IActionResult> GetAllCompanyWarehouseNomenclatures(
            [FromQuery] int companyWarehouseId = 0,
            [FromQuery] int? page = 1,
            [FromQuery] int? pageSize = 10,
            [FromQuery] string? search = null,
            [FromQuery] bool? isAscending = true,
            [FromQuery] string? sortBy = null,
            CancellationToken cancellationToken = default)
        {
            return Ok(await _companyWareHouseNomenclatureService.GetAllCompanyWarehouseNomenclatures(companyWarehouseId, page, pageSize, search, isAscending, sortBy, cancellationToken));
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> CreateCompanyWarehouseNomenclature(
            [FromBody] CompanyWarehouseNomenclature companyWarehouseNomenclature,
            CancellationToken cancellationToken = default)
        {
            return Ok(await _companyWareHouseNomenclatureService.CreateCompanyWarehouseNomenclature(companyWarehouseNomenclature, cancellationToken));
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteCompanyWarehouseNomenclature(int id, CancellationToken cancellationToken = default)
        {
            await _companyWareHouseNomenclatureService.DeleteCompanyWarehouseNomenclature(id, cancellationToken);
            return NoContent();
        }
    }
}