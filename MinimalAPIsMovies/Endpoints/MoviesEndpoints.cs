using AutoMapper;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OutputCaching;
using MinimalAPIsMovies.DTOs;
using MinimalAPIsMovies.Entities;
using MinimalAPIsMovies.Repositories;
using MinimalAPIsMovies.Services;

namespace MinimalAPIsMovies.Endpoints
{
    public static class MoviesEndpoints
    {
        private readonly static string container = "movies";

        public static RouteGroupBuilder MapMovies(this RouteGroupBuilder builder)
        {
            //builder.MapGet("/", GetAll).CacheOutput(c => c.Expire(TimeSpan.FromMinutes(1)).Tag("movies-get"));
            //builder.MapGet("/{id:int}", GetById);
            //builder.MapGet("getByName/{name}", GetByName);
            builder.MapPost("/", Create).DisableAntiforgery();
            //builder.MapPut("/{id:int}", Update).DisableAntiforgery();
            //builder.MapDelete("/{id:int}", Delete);
            return builder;
        }

        static async Task<Created<MovieDTO>> Create([FromForm] CreateMovieDTO createMovieDTO, IMoviesRepository repository, IOutputCacheStore outputCacheStore, IMapper mapper, IFileStorage fileStorage)
        {
            var movie = mapper.Map<Movie>(createMovieDTO);

            if (createMovieDTO.Poster is not null)
            {
                var url = await fileStorage.Store(container, createMovieDTO.Poster);
                movie.Poster = url;
            }

            var id = await repository.Create(movie);
            await outputCacheStore.EvictByTagAsync("movies-get", default);
            var moviesDTO = mapper.Map<MovieDTO>(movie);
            return TypedResults.Created($"/movies/{id}", moviesDTO);
        }
    }
}
