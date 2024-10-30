using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.OutputCaching;
using MinimalAPIsMovies.Entities;
using MinimalAPIsMovies.Repositories;

namespace MinimalAPIsMovies.Endpoints
{
    public static class GenresEndpoints
    {
        public static RouteGroupBuilder MapGenres(this RouteGroupBuilder group)
        {
            group.MapGet("/", GetGenresList)
                .CacheOutput(c => c.Expire(TimeSpan.FromSeconds(60)).Tag("genres-get"));
            group.MapGet("/{id:int}", GetById);
            group.MapPost("/", Create);
            group.MapPut("/{id:int}", Update);
            group.MapDelete("/{id:int}", Delete);
            return group;
        }

        static async Task<Ok<List<Genre>>> GetGenresList(IGenresRepository genresRepository)
        {
            var genres = await genresRepository.GetAll();
            return TypedResults.Ok(genres);
        }

        static async Task<Results<Ok<Genre>, NotFound>> GetById(int id, IGenresRepository genresRepository)
        {
            var genre = await genresRepository.GetById(id);

            if (genre is null)
            {
                return TypedResults.NotFound();
            }

            return TypedResults.Ok(genre);
        }

        static async Task<Created<Genre>> Create(Genre genre, IGenresRepository genresRepository, IOutputCacheStore outputCacheStore)
        {
            await genresRepository.Create(genre);
            await outputCacheStore.EvictByTagAsync("genres-get", default);
            return TypedResults.Created($"/genres/{genre.Id}", genre);
        }

        static async Task<Results<NotFound, NoContent>> Update(int id, Genre genre, IGenresRepository repository, IOutputCacheStore outputCacheStore)
        {
            var exist = await repository.Exist(id);

            if (!exist)
            {
                return TypedResults.NotFound();
            }

            await repository.Update(genre);
            await outputCacheStore.EvictByTagAsync("genres-get", default);
            return TypedResults.NoContent();
        }

        static async Task<Results<NotFound, NoContent>> Delete(int id, IGenresRepository repository, IOutputCacheStore outputCacheStore)
        {
            var exist = await repository.Exist(id);

            if (!exist)
            {
                return TypedResults.NotFound();
            }

            await repository.Delete(id);
            await outputCacheStore.EvictByTagAsync("genres-get", default);
            return TypedResults.NoContent();
        }
    }
}
