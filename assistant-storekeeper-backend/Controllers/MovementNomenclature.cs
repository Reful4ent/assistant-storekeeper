namespace assistant_storekeeper_backend.Controllers
{
    public class MovementNomenclatureController : ControllerBase
    {
        [ApiController]
        [Route("api/movement-nomenclatures")]
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
            CancellationToken cancellationToken = default)
        {
            return Ok(await _movementNomenclatureService.GetAllMovementNomenclatures(page, pageSize, search, isAscending, sortBy, cancellationToken));
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