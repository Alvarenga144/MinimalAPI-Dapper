﻿using Dapper;
using Microsoft.Data.SqlClient;
using MinimalAPIsMovies.Entities;
using System.Data;

namespace MinimalAPIsMovies.Repositories
{
    public class CommentsRepository : ICommentsRepository
    {
        private readonly string connectionString;

        public CommentsRepository(IConfiguration configuration)
        {
            connectionString = configuration.GetConnectionString("DefaultConnection")!;
        }

        public async Task<int> Create(Comment comment)
        {
            using (var connection = new SqlConnection(connectionString))
            {
                var id = await connection.QuerySingleAsync<int>(@"Comments_Create", new { comment.Body, comment.MovieId, comment.UserId }, commandType: CommandType.StoredProcedure);

                comment.Id = id;
                return id;
            }
        }

        public async Task<List<Comment>> GetAll(int movieId)
        {
            using (var connection = new SqlConnection(connectionString))
            {
                var comments = await connection.QueryAsync<Comment>(@"Comments_GetAllByMovieId", new { movieId }, commandType: CommandType.StoredProcedure);
                return comments.ToList();
            }
        }

        public async Task<Comment?> GetById(int id)
        {
            using (var connection = new SqlConnection(connectionString))
            {
                var comments = await connection.QueryFirstOrDefaultAsync<Comment>(@"Comments_GetById", new { id }, commandType: CommandType.StoredProcedure);
                return comments;
            }
        }

        public async Task<bool> Exist(int id)
        {
            using (var connection = new SqlConnection(connectionString))
            {
                var exist = await connection.QuerySingleAsync<bool>(@"Comments_Exist", new { id }, commandType: CommandType.StoredProcedure);
                return exist;
            }
        }

        public async Task Update(Comment comment)
        {
            using (var connection = new SqlConnection(connectionString))
            {
                await connection.ExecuteAsync(@"Comments_Update", new
                {
                    comment.Id,
                    comment.Body,
                    comment.MovieId,
                }, commandType: CommandType.StoredProcedure);
            }
        }

        public async Task Delete(int id)
        {
            using (var connection = new SqlConnection(connectionString))
            {
                await connection.ExecuteAsync(@"Comments_Delete", new { id }, commandType: CommandType.StoredProcedure);
            }
        }
    }
}
