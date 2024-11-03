using MinimalAPIsMovies.Endpoints;
using MinimalAPIsMovies.Repositories;
using MinimalAPIsMovies.Services;

var builder = WebApplication.CreateBuilder(args);

// Services zone - Begin
builder.Services.AddOutputCache();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddScoped<IGenresRepository, GenresRepository>();
builder.Services.AddScoped<IActorsRepository, ActorsRepository>();
builder.Services.AddScoped<IMoviesRepository, MoviesRepository>();
builder.Services.AddScoped<ICommentsRepository, CommentsRepository>();

builder.Services.AddTransient<IFileStorage, AzureFileStorage>();
builder.Services.AddHttpContextAccessor();
builder.Services.AddAutoMapper(typeof(Program));
// Services zone - End

var app = builder.Build();

// Middlewares zone - Begin
app.UseSwagger();
app.UseSwaggerUI();
app.UseOutputCache();

app.MapGroup("/genres").MapGenres();
app.MapGroup("/actors").MapActors();
app.MapGroup("/movies").MapMovies();
// Middlewares zone - End

app.Run();
