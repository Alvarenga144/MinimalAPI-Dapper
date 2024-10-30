using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.OutputCaching;
using MinimalAPIsMovies.DTOs;
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

        static async Task<Ok<List<GenreDTO>>> GetGenresList(IGenresRepository genresRepository)
        {
            var genres = await genresRepository.GetAll();
            var genresDTOs = genres
                .Select(x => new GenreDTO { Id = x.Id, Name = x.Name })
                .ToList();
            return TypedResults.Ok(genresDTOs);
        }

        static async Task<Results<Ok<GenreDTO>, NotFound>> GetById(int id, IGenresRepository genresRepository)
        {
            var genre = await genresRepository.GetById(id);

            if (genre is null)
            {
                return TypedResults.NotFound();
            }

            var genreDTO = new GenreDTO
            {
                Id = genre.Id,
                Name = genre.Name
            };

            return TypedResults.Ok(genreDTO);
        }

        static async Task<Created<GenreDTO>> Create(CreateGenreDTO createGenreDTO, IGenresRepository genresRepository, IOutputCacheStore outputCacheStore)
        {
            var genre = new Genre
            {
                Name = createGenreDTO.Name
            };

            await genresRepository.Create(genre);
            await outputCacheStore.EvictByTagAsync("genres-get", default);

            var genreDTO = new GenreDTO
            {
                Id = genre.Id,
                Name = genre.Name
            };

            return TypedResults.Created($"/genres/{genre.Id}", genreDTO);
        }

        static async Task<Results<NotFound, NoContent>> Update(int id, CreateGenreDTO createGenreDTO, IGenresRepository repository, IOutputCacheStore outputCacheStore)
        {
            var exist = await repository.Exist(id);

            if (!exist)
            {
                return TypedResults.NotFound();
            }

            var genre = new Genre
            {
                Id = id,
                Name = createGenreDTO.Name
            };

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
