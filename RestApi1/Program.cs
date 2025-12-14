using FluentValidation;
using FluentValidation.AspNetCore;
using KinoLib.Api.Configuration;
using KinoLib.Api.Data;  
using KinoLib.Api.Models;
using KinoLib.Api.Repositories; 
using KinoLib.Api.Services;
using KinoLib.Api.Validators;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

builder.Configuration.AddJsonFile("appsettings.json", optional: false, reloadOnChange: true);

var connectionString = builder.Configuration.GetConnectionString("DefaultConnection")
    ?? "Data Source=kino.db";

builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlite(connectionString));

builder.Services.AddScoped<IDirectorRepository, DirectorRepository>();
builder.Services.AddScoped<IMovieRepository, MovieRepository>();
builder.Services.AddScoped<IActorRepository, ActorRepository>();
builder.Services.AddScoped<IReviewRepository, ReviewRepository>();

builder.Services.AddScoped<IDirectorService, DirectorService>();
builder.Services.AddScoped<IMovieService, MovieService>();
builder.Services.AddScoped<IActorService, ActorService>();
builder.Services.AddScoped<IReviewService, ReviewService>();

builder.Services.AddFluentValidationAutoValidation();
builder.Services.AddValidatorsFromAssembly(typeof(Program).Assembly);


builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.ReferenceHandler = System.Text.Json.Serialization.ReferenceHandler.IgnoreCycles;
        options.JsonSerializerOptions.PropertyNameCaseInsensitive = true;
    });


builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new Microsoft.OpenApi.Models.OpenApiInfo
    {
        Title = "KinoLib API",
        Version = "v1",
        Description = "API для управління фільмами, режисерами, акторами та відгуками"
    });
});


builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyMethod()
              .AllowAnyHeader();
    });
});


builder.Services.AddMemoryCache();
builder.Services.Configure<ApiSettings>(
    builder.Configuration.GetSection(ApiSettings.ApiSettingsSection));


builder.Services.AddLogging(configure =>
{
    configure.AddConsole();
    configure.AddDebug();
});

var app = builder.Build();


using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    try
    {
        db.Database.EnsureCreated();
        if (!db.Directors.Any())
        {
            SeedData.Initialize(db);
        }
    }
    catch (Exception ex)
    {
        var logger = scope.ServiceProvider.GetRequiredService<ILogger<Program>>();
        logger.LogError(ex, "Помилка при ініціалізації бази даних");
    }
}


app.UseCors("AllowAll");

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "KinoLib API v1");
        c.RoutePrefix = "swagger";
    });
}

app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();

app.Run();


public static class SeedData
{
    public static void Initialize(AppDbContext context)
    {
        if (context.Directors.Any()) return;

        var directors = new[]
        {
            new Director { Name = "Christopher Nolan", BirthDate = new DateTime(1970, 7, 30), Nationality = "British" },
            new Director { Name = "Quentin Tarantino", BirthDate = new DateTime(1963, 3, 27), Nationality = "American" }
        };

        context.Directors.AddRange(directors);
        context.SaveChanges();

        var movies = new[]
        {
            new Movie
            {
                Title = "Inception",
                Year = 2010,
                Genre = "Sci-Fi",
                Duration = 148,
                DirectorId = directors[0].Id
            },
            new Movie
            {
                Title = "Pulp Fiction",
                Year = 1994,
                Genre = "Crime",
                Duration = 154,
                DirectorId = directors[1].Id
            }
        };

        context.Movies.AddRange(movies);
        context.SaveChanges();

        var actors = new[]
        {
            new Actor { Name = "Leonardo DiCaprio", BirthDate = new DateTime(1974, 11, 11), Nationality = "American" },
            new Actor { Name = "Samuel L. Jackson", BirthDate = new DateTime(1948, 12, 21), Nationality = "American" }
        };

        context.Actors.AddRange(actors);
        context.SaveChanges();

        var movieActors = new[]
        {
            new MovieActor { MovieId = movies[0].Id, ActorId = actors[0].Id },
            new MovieActor { MovieId = movies[1].Id, ActorId = actors[1].Id }
        };

        context.MovieActors.AddRange(movieActors);
        context.SaveChanges();
    }
}