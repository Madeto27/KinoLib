using KinoLib.Api.Models;

namespace KinoLib.Api.Services
{
    public interface IActorService
    {
        List<Actor> GetAllActors();
        Actor? GetActorById(int id);
        Actor AddActor(ActorCreateDto actorDto);
        Actor? UpdateActor(int id, ActorUpdateDto actorDto);
        bool DeleteActor(int id);
        void DeleteAllActors();

        List<Movie> GetMoviesByActor(int actorId);
    }
}
