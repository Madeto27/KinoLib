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
    public class MovieServiceTests
    {
        private readonly Mock<IMovieRepository> _mockRepo;
        private readonly Mock<IDirectorRepository> _mockDirectorRepo;
        private readonly Mock<IActorRepository> _mockActorRepo;
        private readonly Mock<IMemoryCache> _mockCache;
        private readonly Mock<ILogger<MovieService>> _mockLogger;
        private readonly ApiSettings _apiSettings;
        private MovieService _service;

        public MovieServiceTests()
        {
            _mockRepo = new Mock<IMovieRepository>();
            _mockDirectorRepo = new Mock<IDirectorRepository>();
            _mockActorRepo = new Mock<IActorRepository>();
            _mockCache = new Mock<IMemoryCache>();
            _mockLogger = new Mock<ILogger<MovieService>>();
            _apiSettings = new ApiSettings { EnableCaching = false, CacheDurationMinutes = 5 };

            _service = new MovieService(
                _mockRepo.Object,
                _mockDirectorRepo.Object,
                _mockCache.Object,
                _mockLogger.Object,
                Options.Create(_apiSettings)
            );
        }

        [Fact]
        public void AddMovie_ThrowsArgumentException_WhenDirectorNotFound()
        {
            var movieDto = new MovieCreateDto { DirectorId = 999 };
            _mockDirectorRepo.Setup(r => r.GetById(999)).Returns((Director)null);

            Assert.Throws<ArgumentException>(() => _service.AddMovie(movieDto));
        }

        [Fact]
        public void AddMovie_CreatesMovie_WithDirector()
        {
            var director = new Director { Id = 1, Name = "Director" };
            var movieDto = new MovieCreateDto
            {
                Title = "Test Movie",
                Year = 2023,
                Genre = "Action",
                Duration = 120,
                DirectorId = 1,
                ActorIds = new List<int>()
            };

            _mockDirectorRepo.Setup(r => r.GetById(1)).Returns(director);

            Movie savedMovie = null;
            _mockRepo.Setup(r => r.Add(It.IsAny<Movie>()))
                    .Callback<Movie>(movie =>
                    {
                        movie.Id = 1;
                        savedMovie = movie;
                    });


            _mockRepo.Setup(r => r.GetById(It.IsAny<int>()))
                    .Returns<int>(id => savedMovie?.Id == id ? savedMovie : null);

            var result = _service.AddMovie(movieDto);

            Assert.NotNull(result);
            Assert.Equal("Test Movie", result.Title);
            Assert.Equal(1, result.DirectorId);

            _mockRepo.Verify(r => r.Add(It.IsAny<Movie>()), Times.Once);
            _mockRepo.Verify(r => r.SaveChanges(), Times.Once);
        }

        [Fact]
        public void GetAllMovies_ReturnsMoviesWithDirectors()
        {
            var movies = new List<Movie>
            {
                new Movie { Id = 1, Title = "Movie 1", Director = new Director { Id = 1, Name = "Director 1" } }
            };

            _mockRepo.Setup(r => r.GetAll()).Returns(movies);

            var result = _service.GetAllMovies();

            Assert.Single(result);
            Assert.NotNull(result[0].Director);
        }

        [Fact]
        public void DeleteMovie_RemovesMovie_WhenExists()
        {
            var movie = new Movie { Id = 1, Title = "Test Movie" };
            _mockRepo.Setup(r => r.GetById(1)).Returns(movie);

            var result = _service.DeleteMovie(1);

            Assert.True(result);
            _mockRepo.Verify(r => r.Delete(movie), Times.Once);
        }
    }
}
