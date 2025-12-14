using KinoLib.Api.Models;

namespace KinoLib.Api.Repositories
{
    public interface IActorRepository
    {
        List<Actor> GetAll();
        Actor? GetById(int id);
        void Add(Actor actor);
        void Update(Actor actor);
        void Delete(Actor actor);
        void SaveChanges();
    }
}
