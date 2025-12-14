using KinoLib.Api.Models;
using Microsoft.EntityFrameworkCore;

namespace KinoLib.Api.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        public DbSet<Director> Directors { get; set; }
        public DbSet<Movie> Movies { get; set; }
        public DbSet<Actor> Actors { get; set; }
        public DbSet<Review> Reviews { get; set; }
        public DbSet<MovieActor> MovieActors { get; set; }
    }
}
