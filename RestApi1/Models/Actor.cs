namespace KinoLib.Api.Models
{
    public class Actor
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public DateTime BirthDate { get; set; }
        public string Nationality { get; set; } = string.Empty;
        public List<MovieActor> MovieActors { get; set; } = new();
    }

    public class ActorCreateDto
    {
        public string Name { get; set; } = string.Empty;
        public DateTime BirthDate { get; set; }
        public string Nationality { get; set; } = string.Empty;
        public List<int> MovieIds { get; set; } = new();
    }

    public class ActorUpdateDto
    {
        public string Name { get; set; } = string.Empty;
        public DateTime BirthDate { get; set; }
        public string Nationality { get; set; } = string.Empty;
    }
}
