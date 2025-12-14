using Microsoft.AspNetCore.Mvc;
using KinoLib.Api.Models;
using KinoLib.Api.Services;

namespace KinoLib.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class MovieController : ControllerBase
    {
        private readonly IMovieService _movieService;

        public MovieController(IMovieService movieService)
        {
            _movieService = movieService;
        }

        [HttpGet]
        public IActionResult GetAllMovies()
        {
            var movies = _movieService.GetAllMovies();
            return Ok(movies);
        }

        [HttpGet("{id}")]
        public IActionResult GetMovie(int id)
        {
            var movie = _movieService.GetMovieById(id);
            if (movie == null) return NotFound();
            return Ok(movie);
        }

        [HttpPut]
        public IActionResult CreateMovie(MovieCreateDto movieDto)
        {
            var movie = _movieService.AddMovie(movieDto);
            return CreatedAtAction(nameof(GetMovie), new { id = movie.Id }, movie);
        }

        [HttpPut("{id}")]
        public IActionResult UpdateMovie(int id, MovieCreateDto movieDto)
        {
            var updatedMovie = _movieService.UpdateMovie(id, movieDto);
            if (updatedMovie == null) return NotFound();
            return Ok(updatedMovie);
        }

        [HttpDelete("{id}")]
        public IActionResult DeleteMovie(int id)
        {
            var result = _movieService.DeleteMovie(id);
            if (!result) return NotFound();
            return NoContent();
        }

        [HttpDelete]
        public IActionResult DeleteAllMovies()
        {
            _movieService.DeleteAllMovies();
            return NoContent();
        }
    }
}
