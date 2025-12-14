using FluentValidation;
using KinoLib.Api.Models;

namespace KinoLib.Api.Validators
{
    public class ActorCreateDtoValidator : AbstractValidator<ActorCreateDto>
    {
        public ActorCreateDtoValidator()
        {
            RuleFor(x => x.Name)
                .NotEmpty().WithMessage("Ім'я актора є обов'язковим.")
                .MaximumLength(100).WithMessage("Ім'я не може перевищувати 100 символів.");

            RuleFor(x => x.BirthDate)
                .LessThan(DateTime.Now).WithMessage("Дата народження повинна бути у минулому.")
                .GreaterThan(new DateTime(1850, 1, 1)).WithMessage("Дата народження повинна бути після 1850 року.");

            RuleFor(x => x.MovieIds)
                .Must(ids => ids == null || ids.All(id => id > 0))
                .WithMessage("Усі ID фільмів мають бути додатніми.");
        }
    }
}