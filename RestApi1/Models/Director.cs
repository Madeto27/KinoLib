namespace KinoLib.Api.Models
{
    public class Director
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public DateTime BirthDate { get; set; }
        public string Nationality { get; set; } = string.Empty;

        public List<Movie> Movies { get; set; } = new();
    }

    public class DirectorCreateDto
    {
        public string Name { get; set; } = string.Empty;
        public DateTime BirthDate { get; set; }
        public string Nationality { get; set; } = string.Empty;
    }
}
