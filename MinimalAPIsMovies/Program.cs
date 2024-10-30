using Microsoft.AspNetCore.OutputCaching;
using MinimalAPIsMovies.Entities;
using MinimalAPIsMovies.Repositories;

var builder = WebApplication.CreateBuilder(args);

// Services zone - Begin

builder.Services.AddScoped<IGenresRepository, GenresRepository>();
/*
builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(configuration =>
    {
        configuration.WithOrigins(builder.Configuration["AllowedOrigins"]!).AllowAnyMethod().AllowAnyHeader();
    });

    options.AddPolicy("free", configuration =>
    {
        configuration.AllowAnyOrigin().AllowAnyHeader().AllowAnyMethod();
    });
});
*/
builder.Services.AddOutputCache();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Services zone - End

var app = builder.Build();

// Middlewares zone - Begin

app.UseSwagger();
app.UseSwaggerUI();

//app.UseCors();
app.UseOutputCache();

app.MapGet("/", () => "Hello World!");

app.MapGet("/genres", async (IGenresRepository genresRepository) =>
{
    return await genresRepository.GetAll();
}).CacheOutput(c => c.Expire(TimeSpan.FromSeconds(60)).Tag("genres-get"));

app.MapGet("/genres/{id:int}", async (int id, IGenresRepository genresRepository) =>
{
    var genre = await genresRepository.GetById(id);

    if (genre is null)
    {
        return Results.NotFound();
    }

    return Results.Ok(genre);
});

app.MapPost("/genres", async (Genre genre, IGenresRepository genresRepository, IOutputCacheStore outputCacheStore) =>
{
    await genresRepository.Create(genre);
    await outputCacheStore.EvictByTagAsync("genres-get", default);
    return TypedResults.Created($"/genres/{genre.Id}", genre);
});

app.MapPut("/genres/{id:int}", async (int id, Genre genre, IGenresRepository repository, IOutputCacheStore outputCacheStore) =>
{
    var exist = await repository.Exist(id);

    if (!exist)
    {
        return Results.NotFound();
    }

    await repository.Update(genre);
    await outputCacheStore.EvictByTagAsync("genres-get", default);
    return Results.NoContent();
});

app.MapDelete("/genres/{id:int}", async (int id, IGenresRepository repository, IOutputCacheStore outputCacheStore) =>
{
    var exist = await repository.Exist(id);

    if (!exist)
    {
        return Results.NotFound();
    }

    await repository.Delete(id);
    await outputCacheStore.EvictByTagAsync("genres-get", default);
    return Results.NoContent();
});

// Middlewares zone - End

app.Run();