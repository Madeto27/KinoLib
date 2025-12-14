using FluentValidation;
using KinoLib.Api.Models;

namespace KinoLib.Api.Validators
{
    public class DirectorCreateDtoValidator : AbstractValidator<DirectorCreateDto>
    {
        public DirectorCreateDtoValidator()
        {
            RuleFor(x => x.Name)
                .NotEmpty().WithMessage("Ім'я режисера є обов'язковим.")
                .MaximumLength(100).WithMessage("Ім'я не може перевищувати 100 символів.");

            RuleFor(x => x.Nationality)
                .NotEmpty().WithMessage("Національність є обов'язковою.");

            RuleFor(x => x.BirthDate)
                .LessThan(DateTime.Now).WithMessage("Дата народження повинна бути у минулому.");
        }
    }
}
