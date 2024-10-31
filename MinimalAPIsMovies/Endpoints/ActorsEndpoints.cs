using AutoMapper;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OutputCaching;
using MinimalAPIsMovies.DTOs;
using MinimalAPIsMovies.Entities;
using MinimalAPIsMovies.Repositories;

namespace MinimalAPIsMovies.Endpoints
{
    public static class ActorsEndpoints
    {
        public static RouteGroupBuilder MapActors(this RouteGroupBuilder builder)
        {
            builder.MapPost("/", Create).DisableAntiforgery();
            return builder;
        }

        static async Task<Created<ActorDTO>> Create([FromForm] CreateActorDTO createActorDTO, IActorsRepository repository, IOutputCacheStore outputCacheStore, IMapper mapper)
        {
            var actor = mapper.Map<Actor>(createActorDTO);
            var id = await repository.Create(actor);
            await outputCacheStore.EvictByTagAsync("actors-get", default);
            var actorDTO = mapper.Map<ActorDTO>(actor);
            return TypedResults.Created($"/actors/{id}", actorDTO);
        }
    }
}
