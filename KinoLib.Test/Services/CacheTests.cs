using Moq;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using KinoLib.Api.Services;
using KinoLib.Api.Repositories;
using KinoLib.Api.Models;
using KinoLib.Api.Configuration;

namespace KinoLib.Test.Services
{
    public class CacheTests
    {
        [Fact]
        public void GetAllActors_ReturnsCachedData_WhenCacheHit()
        {
            var mockRepo = new Mock<IActorRepository>();
            var mockMovieRepo = new Mock<IMovieRepository>();
            var mockCache = new Mock<IMemoryCache>();
            var mockLogger = new Mock<ILogger<ActorService>>();
            var apiSettings = new ApiSettings { EnableCaching = true, CacheDurationMinutes = 5 };

            var cachedActors = new List<Actor> { new Actor { Id = 1, Name = "Cached" } };
            object cachedValue = cachedActors;

            mockCache.Setup(c => c.TryGetValue(It.IsAny<object>(), out cachedValue))
                     .Returns(true);

            var service = new ActorService(
                mockRepo.Object,
                mockMovieRepo.Object,
                mockCache.Object,
                mockLogger.Object,
                Options.Create(apiSettings)
            );

            var result = service.GetAllActors();

            Assert.Single(result);
            Assert.Equal("Cached", result[0].Name);
            mockRepo.Verify(r => r.GetAll(), Times.Never);
        }

        [Fact]
        public void AddActor_ClearsCache_WhenCachingEnabled()
        {
            var mockRepo = new Mock<IActorRepository>();
            var mockMovieRepo = new Mock<IMovieRepository>();
            var mockCache = new Mock<IMemoryCache>();
            var mockLogger = new Mock<ILogger<ActorService>>();
            var apiSettings = new ApiSettings { EnableCaching = true, CacheDurationMinutes = 5 };

            mockCache.Setup(c => c.Remove(It.IsAny<object>()));

            var service = new ActorService(
                mockRepo.Object,
                mockMovieRepo.Object,
                mockCache.Object,
                mockLogger.Object,
                Options.Create(apiSettings)
            );

            service.AddActor(new ActorCreateDto
            {
                Name = "Test",
                BirthDate = DateTime.Now.AddYears(-30),
                Nationality = "Test"
            });

            mockCache.Verify(c => c.Remove(It.IsAny<object>()), Times.AtLeastOnce);
        }
    }
}