namespace KinoLib.Api.Models
{
    public class Movie
    {
        public int Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public int Year { get; set; }
        public string Genre { get; set; } = string.Empty;
        public int Duration { get; set; }

        public int DirectorId { get; set; }
        public virtual Director Director { get; set; } = null;
        public List<MovieActor> MovieActors { get; set; } = new();

    }

    public class MovieCreateDto
    {
        public string Title { get; set; } = string.Empty;
        public int Year { get; set; }
        public string Genre { get; set; } = string.Empty;
        public int Duration { get; set; }
        public int DirectorId { get; set; }
        public List<int> ActorIds { get; set; } = new();
    }
}
