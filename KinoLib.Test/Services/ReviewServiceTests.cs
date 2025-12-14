using KinoLib.Api.Configuration;
using KinoLib.Api.Models;
using KinoLib.Api.Repositories;
using KinoLib.Api.Services;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Moq;

namespace KinoLib.Test.Services
{
    public class ReviewServiceTests
    {
        private readonly Mock<IReviewRepository> _mockRepo;
        private readonly Mock<IMovieRepository> _mockMovieRepo;
        private readonly Mock<IMemoryCache> _mockCache;
        private readonly Mock<ILogger<ReviewService>> _mockLogger;
        private readonly ApiSettings _apiSettings;
        private ReviewService _service;

        public ReviewServiceTests()
        {
            _mockRepo = new Mock<IReviewRepository>();
            _mockMovieRepo = new Mock<IMovieRepository>();
            _mockCache = new Mock<IMemoryCache>();
            _mockLogger = new Mock<ILogger<ReviewService>>();
            _apiSettings = new ApiSettings { EnableCaching = false, CacheDurationMinutes = 5 };

            _service = new ReviewService(
                _mockRepo.Object,
                _mockMovieRepo.Object,
                _mockCache.Object,
                _mockLogger.Object,
                Options.Create(_apiSettings)
            );
        }

        [Theory]
        [InlineData(new int[] { 5, 5, 5 }, 5.0)]
        [InlineData(new int[] { 1, 10 }, 5.5)]
        [InlineData(new int[] { }, 0.0)]
        public void GetAverageRatingForMovie_CalculatesCorrectAverage(int[] ratings, double expectedAverage)
        {
            var reviews = ratings.Select(r => new Review { Rating = r, MovieId = 1 }).ToList();
            _mockRepo.Setup(r => r.GetAll()).Returns(reviews);

            var result = _service.GetAverageRatingForMovie(1);

            Assert.Equal(expectedAverage, result);
        }

        [Fact]
        public void AddReview_ThrowsArgumentException_WhenMovieNotFound()
        {
            var reviewDto = new ReviewCreateDto { MovieId = 999 };
            _mockMovieRepo.Setup(r => r.GetById(999)).Returns((Movie)null);

            Assert.Throws<ArgumentException>(() => _service.AddReview(reviewDto));
        }

        [Fact]
        public void AddReview_ClampsRating_WhenOutOfRange()
        {
            var movie = new Movie { Id = 1 };
            var reviewDto = new ReviewCreateDto { Rating = 15, MovieId = 1 };

            _mockMovieRepo.Setup(r => r.GetById(1)).Returns(movie);

            var result = _service.AddReview(reviewDto);

            Assert.Equal(10, result.Rating);
        }

        [Fact]
        public void GetReviewsByMovieId_ReturnsFilteredReviews()
        {
            var reviews = new List<Review>
            {
                new Review { Id = 1, MovieId = 1 },
                new Review { Id = 2, MovieId = 2 },
                new Review { Id = 3, MovieId = 1 }
            };

            _mockRepo.Setup(r => r.GetAll()).Returns(reviews);

            var result = _service.GetReviewsByMovieId(1);

            Assert.Equal(2, result.Count);
            Assert.All(result, r => Assert.Equal(1, r.MovieId));
        }

        [Fact]
        public void UpdateReview_ReturnsNull_WhenReviewNotFound()
        {
            _mockRepo.Setup(r => r.GetById(999)).Returns((Review)null);

            var result = _service.UpdateReview(999, new ReviewUpdateDto());

            Assert.Null(result);
        }
    }
}
