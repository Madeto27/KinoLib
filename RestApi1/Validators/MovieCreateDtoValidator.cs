using FluentValidation;
using KinoLib.Api.Models;

namespace KinoLib.Api.Validators
{
    public class MovieCreateDtoValidator : AbstractValidator<MovieCreateDto>
    {
        public MovieCreateDtoValidator()
        {
            RuleFor(x => x.Title)
                .NotEmpty().WithMessage("Назва фільму обов'язкова.");

            RuleFor(x => x.Year)
                .GreaterThan(1888).WithMessage("Рік має бути більше 1888.")
                .LessThanOrEqualTo(DateTime.Now.Year + 5);

            RuleFor(x => x.Duration)
                .GreaterThan(0).WithMessage("Тривалість має бути більше 0.");
            
            RuleFor(x => x.DirectorId)
                .GreaterThan(0).WithMessage("DirectorId має бути більше 0.");
        }
    }
}
