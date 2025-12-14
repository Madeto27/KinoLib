using KinoLib.Api.Models;

namespace KinoLib.Api.Services
{
    public interface IDirectorService
    {
        List<Director> GetAllDirectors();
        Director? GetDirectorById(int id);
        Director AddDirector(DirectorCreateDto directorDto);
        Director? UpdateDirector(int id, DirectorCreateDto directorDto);
        bool DeleteDirector(int id);
        void DeleteAllDirectors();
    }
}