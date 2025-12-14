using Microsoft.AspNetCore.Mvc;
using KinoLib.Api.Models;
using KinoLib.Api.Services;

namespace KinoLib.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class DirectorController : ControllerBase
    {
        private readonly IDirectorService _directorService;

        public DirectorController(IDirectorService directorService)
        {
            _directorService = directorService;
        }

        [HttpGet]
        public IActionResult GetAllDirectors()
        {
            var directors = _directorService.GetAllDirectors();
            return Ok(directors);
        }

        [HttpGet("{id}")]
        public IActionResult GetDirector(int id)
        {
            var director = _directorService.GetDirectorById(id);
            if (director == null) return NotFound();
            return Ok(director);
        }

        [HttpPut]
        public IActionResult CreateDirector(DirectorCreateDto directorDto)
        {
            var director = _directorService.AddDirector(directorDto);
            return CreatedAtAction(nameof(GetDirector), new { id = director.Id }, director);
        }

        [HttpPut("{id}")]
        public IActionResult UpdateDirector(int id, DirectorCreateDto directorDto)
        {
            var updatedDirector = _directorService.UpdateDirector(id, directorDto);
            if (updatedDirector == null) return NotFound();
            return Ok(updatedDirector);
        }

        [HttpDelete("{id}")]
        public IActionResult DeleteDirector(int id)
        {
            var result = _directorService.DeleteDirector(id);
            if (!result) return NotFound();
            return NoContent();
        }

        [HttpDelete]
        public IActionResult DeleteAllDirectors()
        {
            _directorService.DeleteAllDirectors();
            return NoContent();
        }
    }
}
