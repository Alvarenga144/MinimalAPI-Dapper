﻿using MinimalAPIsMovies.DTOs;
using MinimalAPIsMovies.Entities;

namespace MinimalAPIsMovies.Repositories
{
    public interface IMoviesRepository
    {
        Task<int> Create(Movie movie);
        Task<List<Movie>> GetAll(PaginationDTO pagination);
        Task<Movie?> GetById(int id);
        Task<bool> Exist(int id);
        Task Update(Movie movie);
        Task Delete(int id);
    }
}