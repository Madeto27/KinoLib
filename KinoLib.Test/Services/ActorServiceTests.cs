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
    public class ActorServiceTests
    {
        private readonly Mock<IActorRepository> _mockRepo;
        private readonly Mock<IMovieRepository> _mockMovieRepo;
        private readonly Mock<IMemoryCache> _mockCache;
        private readonly Mock<ILogger<ActorService>> _mockLogger;
        private readonly ApiSettings _apiSettings;
        private ActorService _service;

        public ActorServiceTests()
        {
            _mockRepo = new Mock<IActorRepository>();
            _mockMovieRepo = new Mock<IMovieRepository>();
            _mockCache = new Mock<IMemoryCache>();
            _mockLogger = new Mock<ILogger<ActorService>>();
            _apiSettings = new ApiSettings { EnableCaching = true, CacheDurationMinutes = 5 };

            _service = new ActorService(
                _mockRepo.Object,
                _mockMovieRepo.Object,
                _mockCache.Object,
                _mockLogger.Object,
                Options.Create(_apiSettings)
            );
        }


        [Fact]
        public void GetAllActors_ReturnsFromRepository_WhenCacheDisabled()
        {
            _apiSettings.EnableCaching = false;
            var actors = new List<Actor> { new Actor { Id = 1, Name = "Actor 1" } };
            _mockRepo.Setup(r => r.GetAll()).Returns(actors);

            var result = _service.GetAllActors();

            Assert.Single(result);
            _mockRepo.Verify(r => r.GetAll(), Times.Once);
        }

        [Fact]
        public void GetActorById_ReturnsActor_WhenExists()
        {
            var expectedActor = new Actor { Id = 1, Name = "Test" };
            _mockRepo.Setup(r => r.GetById(1)).Returns(expectedActor);

            var result = _service.GetActorById(1);

            Assert.Equal(expectedActor, result);
        }

        [Fact]
        public void AddActor_CreatesActorWithMovies_WhenValidData()
        {
            var actorDto = new ActorCreateDto
            {
                Name = "Test Actor",
                MovieIds = new List<int> { 1, 2 }
            };

            var movies = new List<Movie>
            {
                new Movie { Id = 1, Title = "Movie 1" },
                new Movie { Id = 2, Title = "Movie 2" }
            };

            _mockMovieRepo.Setup(r => r.GetById(1)).Returns(movies[0]);
            _mockMovieRepo.Setup(r => r.GetById(2)).Returns(movies[1]);

            var result = _service.AddActor(actorDto);

            Assert.Equal("Test Actor", result.Name);
            Assert.Equal(2, result.MovieActors.Count);
        }

        [Fact]
        public void UpdateActor_ReturnsNull_WhenActorNotFound()
        {
            _mockRepo.Setup(r => r.GetById(999)).Returns((Actor)null);

            var result = _service.UpdateActor(999, new ActorUpdateDto());

            Assert.Null(result);
            _mockRepo.Verify(r => r.Update(It.IsAny<Actor>()), Times.Never);
        }

        [Fact]
        public void DeleteActor_ReturnsFalse_WhenActorNotFound()
        {
            _mockRepo.Setup(r => r.GetById(999)).Returns((Actor)null);

            var result = _service.DeleteActor(999);

            Assert.False(result);
        }
    }
}
