using Microsoft.AspNetCore.Mvc;
using assistant_storekeeper_backend.Services.Movements;
using System.Threading.Tasks;
using assistant_storekeeper_backend.Models;

namespace assistant_storekeeper_backend.Controllers
{
    [ApiController]
    [Route("api/movements")]
    public class MovementController : ControllerBase
    {
        private readonly IMovementService _movementService;
        public MovementController(IMovementService movementService)
        {
            _movementService = movementService;
        }

        [HttpGet("{id:int}")]
        public async Task<IActionResult> GetMovementById(int id)
        {
            var movement = await _movementService.GetMovementById(id);
            return Ok(movement);
        }

        [HttpGet]
        public async Task<IActionResult> GetAllMovements(
            [FromQuery] int? page, 
            [FromQuery] int? pageSize,
            [FromQuery] string? search,
            [FromQuery] bool? isAscending,
            [FromQuery] string? sortBy)
        {
            var movements = await _movementService.GetAllMovements(page, pageSize, search, isAscending, sortBy);
            return Ok(movements);
        }

        [HttpPost]
        public async Task<IActionResult> CreateMovement([FromBody] Movement movement)
        {
            var newMovement = await _movementService.CreateMovement(movement);
            return CreatedAtAction(nameof(GetMovementById), new { id = newMovement.Id }, newMovement);
        }
    }
}