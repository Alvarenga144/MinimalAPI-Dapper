﻿using MinimalAPIsMovies.Entities;

namespace MinimalAPIsMovies.Repositories
{
    public interface IGenresRepository
    {
        Task<int> Create(Genre genre);
        Task<List<Genre>> GetAll();
        Task<Genre?> GetById(int id);
        Task<bool> Exist(int id);
        Task Update(Genre genre);
        Task Delete(int id);
        Task<List<int>> Exist(List<int> ids);
        Task<bool> Exists(int id, string name);
    }
}
