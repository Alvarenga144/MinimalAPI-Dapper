namespace MinimalAPIsMovies.DTOs
{
    public class MovieDTO
    {
        public int Id { get; set; }
        public string Title { get; set; } = null!;
        public bool InTheaters { get; set; }
        public DateTime ReleaseDate { get; set; }
        public string? Poster { get; set; }
    }
}
