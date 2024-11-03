﻿using AutoMapper;
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
            builder.MapGet("/", GetAll).CacheOutput(c => c.Expire(TimeSpan.FromMinutes(1)).Tag("movies-get"));
            builder.MapGet("/{id:int}", GetById);
            //builder.MapGet("getByName/{name}", GetByName);
            builder.MapPost("/", Create).DisableAntiforgery();
            builder.MapPut("/{id:int}", Update).DisableAntiforgery();
            //builder.MapDelete("/{id:int}", Delete);
            return builder;
        }

        static async Task<Ok<List<MovieDTO>>> GetAll(IMoviesRepository repository, IMapper mapper, int page = 1, int recordsPerPage = 10)
        {
            var pagination = new PaginationDTO { Page = page, RecordsPerPage = recordsPerPage };
            var movies = await repository.GetAll(pagination);
            var moviesDTO = mapper.Map<List<MovieDTO>>(movies);
            return TypedResults.Ok(moviesDTO);
        }

        static async Task<Results<Ok<MovieDTO>, NotFound>> GetById(int id, IMoviesRepository repository, IMapper mapper)
        {
            var movie = await repository.GetById(id);

            if (movie is null)
            {
                return TypedResults.NotFound();
            }

            var movieDTO = mapper.Map<MovieDTO>(movie);
            return TypedResults.Ok(movieDTO);
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

        static async Task<Results<NoContent, NotFound>> Update(int id, [FromForm] CreateMovieDTO createMovieDTO, IMoviesRepository repository, IFileStorage fileStorage, IOutputCacheStore outputCacheStore, IMapper mapper)
        {
            var movieDb = await repository.GetById(id);

            if (movieDb is null)
            {
                return TypedResults.NotFound();
            }

            var movieForUpdate = mapper.Map<Movie>(createMovieDTO);
            movieForUpdate.Id = id;
            movieForUpdate.Poster = movieDb.Poster;

            if (createMovieDTO.Poster is not null)
            {
                var url = await fileStorage.Edit(movieForUpdate.Poster, container, createMovieDTO.Poster);
                movieForUpdate.Poster = url;
            }

            await repository.Update(movieForUpdate);
            await outputCacheStore.EvictByTagAsync("movies-get", default);
            return TypedResults.NoContent();
        }
    }
}
