using Microsoft.AspNetCore.Mvc;
using assistant_storekeeper_backend.Services.Movements;
using System.Threading.Tasks;
using assistant_storekeeper_backend.Models;
using assistant_storekeeper_backend.DTOS.MovementDTOs;
using AutoMapper;
using System.Collections.Generic;
using assistant_storekeeper_backend.DTOS.WarehouseStateRequestDTOs;
using System.Threading;
using assistant_storekeeper_backend.DTOS.PagedResultDTOs;

namespace assistant_storekeeper_backend.Controllers
{
    [ApiController]
    [Route("api/movements")]
    public class MovementController : ControllerBase
    {
        private readonly IMovementService _movementService;
        private readonly IMapper _mapper;
        public MovementController(IMovementService movementService, IMapper mapper)
        {   
            _movementService = movementService;
            _mapper = mapper;
        }

        [HttpGet("{id:int}")]
        public async Task<IActionResult> GetMovementById(int id)
        {
            var movement = await _movementService.GetMovementById(id);
            return Ok(_mapper.Map<MovementResponseDTO>(movement));
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
            return Ok(new PagedResultDTO<MovementResponseDTO>
            {
                Data = _mapper.Map<List<MovementResponseDTO>>(movements.data),
                Total = movements.total,
                TotalPages = movements.totalPages,
            });
        }

        [HttpPost]
        public async Task<IActionResult> CreateMovement([FromBody] MovementDTO movementDTO)
        {
            var newMovement = await _movementService.CreateMovement(movementDTO);
            return CreatedAtAction(nameof(GetMovementById), new { id = newMovement.Id }, _mapper.Map<MovementResponseDTO>(newMovement));
        }

        [HttpDelete("{id:int}")]
        public async Task<IActionResult> DeleteMovement(int id)
        {
            await _movementService.DeleteMovement(id);
            return NoContent();
        }

        [HttpPost("warehouse-state")]
        public async Task<IActionResult> GetWarehouseState(
            [FromBody] WarehouseStateRequestDTO warehouseStateRequestDTO)
        {
            var warehouseState = await _movementService.GetMovementsByDate(warehouseStateRequestDTO);
            return Ok(warehouseState);
        }
    }
}