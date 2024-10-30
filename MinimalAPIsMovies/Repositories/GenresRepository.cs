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
            connectionString = configuration.GetConnectionString("DefaultConnecion")!;
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
                                                                ", new {id});
                return genres;
            }
        }
    }
}
