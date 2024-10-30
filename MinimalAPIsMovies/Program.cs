using MinimalAPIsMovies.Endpoints;
using MinimalAPIsMovies.Repositories;

var builder = WebApplication.CreateBuilder(args);

// Services zone - Begin
builder.Services.AddScoped<IGenresRepository, GenresRepository>();
builder.Services.AddOutputCache();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
// Services zone - End

var app = builder.Build();

// Middlewares zone - Begin
app.UseSwagger();
app.UseSwaggerUI();
app.UseOutputCache();

app.MapGroup("/genres").MapGenres();
// Middlewares zone - End

app.Run();
