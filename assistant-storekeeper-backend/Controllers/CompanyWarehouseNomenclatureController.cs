using assistant_storekeeper_backend.Services.CompanyWareHouseNomenclatures;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using System.Threading;
using assistant_storekeeper_backend.Models;
using assistant_storekeeper_backend.DTOS.CompanyWarehouseNomenclatureDTOs;
using AutoMapper;
using assistant_storekeeper_backend.Mappers;
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
            return Ok(_mapper.Map<IEnumerable<CompanyWarehouseNomenclatureDTO>>(await _companyWareHouseNomenclatureService.GetAllCompanyWarehouseNomenclatures(companyWarehouseId, page, pageSize, search, isAscending, sortBy, cancellationToken)));
        }
    }
}