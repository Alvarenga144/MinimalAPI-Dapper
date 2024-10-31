using MinimalAPIsMovies.Endpoints;
using MinimalAPIsMovies.Repositories;

var builder = WebApplication.CreateBuilder(args);

// Services zone - Begin
builder.Services.AddOutputCache();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddScoped<IGenresRepository, GenresRepository>();
builder.Services.AddScoped<IActorsRepository, ActorsRepository>();
builder.Services.AddAutoMapper(typeof(Program));
// Services zone - End

var app = builder.Build();

// Middlewares zone - Begin
app.UseSwagger();
app.UseSwaggerUI();
app.UseOutputCache();

app.MapGroup("/genres").MapGenres();
app.MapGroup("/actors").MapActors();
// Middlewares zone - End

app.Run();
