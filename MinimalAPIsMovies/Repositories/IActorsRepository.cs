using MinimalAPIsMovies.DTOs;
using MinimalAPIsMovies.Entities;

namespace MinimalAPIsMovies.Repositories
{
    public interface IActorsRepository
    {
        Task<int> Create(Actor actor);
        Task<List<Actor>> GetAll(PaginationDTO pagination);
        Task<Actor?> GetById(int id);
        Task<bool> Exist(int id);
        Task Update(Actor actor);
        Task Delete(int id);
        Task<List<Actor>> GetByName(string name);
    }
}