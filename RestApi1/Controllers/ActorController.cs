using Microsoft.AspNetCore.Mvc;
using KinoLib.Api.Models;
using KinoLib.Api.Services;

namespace KinoLib.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ActorController : ControllerBase
    {
        private readonly IActorService _actorService;

        public ActorController(IActorService actorService)
        {
            _actorService = actorService;
        }

        [HttpGet]
        public IActionResult GetAllActors()
        {
            var actors = _actorService.GetAllActors();
            return Ok(actors);
        }

        [HttpGet("{id}")]
        public IActionResult GetActor(int id)
        {
            var actor = _actorService.GetActorById(id);
            if (actor == null) return NotFound();
            return Ok(actor);
        }

        [HttpPost]
        public IActionResult CreateActor([FromBody] ActorCreateDto actorDto)
        {
            try
            {
                var actor = _actorService.AddActor(actorDto);
                return CreatedAtAction(nameof(GetActor), new { id = actor.Id }, actor);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPut("{id}")]
        public IActionResult UpdateActor(int id, [FromBody] ActorUpdateDto actorDto)
        {
            var updatedActor = _actorService.UpdateActor(id, actorDto);
            if (updatedActor == null) return NotFound();
            return Ok(updatedActor);
        }

        [HttpDelete("{id}")]
        public IActionResult DeleteActor(int id)
        {
            var result = _actorService.DeleteActor(id);
            if (!result) return NotFound();
            return NoContent();
        }

        [HttpDelete]
        public IActionResult DeleteAllActors()
        {
            _actorService.DeleteAllActors();
            return NoContent();
        }

        [HttpGet("{id}/movies")]
        public IActionResult GetActorMovies(int id)
        {
            var movies = _actorService.GetMoviesByActor(id);
            return Ok(movies);
        }
    }
}