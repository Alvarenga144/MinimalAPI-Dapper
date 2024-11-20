using Microsoft.OpenApi.Any;
using MinimalAPIsMovies.DTOs;

namespace MinimalAPIsMovies.Utilities
{
    public static class SwaggerExtensions
    {
        public static TBuilder AddPaginationParameters<TBuilder>(this TBuilder builder) where TBuilder : IEndpointConventionBuilder
        {
            return builder.WithOpenApi(options =>
            {
                options.Parameters.Add(new Microsoft.OpenApi.Models.OpenApiParameter
                {
                    Name = "Page",
                    In = Microsoft.OpenApi.Models.ParameterLocation.Query,
                    Schema = new Microsoft.OpenApi.Models.OpenApiSchema
                    {
                        Type = "integer",
                        Default = new OpenApiInteger(PaginationDTO.pageInitialValue)
                    }
                });

                options.Parameters.Add(new Microsoft.OpenApi.Models.OpenApiParameter
                {
                    Name = "RecordsPerPage",
                    In = Microsoft.OpenApi.Models.ParameterLocation.Query,
                    Schema = new Microsoft.OpenApi.Models.OpenApiSchema
                    {
                        Type = "integer",
                        Default = new OpenApiInteger(PaginationDTO.recordsPerPageInitialValue)
                    }
                });

                return options;
            });
        }
    }
}
