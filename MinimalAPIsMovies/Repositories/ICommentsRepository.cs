﻿using MinimalAPIsMovies.Entities;

namespace MinimalAPIsMovies.Repositories
{
    public interface ICommentsRepository
    {
        Task<int> Create(Comment comment);
        Task Delete(int id);
        Task<bool> Exist(int id);
        Task<List<Comment>> GetAll();
        Task<Comment?> GetById(int id);
        Task Update(Comment comment);
    }
}