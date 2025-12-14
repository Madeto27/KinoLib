using KinoLib.Api.Models;

namespace KinoLib.Api.Repositories
{
    public interface IDirectorRepository
    {
        List<Director> GetAll();
        Director? GetById(int id);
        void Add(Director director);
        void Update(Director director);
        void Delete(Director director);
        void SaveChanges();
    }
}
