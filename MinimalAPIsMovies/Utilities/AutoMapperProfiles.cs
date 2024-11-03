using AutoMapper;
using MinimalAPIsMovies.DTOs;
using MinimalAPIsMovies.Entities;

namespace MinimalAPIsMovies.Utilities
{
    public class AutoMapperProfiles : Profile
    {
        public AutoMapperProfiles()
        {
            CreateMap<Genre, GenreDTO>();
            CreateMap<CreateGenreDTO, Genre>();

            CreateMap<Actor, ActorDTO>();
            CreateMap<CreateActorDTO, Actor>()
                .ForMember(p => p.Picture, options => options.Ignore());

            CreateMap<Movie, MovieDTO>();
            CreateMap<CreateMovieDTO, Movie>()
                .ForMember(p => p.Poster, options => options.Ignore());

            CreateMap<Comment, CommentDTO>();
            CreateMap<CreateCommentDTO, Comment>();
        }
    }
}
