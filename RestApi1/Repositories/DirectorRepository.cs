using KinoLib.Api.Models;
using KinoLib.Api.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace KinoLib.Api.Repositories
{
    public class DirectorRepository : IDirectorRepository
    {
        private readonly AppDbContext _context;
        private readonly ILogger<DirectorRepository> _logger;

        public DirectorRepository(AppDbContext context, ILogger<DirectorRepository> logger)
        {
            _context = context;
            _logger = logger;
        }

        public List<Director> GetAll()
        {
            _logger.LogInformation("Getting all directors (including movies).");
            return _context.Directors
                .Include(d => d.Movies)
                .ToList();
        }

        public Director? GetById(int id)
        {
            _logger.LogInformation("Getting director by ID: {DirectorId}", id);
            return _context.Directors
                .Include(d => d.Movies)
                .FirstOrDefault(d => d.Id == id);
        }

        public void Add(Director director)
        {
            _logger.LogInformation("Adding a new director: {DirectorName}", director.Name);
            _context.Directors.Add(director);
        }

        public void Update(Director director)
        {
            _logger.LogInformation("Updating director with ID: {DirectorId}", director.Id);
            _context.Directors.Update(director);
        }

        public void Delete(Director director)
        {
            _logger.LogWarning("Deleting director with ID: {DirectorId}", director.Id);
            _context.Directors.Remove(director);
        }

        public void SaveChanges()
        {
            try
            {
                _context.SaveChanges();
                _logger.LogInformation("Changes saved successfully to the database.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while saving changes.");
                throw;
            }
        }
    }
}