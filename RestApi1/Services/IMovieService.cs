using KinoLib.Api.Models;

namespace KinoLib.Api.Services
{
    public interface IMovieService
    {
        List<Movie> GetAllMovies();
        Movie? GetMovieById(int id);
        Movie AddMovie(MovieCreateDto movieDto);
        Movie? UpdateMovie(int id, MovieCreateDto movieDto);
        bool DeleteMovie(int id);
        void DeleteAllMovies();
    }
}
