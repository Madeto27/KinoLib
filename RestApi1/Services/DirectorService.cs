using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Options;
using KinoLib.Api.Configuration;
using KinoLib.Api.Models;
using KinoLib.Api.Repositories;

namespace KinoLib.Api.Services
{
    public class DirectorService : IDirectorService
    {
        private readonly IDirectorRepository _repository;

        private readonly IMemoryCache _cache;
        private readonly ILogger<DirectorService> _logger;
        private readonly ApiSettings _apiSettings;

        private const string AllDirectorsCacheKey = "AllDirectorsCache";


        public DirectorService(
            IDirectorRepository repository,
            IMemoryCache cache,
            ILogger<DirectorService> logger,
            IOptions<ApiSettings> apiSettingsOptions)
        {
            _repository = repository;
            _cache = cache;
            _logger = logger;
            _apiSettings = apiSettingsOptions.Value; 
        }


        public List<Director> GetAllDirectors()
        {
            if (_apiSettings.EnableCaching)
            {
                var cacheDuration = TimeSpan.FromMinutes(_apiSettings.CacheDurationMinutes);

                if (_cache.TryGetValue(AllDirectorsCacheKey, out List<Director> directors))
                {
                    _logger.LogInformation("SERVICE: Serving all directors from cache (TTL: {Minutes} min).", cacheDuration.TotalMinutes);
                    return directors;
                }

                _logger.LogInformation("SERVICE: Caching ENABLED. Fetching directors from repository and adding to cache.");
                directors = _repository.GetAll();

                var cacheEntryOptions = new MemoryCacheEntryOptions()
                    .SetSlidingExpiration(cacheDuration);

                _cache.Set(AllDirectorsCacheKey, directors, cacheEntryOptions);

                return directors;
            }

            _logger.LogInformation("SERVICE: Caching DISABLED by config. Fetching directors directly from repository.");
            return _repository.GetAll();
        }

        public Director? GetDirectorById(int id)
        {
            _logger.LogInformation("SERVICE: Getting director by ID: {DirectorId}", id);
            return _repository.GetById(id);
        }


        public Director AddDirector(DirectorCreateDto directorDto)
        {
            if (_apiSettings.EnableCaching)
            {
                _cache.Remove(AllDirectorsCacheKey);
                _logger.LogInformation("SERVICE: Cache invalidated after adding a new director.");
            }

            var director = new Director
            {
                Name = directorDto.Name,
                BirthDate = directorDto.BirthDate,
                Nationality = directorDto.Nationality
            };

            _repository.Add(director);
            _repository.SaveChanges();
            return director;
        }

        public void DeleteAllDirectors()
        {
            if (_apiSettings.EnableCaching)
            {
                _cache.Remove(AllDirectorsCacheKey);
                _logger.LogInformation("SERVICE: Cache invalidated after deleting all directors.");
            }

            var all = _repository.GetAll();
            foreach (var d in all) _repository.Delete(d);
            _repository.SaveChanges();
        }

        public bool DeleteDirector(int id)
        {
            var director = _repository.GetById(id);
            if (director == null) return false;

            if (_apiSettings.EnableCaching)
            {
                _cache.Remove(AllDirectorsCacheKey);
                _logger.LogInformation("SERVICE: Cache invalidated after deleting director ID: {DirectorId}", id);
            }

            _repository.Delete(director);
            _repository.SaveChanges();
            return true;
        }

        public Director? UpdateDirector(int id, DirectorCreateDto directorDto)
        {
            var existingDirector = _repository.GetById(id);
            if (existingDirector == null) return null;

            if (_apiSettings.EnableCaching)
            {
                _cache.Remove(AllDirectorsCacheKey);
                _logger.LogInformation("SERVICE: Cache invalidated after updating director ID: {DirectorId}", id);
            }

            existingDirector.Name = directorDto.Name;
            existingDirector.BirthDate = directorDto.BirthDate;
            existingDirector.Nationality = directorDto.Nationality;

            _repository.Update(existingDirector);
            _repository.SaveChanges();

            return existingDirector;
        }
    }
}
