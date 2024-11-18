using AutoMapper;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.OutputCaching;
using MinimalAPIsMovies.DTOs;
using MinimalAPIsMovies.Entities;
using MinimalAPIsMovies.Filters;
using MinimalAPIsMovies.Repositories;

namespace MinimalAPIsMovies.Endpoints
{
    public static class GenresEndpoints
    {
        public static RouteGroupBuilder MapGenres(this RouteGroupBuilder group)
        {
            group.MapGet("/", GetGenresList)
                .CacheOutput(c => c.Expire(TimeSpan.FromSeconds(60)).Tag("genres-get"));
                //.RequireAuthorization();
            group.MapGet("/{id:int}", GetById);

            group.MapPost("/", Create)
                .AddEndpointFilter<ValidationFilter<CreateGenreDTO>>()
                .RequireAuthorization("isadmin");
            group.MapPut("/{id:int}", Update)
                .AddEndpointFilter<ValidationFilter<CreateGenreDTO>>()
                .RequireAuthorization("isadmin");
            group.MapDelete("/{id:int}", Delete).RequireAuthorization("isadmin");
            return group;
        }

        static async Task<Ok<List<GenreDTO>>> GetGenresList(IGenresRepository genresRepository, IMapper mapper, ILoggerFactory loggerFactory)
        {
            var type = typeof(GenresEndpoints);
            var logger = loggerFactory.CreateLogger(type.FullName!);

            logger.LogTrace("This is a trace message");
            logger.LogDebug("This is a debug message");
            logger.LogInformation("This is a information message");
            logger.LogWarning("This is a warning message");
            logger.LogError("This is a error message");
            logger.LogCritical("This is a critical message");

            var genres = await genresRepository.GetAll();
            var genresDTOs = mapper.Map<List<GenreDTO>>(genres);
            return TypedResults.Ok(genresDTOs);
        }

        static async Task<Results<Ok<GenreDTO>, NotFound>> GetById(int id, IGenresRepository genresRepository, IMapper mapper)
        {
            var genre = await genresRepository.GetById(id);

            if (genre is null)
            {
                return TypedResults.NotFound();
            }

            var genreDTO = mapper.Map<GenreDTO>(genre);

            return TypedResults.Ok(genreDTO);
        }

        static async Task<Created<GenreDTO>> Create(CreateGenreDTO createGenreDTO, IGenresRepository genresRepository, IOutputCacheStore outputCacheStore, IMapper mapper)
        {
            var genre = mapper.Map<Genre>(createGenreDTO); ;

            await genresRepository.Create(genre);
            await outputCacheStore.EvictByTagAsync("genres-get", default);

            var genreDTO = mapper.Map<GenreDTO>(genre);

            return TypedResults.Created($"/genres/{genre.Id}", genreDTO);
        }

        static async Task<Results<NotFound, NoContent>> Update(int id, CreateGenreDTO createGenreDTO, IGenresRepository repository, IOutputCacheStore outputCacheStore, IMapper mapper)
        {
            var exist = await repository.Exist(id);

            if (!exist)
            {
                return TypedResults.NotFound();
            }

            var genre = mapper.Map<Genre>(createGenreDTO);
            genre.Id = id;

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
