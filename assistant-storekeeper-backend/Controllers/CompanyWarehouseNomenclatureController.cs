using assistant_storekeeper_backend.Services.CompanyWareHouseNomenclatures;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using System.Threading;
using assistant_storekeeper_backend.Models;
using AutoMapper;
using assistant_storekeeper_backend.DTOS.CompanyWarehouseNomenclatureDTOs;
using System.Collections.Generic;

namespace assistant_storekeeper_backend.Controllers
{
    [ApiController]
    [Route("api/company-warehouse-nomenclatures")]
    public class CompanyWarehouseNomenclatureController : ControllerBase
    {
        private readonly ICompanyWareHouseNomenclatureService _companyWareHouseNomenclatureService;
        private readonly IMapper _mapper;

        public CompanyWarehouseNomenclatureController(ICompanyWareHouseNomenclatureService companyWareHouseNomenclatureService, IMapper mapper)
        {
            _companyWareHouseNomenclatureService = companyWareHouseNomenclatureService;
            _mapper = mapper;
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
            var companyWarehouseNomenclatures = await _companyWareHouseNomenclatureService.GetAllCompanyWarehouseNomenclatures(companyWarehouseId, page, pageSize, search, isAscending, sortBy, cancellationToken);
            return Ok(_mapper.Map<List<CompanyWarehouseNomenclatureDTO>>(companyWarehouseNomenclatures));
        }

        [HttpPut("{id:int}")]
        public async Task<IActionResult> UpdateCompanyWarehouseNomenclature(
            int id,
            [FromBody] CompanyWarehouseNomenclature companyWarehouseNomenclature,
            CancellationToken cancellationToken = default)
        {
            var updatedCompanyWarehouseNomenclature = await _companyWareHouseNomenclatureService.UpdateCompanyWarehouseNomenclature(id, companyWarehouseNomenclature, cancellationToken);
            return Ok(_mapper.Map<CompanyWarehouseNomenclatureDTO>(updatedCompanyWarehouseNomenclature));
        }

        [HttpDelete("{id:int}")]
        public async Task<IActionResult> DeleteCompanyWarehouseNomenclature(int id, CancellationToken cancellationToken = default)
        {
            await _companyWareHouseNomenclatureService.DeleteCompanyWarehouseNomenclature(id, cancellationToken);
            return NoContent();
        }
    }
}