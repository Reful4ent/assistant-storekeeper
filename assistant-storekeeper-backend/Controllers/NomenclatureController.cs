using System.Threading.Tasks;
using assistant_storekeeper_backend.Models;
using assistant_storekeeper_backend.Services.Nomenclatures;
using Microsoft.AspNetCore.Mvc;

namespace assistant_storekeeper_backend.Controllers
{
    [ApiController]
    [Route("api/nomenclatures")]
    public class NomenclatureController : ControllerBase
    {
        private readonly INomenclatureService _nomenclatureService;
        public NomenclatureController(INomenclatureService nomenclatureService)
        {
            _nomenclatureService = nomenclatureService;
        }

        [HttpGet("{id:int}")]
        public async Task<IActionResult> GetNomenclatureById(int id)
        {
            var nomenclature = await _nomenclatureService.GetNomenclatureById(id);
            return Ok(nomenclature);
        }

        [HttpGet]
        public async Task<IActionResult> GetAllNomenclatures(
            [FromQuery] int? page, 
            [FromQuery] int? pageSize, 
            [FromQuery] string? search, 
            [FromQuery] bool? isAscending,
            [FromQuery] string? sortBy)
        {
            var nomenclatures = await _nomenclatureService.GetAllNomenclatures(page, pageSize, search, isAscending, sortBy);
            return Ok(nomenclatures);
        }

        [HttpPost]
        public async Task<IActionResult> CreateNomenclature([FromBody] Nomenclature nomenclature)
        {
            var createdNomenclature = await _nomenclatureService.CreateNomenclature(nomenclature);
            return CreatedAtAction(nameof(GetNomenclatureById), new { id = createdNomenclature.Id }, createdNomenclature);
        }

        [HttpPut("{id:int}")]
        public async Task<IActionResult> UpdateNomenclature(int id, [FromBody] Nomenclature nomenclature)
        {
            var updatedNomenclature = await _nomenclatureService.UpdateNomenclature(id, nomenclature);
            return Ok(updatedNomenclature);
        }

        [HttpDelete("{id:int}")]
        public async Task<IActionResult> DeleteNomenclature(int id)
        {
            await _nomenclatureService.DeleteNomenclature(id);
            return NoContent();
        }
    }
}