using KinoLib.Api.Data;
using KinoLib.Api.Models;
using KinoLib.Api.Repositories;
using Microsoft.EntityFrameworkCore;

namespace KinoLib.Api.Repositories
{
    public class ReviewRepository : IReviewRepository
    {
        private readonly AppDbContext _context;
        private readonly ILogger<ReviewRepository> _logger;

        public ReviewRepository(AppDbContext context, ILogger<ReviewRepository> logger)
        {
            _context = context;
            _logger = logger;
        }

        public List<Review> GetAll()
        {
            _logger.LogInformation("Getting all reviews.");
            return _context.Reviews
                .Include(r => r.Movie)
                .ThenInclude(m => m.Director)
                .OrderByDescending(r => r.CreatedAt)
                .ToList();
        }

        public Review? GetById(int id)
        {
            _logger.LogInformation("Getting review by ID: {ReviewId}", id);
            return _context.Reviews
                .Include(r => r.Movie)
                .ThenInclude(m => m.Director)
                .FirstOrDefault(r => r.Id == id);
        }

        public void Add(Review review)
        {
            _logger.LogInformation("Adding new review for movie ID: {MovieId}", review.MovieId);
            _context.Reviews.Add(review);
        }

        public void Update(Review review)
        {
            _logger.LogInformation("Updating review with ID: {ReviewId}", review.Id);
            _context.Reviews.Update(review);
        }

        public void Delete(Review review)
        {
            _logger.LogInformation("Deleting review with ID: {ReviewId}", review.Id);
            _context.Reviews.Remove(review);
        }

        public void SaveChanges()
        {
            try
            {
                _context.SaveChanges();
                _logger.LogInformation("Review repository changes saved successfully.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error saving changes in ReviewRepository");
                throw;
            }
        }
    }
}