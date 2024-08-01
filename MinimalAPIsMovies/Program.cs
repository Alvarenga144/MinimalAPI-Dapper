using MinimalAPIsMovies.Entities;

var builder = WebApplication.CreateBuilder(args);

// Services zone - Begin

// Services zone - End

var app = builder.Build();

// Middlewares zone - Begin

app.MapGet("/", () => "Hello World!");
app.MapGet("/genres", () =>
{
    var genres = new List<Genre>()
    {
        new Genre
        {
            Id = 1,
            Name = "Drama",
        },
        new Genre
        {
            Id = 2,
            Name = "Comedy",
        },
        new Genre
        {
            Id = 3,
            Name = "Horror",
        },
    };

    return genres;
});

// Middlewares zone - End

app.Run();