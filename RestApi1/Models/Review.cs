namespace KinoLib.Api.Models
{
    public class Review
    {
        public int Id { get; set; }
        public string Author { get; set; } = string.Empty;
        public string Content { get; set; } = string.Empty;
        public int Rating { get; set; }
        public DateTime CreatedAt { get; set; }

        public int MovieId { get; set; }
        public Movie Movie { get; set; } = null!;
    }

    public class ReviewCreateDto
    {
        public string Author { get; set; } = string.Empty;
        public string Content { get; set; } = string.Empty;
        public int Rating { get; set; }

        public int MovieId { get; set; }
    }

    public class ReviewUpdateDto
    {
        public string Author { get; set; } = string.Empty;
        public string Content { get; set; } = string.Empty;
        public int Rating { get; set; }
    }
}
