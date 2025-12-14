using KinoLib.Api.Configuration;
using KinoLib.Api.Models;
using KinoLib.Api.Repositories;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Options;

namespace KinoLib.Api.Services
{
    public class ReviewService : IReviewService
    {
        private readonly IReviewRepository _reviewRepository;
        private readonly IMovieRepository _movieRepository;
        private readonly IMemoryCache _cache;
        private readonly ILogger<ReviewService> _logger;
        private readonly ApiSettings _apiSettings;

        private const string AllReviewsCacheKey = "AllReviewsCache";
        private const string MovieReviewsCacheKeyPrefix = "MovieReviews_";

        public ReviewService(
            IReviewRepository reviewRepository,
            IMovieRepository movieRepository,
            IMemoryCache cache,
            ILogger<ReviewService> logger,
            IOptions<ApiSettings> apiSettingsOptions)
        {
            _reviewRepository = reviewRepository;
            _movieRepository = movieRepository;
            _cache = cache;
            _logger = logger;
            _apiSettings = apiSettingsOptions.Value;
        }

        public List<Review> GetAllReviews()
        {
            if (_apiSettings.EnableCaching)
            {
                if (_cache.TryGetValue(AllReviewsCacheKey, out List<Review> reviews))
                {
                    _logger.LogInformation("Serving reviews from cache.");
                    return reviews;
                }

                reviews = _reviewRepository.GetAll();
                var cacheOptions = new MemoryCacheEntryOptions()
                    .SetSlidingExpiration(TimeSpan.FromMinutes(_apiSettings.CacheDurationMinutes));

                _cache.Set(AllReviewsCacheKey, reviews, cacheOptions);
                return reviews;
            }

            return _reviewRepository.GetAll();
        }

        public Review? GetReviewById(int id)
        {
            _logger.LogInformation("Getting review by ID: {ReviewId}", id);

            if (id <= 0)
            {
                _logger.LogWarning("Invalid review ID: {ReviewId}", id);
                return null;
            }

            return _reviewRepository.GetById(id);
        }

        public Review AddReview(ReviewCreateDto reviewDto)
        {
            if (_apiSettings.EnableCaching)
            {
                _cache.Remove(AllReviewsCacheKey);
                _cache.Remove($"{MovieReviewsCacheKeyPrefix}{reviewDto.MovieId}");
                _logger.LogInformation("Cache invalidated after adding review.");
            }

            var movie = _movieRepository.GetById(reviewDto.MovieId);
            if (movie == null)
            {
                _logger.LogError("Movie with ID {MovieId} not found.", reviewDto.MovieId);
                throw new ArgumentException($"Movie with ID {reviewDto.MovieId} does not exist.");
            }

            var review = new Review
            {
                Author = reviewDto.Author,
                Content = reviewDto.Content,
                Rating = Math.Clamp(reviewDto.Rating, 1, 10),
                CreatedAt = DateTime.UtcNow,
                MovieId = reviewDto.MovieId,
                Movie = movie
            };

            _reviewRepository.Add(review);
            _reviewRepository.SaveChanges();

            _logger.LogInformation("Review added successfully. ID: {ReviewId}", review.Id);
            return review;
        }

        public Review? UpdateReview(int id, ReviewUpdateDto reviewDto)
        {
            var existingReview = _reviewRepository.GetById(id);
            if (existingReview == null)
            {
                _logger.LogWarning("Review with ID {ReviewId} not found.", id);
                return null;
            }

            if (_apiSettings.EnableCaching)
            {
                _cache.Remove(AllReviewsCacheKey);
                _cache.Remove($"{MovieReviewsCacheKeyPrefix}{existingReview.MovieId}");
                _logger.LogInformation("Cache invalidated after updating review ID: {ReviewId}", id);
            }

            existingReview.Author = reviewDto.Author;
            existingReview.Content = reviewDto.Content;
            existingReview.Rating = Math.Clamp(reviewDto.Rating, 1, 10);

            _reviewRepository.Update(existingReview);
            _reviewRepository.SaveChanges();

            _logger.LogInformation("Review with ID {ReviewId} updated.", id);
            return existingReview;
        }

        public bool DeleteReview(int id)
        {
            var review = _reviewRepository.GetById(id);
            if (review == null)
            {
                _logger.LogWarning("Review with ID {ReviewId} not found.", id);
                return false;
            }

            if (_apiSettings.EnableCaching)
            {
                _cache.Remove(AllReviewsCacheKey);
                _cache.Remove($"{MovieReviewsCacheKeyPrefix}{review.MovieId}");
                _logger.LogInformation("Cache invalidated after deleting review ID: {ReviewId}", id);
            }

            _reviewRepository.Delete(review);
            _reviewRepository.SaveChanges();

            _logger.LogInformation("Review with ID {ReviewId} deleted.", id);
            return true;
        }

        public void DeleteAllReviews()
        {
            var allReviews = _reviewRepository.GetAll();

            if (_apiSettings.EnableCaching)
            {
                _cache.Remove(AllReviewsCacheKey);

                var uniqueMovieIds = allReviews.Select(r => r.MovieId).Distinct();
                foreach (var movieId in uniqueMovieIds)
                {
                    _cache.Remove($"{MovieReviewsCacheKeyPrefix}{movieId}");
                }

                _logger.LogInformation("All review cache entries invalidated.");
            }

            foreach (var review in allReviews)
            {
                _reviewRepository.Delete(review);
            }
            _reviewRepository.SaveChanges();

            _logger.LogWarning("All reviews deleted. Count: {Count}", allReviews.Count);
        }

        public List<Review> GetReviewsByMovieId(int movieId)
        {
            if (movieId <= 0)
            {
                _logger.LogWarning("Invalid movie ID: {MovieId}", movieId);
                return new List<Review>();
            }

            var cacheKey = $"{MovieReviewsCacheKeyPrefix}{movieId}";

            if (_apiSettings.EnableCaching)
            {
                var cacheDuration = TimeSpan.FromMinutes(_apiSettings.CacheDurationMinutes);

                if (_cache.TryGetValue(cacheKey, out List<Review> reviews))
                {
                    _logger.LogInformation("Serving reviews for movie {MovieId} from cache.", movieId);
                    return reviews;
                }

                reviews = _reviewRepository.GetAll()
                    .Where(r => r.MovieId == movieId)
                    .OrderByDescending(r => r.CreatedAt)
                    .ToList();

                var cacheEntryOptions = new MemoryCacheEntryOptions()
                    .SetSlidingExpiration(cacheDuration);

                _cache.Set(cacheKey, reviews, cacheEntryOptions);
                return reviews;
            }

            return _reviewRepository.GetAll()
                .Where(r => r.MovieId == movieId)
                .OrderByDescending(r => r.CreatedAt)
                .ToList();
        }

        public double GetAverageRatingForMovie(int movieId)
        {
            if (movieId <= 0)
            {
                _logger.LogWarning("Invalid movie ID: {MovieId}", movieId);
                return 0;
            }

            var reviews = GetReviewsByMovieId(movieId);

            if (!reviews.Any())
            {
                _logger.LogInformation("No reviews found for movie ID {MovieId}.", movieId);
                return 0;
            }

            var average = reviews.Average(r => r.Rating);
            var roundedAverage = Math.Round(average, 2);

            _logger.LogInformation("Average rating for movie {MovieId} is {Average}", movieId, roundedAverage);
            return roundedAverage;
        }
    }
}