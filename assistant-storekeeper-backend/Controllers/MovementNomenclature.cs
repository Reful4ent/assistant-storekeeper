using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using System.Threading;
using assistant_storekeeper_backend.Services.MovementNomenclatures;
using assistant_storekeeper_backend.Models;

namespace assistant_storekeeper_backend.Controllers
{
    [ApiController]
    [Route("api/movement-nomenclatures")]
    public class MovementNomenclatureController : ControllerBase
    {
        private readonly IMovementNomenclatureService _movementNomenclatureService;
        public MovementNomenclatureController(IMovementNomenclatureService movementNomenclatureService)
        {
            _movementNomenclatureService = movementNomenclatureService;
        }

        [HttpGet]
        public async Task<IActionResult> GetAllMovementNomenclatures(
            [FromQuery] int? page = 1,
            [FromQuery] int? pageSize = 10,
            [FromQuery] string? search = null,
            [FromQuery] bool? isAscending = true,
            [FromQuery] string? sortBy = null,
            [FromQuery] int movementId = 0,
            CancellationToken cancellationToken = default)
        {
            return Ok(await _movementNomenclatureService.GetAllMovementNomenclatures(movementId, page, pageSize, search, isAscending, sortBy, cancellationToken));
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateMovementNomenclature(int id,[FromBody] MovementNomenclature movementNomenclature, CancellationToken cancellationToken = default)
        {
            return Ok(await _movementNomenclatureService.UpdateMovementNomenclature(id, movementNomenclature, cancellationToken));
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteMovementNomenclature(int id, CancellationToken cancellationToken = default)
        {
            await _movementNomenclatureService.DeleteMovementNomenclature(id, cancellationToken);
            return NoContent();
        }
    }
}