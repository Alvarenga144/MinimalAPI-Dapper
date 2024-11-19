using MinimalAPIsMovies.Utilities;

namespace MinimalAPIsMovies.DTOs
{
    public class PaginationDTO
    {
        private const int pageInitialValue = 1;
        private const int recordsPerPageInitialValue = 10;
        public int Page { get; set; } = 1;
        private int recordsPerPage = 10;
        private readonly int recordsPerPageMax = 50;

        public int RecordsPerPage
        {
            get
            {
                return recordsPerPage;
            }
            set
            {
                if (value > recordsPerPageMax)
                {
                    recordsPerPage = recordsPerPageMax;
                }
                else
                {
                    recordsPerPage = value;
                }
            }
        }

        public static ValueTask<PaginationDTO> BindAsync(HttpContext context)
        {
            var page = context.ExtractValueOrDefault(nameof(Page), pageInitialValue);
            var recordsPerpage = context.ExtractValueOrDefault(nameof(RecordsPerPage), recordsPerPageInitialValue);

            var response = new PaginationDTO
            {
                Page = page,
                RecordsPerPage = recordsPerpage,
            };

            return ValueTask.FromResult(response);
        }
    }
}
