using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Options;
using KinoLib.Api.Configuration;
using KinoLib.Api.Models;
using KinoLib.Api.Repositories;

namespace KinoLib.Api.Services
{
    public class MovieService : IMovieService
    {
        private readonly IMovieRepository _repository;
        private readonly IDirectorRepository _directorRepository;
        private readonly IActorRepository _actorRepository;

        private readonly IMemoryCache _cache;
        private readonly ILogger<MovieService> _logger;
        private readonly ApiSettings _apiSettings;
        private const string AllMoviesCacheKey = "AllMoviesCache";

        public MovieService(
            IMovieRepository repository, 
            IDirectorRepository directorRepository,
            IMemoryCache cache,
            ILogger<MovieService> logger,
            IOptions<ApiSettings> apiSettingsOptions)
        {
            _repository = repository;
            _directorRepository = directorRepository;
            _cache = cache;
            _logger = logger;
            _apiSettings = apiSettingsOptions.Value;
        }


        public List<Movie> GetAllMovies()
        {
            if (_apiSettings.EnableCaching)
            {
                var cacheDuration = TimeSpan.FromMinutes(_apiSettings.CacheDurationMinutes);

                if (_cache.TryGetValue(AllMoviesCacheKey, out List<Movie> movies))
                {
                    _logger.LogInformation("SERVICE: Serving all movies from cache (TTL: {Minutes} min).", cacheDuration.TotalMinutes);
                    return movies;
                }

                _logger.LogInformation("SERVICE: Caching ENABLED. Fetching movies from repository and adding to cache.");
                movies = _repository.GetAll();

                var cacheEntryOptions = new MemoryCacheEntryOptions()
                    .SetSlidingExpiration(cacheDuration);

                _cache.Set(AllMoviesCacheKey, movies, cacheEntryOptions);

                return movies;
            }

            _logger.LogInformation("SERVICE: Caching DISABLED by config. Fetching movies directly from repository.");
            return _repository.GetAll();
        }

        public Movie? GetMovieById(int id)
        {
            _logger.LogInformation("SERVICE: Getting movie by ID: {MovieId}", id);
            return _repository.GetById(id);
        }


        public Movie AddMovie(MovieCreateDto movieDto)
        {
            if (_apiSettings.EnableCaching) _cache.Remove(AllMoviesCacheKey);

            var director = _directorRepository.GetById(movieDto.DirectorId);
            if (director == null) throw new ArgumentException($"Director {movieDto.DirectorId} не знайдено");

            var movie = new Movie
            {
                Title = movieDto.Title,
                Year = movieDto.Year,
                Genre = movieDto.Genre,
                Duration = movieDto.Duration,
                DirectorId = movieDto.DirectorId,
                Director = director,
                MovieActors = new List<MovieActor>()
            };

            foreach (var actorId in movieDto.ActorIds ?? new List<int>())
            {
                var actor = _actorRepository.GetById(actorId);
                if (actor != null)
                {
                    movie.MovieActors.Add(new MovieActor
                    {
                        Actor = actor,
                        Movie = movie,
                        ActorId = actorId,
                        MovieId = movie.Id
                    });
                }
            }

            _repository.Add(movie);
            _repository.SaveChanges();
            return _repository.GetById(movie.Id);
        }

        public Movie? UpdateMovie(int id, MovieCreateDto movieDto)
        {
            var existingMovie = _repository.GetById(id);
            if (existingMovie == null) return null;

            if (_apiSettings.EnableCaching)
            {
                _cache.Remove(AllMoviesCacheKey);
                _logger.LogInformation("SERVICE: Cache invalidated after updating movie ID: {MovieId}", id);
            }

            var director = _directorRepository.GetById(movieDto.DirectorId);
            if (director == null)
            {
                _logger.LogError("SERVICE: Director with ID {DirectorId} not found during movie update.", movieDto.DirectorId);
                throw new ArgumentException($"Director with ID {movieDto.DirectorId} does not exist.");
            }

            existingMovie.Title = movieDto.Title;
            existingMovie.Year = movieDto.Year;
            existingMovie.Genre = movieDto.Genre;
            existingMovie.Duration = movieDto.Duration;
            existingMovie.DirectorId = movieDto.DirectorId;
            existingMovie.Director = director;

            _repository.Update(existingMovie);
            _repository.SaveChanges();

            return _repository.GetById(id);
        }

        public bool DeleteMovie(int id)
        {
            var movie = _repository.GetById(id);
            if (movie == null) return false;

            if (_apiSettings.EnableCaching)
            {
                _cache.Remove(AllMoviesCacheKey);
                _logger.LogInformation("SERVICE: Cache invalidated after deleting movie ID: {MovieId}", id);
            }

            _repository.Delete(movie);
            _repository.SaveChanges();
            return true;
        }

        public void DeleteAllMovies()
        {
            if (_apiSettings.EnableCaching)
            {
                _cache.Remove(AllMoviesCacheKey);
                _logger.LogInformation("SERVICE: Cache invalidated after deleting all movies.");
            }

            var allMovies = _repository.GetAll();
            foreach (var movie in allMovies)
            {
                _repository.Delete(movie);
            }
            _repository.SaveChanges();
        }

        public void AddActorToMovie(int movieId, int actorId)
        {
            var movie = _repository.GetById(movieId);
            var actor = _actorRepository.GetById(actorId);

            if (movie == null || actor == null)
            {
                throw new ArgumentException("Movie or Actor not found");
            }

            var movieActor = new MovieActor { MovieId = movieId, ActorId = actorId };
            movie.MovieActors.Add(movieActor);
            _repository.Update(movie);
            _repository.SaveChanges();
        }
    }
}
