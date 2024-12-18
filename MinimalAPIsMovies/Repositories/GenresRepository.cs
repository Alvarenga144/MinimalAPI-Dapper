﻿using Dapper;
using Microsoft.Data.SqlClient;
using MinimalAPIsMovies.Entities;
using System.Data;

namespace MinimalAPIsMovies.Repositories
{
    public class GenresRepository : IGenresRepository
    {
        private readonly string connectionString;

        public GenresRepository(IConfiguration configuration)
        {
            connectionString = configuration.GetConnectionString("DefaultConnection")!;
        }

        public async Task<int> Create(Genre genre)
        {
            using (var connection = new SqlConnection(connectionString))
            {
                var id = await connection.QuerySingleAsync<int>(@"Genres_Create", new { genre.Name }, commandType: CommandType.StoredProcedure);

                genre.Id = id;
                return id;
            }
        }

        public async Task<List<Genre>> GetAll()
        {
            using (var connection = new SqlConnection(connectionString))
            {
                var genres = await connection.QueryAsync<Genre>(@"Genres_GetAll",
                    commandType: CommandType.StoredProcedure);
                return genres.ToList();
            }
        }

        public async Task<Genre?> GetById(int id)
        {
            using (var connection = new SqlConnection(connectionString))
            {
                var genres = await connection.QueryFirstOrDefaultAsync<Genre>(@"Genres_GetById", new { id },
                    commandType: CommandType.StoredProcedure);
                return genres;
            }
        }

        public async Task<bool> Exist(int id)
        {
            using (var connection = new SqlConnection(connectionString))
            {
                var exist = await connection.QuerySingleAsync<bool>(@"Genres_Exist", new { id }, commandType: CommandType.StoredProcedure);
                return exist;
            }
        }

        public async Task<bool> Exists(int id, string name)
        {
            using (var connection = new SqlConnection(connectionString))
            {
                var exists = await connection.QuerySingleAsync<bool>(@"Genres_ExistsByIdAndName",
                    new { id, name }, commandType: CommandType.StoredProcedure);
                return exists;
            }
        }

        public async Task<List<int>> Exist(List<int> ids)
        {
            var dt = new DataTable();
            dt.Columns.Add("Id", typeof(int));

            foreach (var genreId in ids)
            {
                dt.Rows.Add(genreId);
            }

            using (var connection = new SqlConnection(connectionString))
            {
                var idsOfGenresThatExist = await connection.QueryAsync<int>("Genres_GetBySeveralIds", new { genresIds = dt }, commandType: CommandType.StoredProcedure);

                return idsOfGenresThatExist.ToList();
            }
        }


        public async Task Update(Genre genre)
        {
            using (var connection = new SqlConnection(connectionString))
            {
                await connection.ExecuteAsync(@"Genres_Update", new { genre.Id, genre.Name }, commandType: CommandType.StoredProcedure);
            }
        }

        public async Task Delete(int id)
        {
            using (var connection = new SqlConnection(connectionString))
            {
                await connection.ExecuteAsync(@"Genres_Delete", new { id }, commandType: CommandType.StoredProcedure);
            }
        }
    }
}
