using Dapper;
using Microsoft.Data.SqlClient;
using MinimalAPIsMovies.Entities;

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
                var query = @"
                            INSERT INTO Genres (Name)
                            VALUES (@Name);
                            Select SCOPE_IDENTITY()
                            ";

                var id = await connection.QuerySingleAsync<int>(query, genre);
                genre.Id = id;
                return id;
            }

        }

        public async Task Delete(int id)
        {
            using (var connection = new SqlConnection(connectionString))
            {
                await connection.ExecuteAsync(@"DELETE Genres WHERE Id = @Id", new { id });
            }
        }

        public async Task<bool> Exist(int id)
        {
            using (var connection = new SqlConnection(connectionString))
            {
                var exist = await connection.QuerySingleAsync<bool>(@"
                                                                    IF EXISTS (SELECT 1 FROM Genres WHERE Id = @id)
	                                                                    SELECT 1;
                                                                    ELSE
	                                                                    SELECT 0;
                                                                    ", new { id });
                return exist;
            }
        }

        public async Task<List<Genre>> GetAll()
        {
            using (var connection = new SqlConnection(connectionString))
            {
                var genres = await connection.QueryAsync<Genre>(@"
                                                                SELECT Id, Name FROM Genres
                                                                ORDER BY Name
                                                                ");
                return genres.ToList();
            }
        }

        public async Task<Genre?> GetById(int id)
        {
            using (var connection = new SqlConnection(connectionString))
            {
                var genres = await connection.QueryFirstOrDefaultAsync<Genre>(@"
                                                                SELECT Id, Name 
                                                                FROM Genres
                                                                WHERE Id = @Id
                                                                ", new { id });
                return genres;
            }
        }

        public async Task Update(Genre genre)
        {
            using (var connection = new SqlConnection(connectionString))
            {
                await connection.ExecuteAsync(@"
                                            UPDATE Genres
                                            SET Name = @Name
                                            WHERE Id = @Id
                                            ", genre);
            }
        }
    }
}
