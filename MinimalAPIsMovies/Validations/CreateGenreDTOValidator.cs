using FluentValidation;
using MinimalAPIsMovies.DTOs;
using MinimalAPIsMovies.Repositories;

namespace MinimalAPIsMovies.Validations
{
    public class CreateGenreDTOValidator : AbstractValidator<CreateGenreDTO>
    {
        public CreateGenreDTOValidator(IGenresRepository genresRepository, IHttpContextAccessor httpContextAccessor)
        {
            var routeValueId = httpContextAccessor.HttpContext!.Request.RouteValues["id"];
            var id = 0;

            if (routeValueId is string routeValueIdString)
            {
                int.TryParse(routeValueIdString, out id);
            }

            RuleFor(p => p.Name)
                .NotEmpty().WithMessage("The field {PropertyName} is required")
                .MaximumLength(150).WithMessage("The field {PropertyName} should be less than {MaxLength} characters")
                .Must(FirsLetterIsUppercase).WithMessage("the field {PropertyName} should start with uppercase")
                .MustAsync(async (name, _) =>
                {
                    var exists = await genresRepository.Exists(id, name);
                    return !exists;
                }).WithMessage(g => $"A genre with the name {g.Name} already exists");
        }

        private bool FirsLetterIsUppercase(string value)
        {
            if (string.IsNullOrWhiteSpace(value))
            {
                return true;
            }

            var firstLetter = value[0].ToString();
            return firstLetter == firstLetter.ToUpper();
        }
    }
}
