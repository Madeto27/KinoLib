using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using KinoLib.Api.Data;
using KinoLib.Api.Models;
using Microsoft.Extensions.Logging;

namespace KinoLib.Api.Repositories
{
    public class MovieRepository : IMovieRepository
    {
        private readonly AppDbContext _context;
        private readonly ILogger<MovieRepository> _logger;

        public MovieRepository(AppDbContext context, ILogger<MovieRepository> logger)
        {
            _context = context;
            _logger = logger;
        }

        public List<Movie> GetAll()
        {
            _logger.LogInformation("REPO: Fetching all movies, including their directors.");
            return _context.Movies
                .Include(m => m.Director)
                .ToList();
        }

        public Movie? GetById(int id)
        {
            _logger.LogInformation("REPO: Fetching movie by ID: {MovieId}", id);
            return _context.Movies
                .Include(m => m.Director)
                .FirstOrDefault(m => m.Id == id);
        }

        public void Add(Movie movie)
        {
            _logger.LogInformation("REPO: Adding new movie: {MovieTitle}", movie.Title);
            _context.Movies.Add(movie);
        }

        public void Update(Movie movie)
        {
            _logger.LogInformation("REPO: Updating movie with ID: {MovieId}", movie.Id);
            _context.Movies.Update(movie);
        }

        public void Delete(Movie movie)
        {
            _logger.LogWarning("REPO: Deleting movie with ID: {MovieId}", movie.Id);
            _context.Movies.Remove(movie);
        }

        public void SaveChanges()
        {
            try
            {
                _context.SaveChanges();
                _logger.LogInformation("REPO: Database changes saved successfully.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "REPO: An error occurred while saving changes to the database.");
                throw;
            }
        }
    }
}
