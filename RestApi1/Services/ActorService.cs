using KinoLib.Api.Configuration;
using KinoLib.Api.Models;
using KinoLib.Api.Repositories;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Options;

namespace KinoLib.Api.Services
{
    public class ActorService : IActorService
    {
        private readonly IActorRepository _repository;
        private readonly IMovieRepository _movieRepository;
        private readonly IMemoryCache _cache;
        private readonly ILogger<ActorService> _logger;
        private readonly ApiSettings _apiSettings;
        private const string AllActorsCacheKey = "AllActorsCache";

        public ActorService(
            IActorRepository repository,
            IMovieRepository movieRepository,
            IMemoryCache cache,
            ILogger<ActorService> logger,
            IOptions<ApiSettings> apiSettingsOptions)
        {
            _repository = repository;
            _movieRepository = movieRepository;
            _cache = cache;
            _logger = logger;
            _apiSettings = apiSettingsOptions.Value;
        }

        public List<Actor> GetAllActors()
        {
            if (_apiSettings.EnableCaching)
            {
                var cacheDuration = TimeSpan.FromMinutes(_apiSettings.CacheDurationMinutes);
                if (_cache.TryGetValue(AllActorsCacheKey, out List<Actor> actors))
                {
                    _logger.LogInformation("Serving all actors from cache.");
                    return actors;
                }

                actors = _repository.GetAll();
                var cacheEntryOptions = new MemoryCacheEntryOptions()
                    .SetSlidingExpiration(cacheDuration);
                _cache.Set(AllActorsCacheKey, actors, cacheEntryOptions);
                return actors;
            }

            return _repository.GetAll();
        }

        public Actor? GetActorById(int id)
        {
            return _repository.GetById(id);
        }

        public Actor AddActor(ActorCreateDto actorDto)
        {
            if (_apiSettings.EnableCaching)
                _cache.Remove(AllActorsCacheKey);

            var actor = new Actor
            {
                Name = actorDto.Name,
                BirthDate = actorDto.BirthDate,
                Nationality = actorDto.Nationality,
            };

            _repository.Add(actor);
            _repository.SaveChanges();

            foreach (var movieId in actorDto.MovieIds ?? new List<int>())
            {
                var movie = _movieRepository.GetById(movieId);
                if (movie != null)
                {
                    actor.MovieActors.Add(new MovieActor
                    {
                        MovieId = movieId,
                        ActorId = actor.Id,
                        Movie = movie,
                        Actor = actor
                    });
                }
            }

            _repository.Update(actor);
            _repository.SaveChanges();

            return actor;
        }

        public Actor? UpdateActor(int id, ActorUpdateDto actorDto)
        {
            var existingActor = _repository.GetById(id);
            if (existingActor == null) return null;

            if (_apiSettings.EnableCaching)
            {
                _cache.Remove(AllActorsCacheKey);
            }

            existingActor.Name = actorDto.Name;
            existingActor.BirthDate = actorDto.BirthDate;
            existingActor.Nationality = actorDto.Nationality;

            _repository.Update(existingActor);
            _repository.SaveChanges();
            return existingActor;
        }

        public bool DeleteActor(int id)
        {
            var actor = _repository.GetById(id);
            if (actor == null) return false;

            if (_apiSettings.EnableCaching)
            {
                _cache.Remove(AllActorsCacheKey);
            }

            _repository.Delete(actor);
            _repository.SaveChanges();
            return true;
        }

        public void DeleteAllActors()
        {
            if (_apiSettings.EnableCaching)
            {
                _cache.Remove(AllActorsCacheKey);
            }

            var all = _repository.GetAll();
            foreach (var actor in all)
            {
                _repository.Delete(actor);
            }
            _repository.SaveChanges();
        }

        public List<Movie> GetMoviesByActor(int actorId)
        {
            var actor = _repository.GetById(actorId);
            if (actor == null) return new List<Movie>();

            return actor.MovieActors.Select(ma => ma.Movie).ToList();
        }
    }
}