using Dapper;
using Microsoft.Data.SqlClient;
using MinimalAPIsMovies.DTOs;
using MinimalAPIsMovies.Entities;
using System.Data;

namespace MinimalAPIsMovies.Repositories
{
    public class MoviesRepository : IMoviesRepository
    {
        private readonly string connectionString;
        private readonly HttpContext httpContext;

        public MoviesRepository(IConfiguration configuration, IHttpContextAccessor httpContextAccessor)
        {
            connectionString = configuration.GetConnectionString("DefaultConnection")!;
            httpContext = httpContextAccessor.HttpContext!;
        }

        public async Task<int> Create(Movie movie)
        {
            using (var connection = new SqlConnection(connectionString))
            {
                var id = await connection.QuerySingleAsync<int>("Movies_Create", new { movie.Title, movie.Poster, movie.ReleaseDate, movie.InTheaters }, commandType: CommandType.StoredProcedure);

                movie.Id = id;
                return id;
            }
        }

        public async Task<List<Movie>> GetAll(PaginationDTO pagination)
        {
            using (var connection = new SqlConnection(connectionString))
            {
                var movies = await connection.QueryAsync<Movie>("Movies_GetAll",
                    new { pagination.Page, pagination.RecordsPerPage }
                    , commandType: CommandType.StoredProcedure);

                var MoviesCount = await connection.QuerySingleAsync<int>("Movies_Count", commandType: CommandType.StoredProcedure);

                httpContext.Response.Headers.Append("TotalAmountOfRecords", MoviesCount.ToString());

                return movies.ToList();
            }
        }

        public async Task<Movie?> GetById(int id)
        {
            using (var connection = new SqlConnection(connectionString))
            {
                using (var multi = await connection.QueryMultipleAsync("Movie_GetById", new { id }))
                {
                    var movie = await multi.ReadFirstAsync<Movie>();
                    var comments = await multi.ReadAsync<Comment>();

                    movie.Comments = comments.ToList();
                    return movie;
                }
            }
        }

        public async Task<bool> Exist(int id)
        {
            using (var connection = new SqlConnection(connectionString))
            {
                var exist = await connection.QuerySingleAsync<bool>("Movies_Exist", new { id }, commandType: CommandType.StoredProcedure);
                return exist;
            }
        }

        public async Task Update(Movie movie)
        {
            using (var connection = new SqlConnection(connectionString))
            {
                await connection.ExecuteAsync("Movies_Update", new
                {
                    movie.Id,
                    movie.Title,
                    movie.InTheaters,
                    movie.Poster,
                    movie.ReleaseDate
                },
                commandType: CommandType.StoredProcedure);
            }
        }

        public async Task Delete(int id)
        {
            using (var connection = new SqlConnection(connectionString))
            {
                await connection.ExecuteAsync("Movies_Delete", new { id }, commandType: CommandType.StoredProcedure);
            }
        }

        public async Task Assign(int id, List<int> genresIds)
        {
            var dt = new DataTable();
            dt.Columns.Add("Id", typeof(int));

            foreach (var genreId in genresIds)
            {
                dt.Rows.Add(genreId);
            }

            using (var connection = new SqlConnection(connectionString))
            {
                await connection.ExecuteAsync("Movies_AssignGenres", new { moviesId = id, genresIds = dt }, commandType: CommandType.StoredProcedure);
            }
        }

        public async Task Assign(int id, List<ActoreMovie> actors)
        {
            for (int i = 1; i <= actors.Count; i++)
            {
                actors[i - i].Order = i;
            }

            var dt = new DataTable();
            dt.Columns.Add("Id", typeof(int));
            dt.Columns.Add("Character", typeof(string));
            dt.Columns.Add("Order", typeof(int));

            foreach (var actorMovie in actors)
            {
                dt.Rows.Add(actorMovie.ActorId, actorMovie.Character, actorMovie.Order);
            }

            using (var connection = new SqlConnection(connectionString))
            {
                await connection.ExecuteAsync("Movies_AssignActors", new { movieId = id, actors = dt }, commandType: CommandType.StoredProcedure);
            }
        }
    }
}
