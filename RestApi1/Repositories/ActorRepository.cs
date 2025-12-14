using KinoLib.Api.Data;
using KinoLib.Api.Models;
using Microsoft.EntityFrameworkCore;

namespace KinoLib.Api.Repositories
{
    public class ActorRepository : IActorRepository
    {
        private readonly AppDbContext _context;
        private readonly ILogger<ActorRepository> _logger;

        public ActorRepository(AppDbContext context, ILogger<ActorRepository> logger)
        {
            _context = context;
            _logger = logger;
        }

        public List<Actor> GetAll()
        {
            _logger.LogInformation("Getting all actors.");
            return _context.Actors
                .Include(a => a.MovieActors)
                .ThenInclude(ma => ma.Movie)
                .ToList();
        }

        public Actor? GetById(int id)
        {
            _logger.LogInformation("Getting actor by ID: {ActorId}", id);
            return _context.Actors
                .Include(a => a.MovieActors)
                .ThenInclude(ma => ma.Movie)
                .FirstOrDefault(a => a.Id == id);
        }

        public void Add(Actor actor)
        {
            _logger.LogInformation("Adding a new actor: {ActorName}", actor.Name);
            _context.Actors.Add(actor);
        }

        public void Update(Actor actor)
        {
            _logger.LogInformation("Updating actor with ID: {ActorId}", actor.Id);
            _context.Actors.Update(actor);
        }

        public void Delete(Actor actor)
        {
            _logger.LogWarning("Deleting actor with ID: {ActorId}", actor.Id);
            _context.Actors.Remove(actor);
        }

        public void SaveChanges()
        {
            try
            {
                _context.SaveChanges();
                _logger.LogInformation("Actor changes saved successfully.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while saving actor changes.");
                throw;
            }
        }
    }
}
