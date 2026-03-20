using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using System.Threading;
using assistant_storekeeper_backend.Services.MovementNomenclatures;
using assistant_storekeeper_backend.Models;
using AutoMapper;
using assistant_storekeeper_backend.DTOS.MovementNomenclatureDTOs;
using System.Collections.Generic;


namespace assistant_storekeeper_backend.Controllers
{
    [ApiController]
    [Route("api/movement-nomenclatures")]
    public class MovementNomenclatureController : ControllerBase
    {
        private readonly IMovementNomenclatureService _movementNomenclatureService;
        private readonly IMapper _mapper;
        public MovementNomenclatureController(IMovementNomenclatureService movementNomenclatureService, IMapper mapper)
        {
            _movementNomenclatureService = movementNomenclatureService;
            _mapper = mapper;
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
            var movementNomenclatures = await _movementNomenclatureService.GetAllMovementNomenclatures(movementId, page, pageSize, search, isAscending, sortBy, cancellationToken);
            return Ok(_mapper.Map<List<MovementNomenclatureDTO>>(movementNomenclatures));
        }

        /*
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
        }*/
    }
}