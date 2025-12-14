using FluentValidation;
using KinoLib.Api.Models;

namespace KinoLib.Api.Validators
{
    public class ReviewCreateDtoValidator : AbstractValidator<ReviewCreateDto>
    {
        public ReviewCreateDtoValidator()
        {
            RuleFor(x => x.Author)
                .NotEmpty().WithMessage("Ім'я автора є обов'язковим.")
                .MaximumLength(100).WithMessage("Ім'я автора не може перевищувати 100 символів.")
                .Matches("^[a-zA-Zа-яА-ЯёЁіІїЇєЄ0-9\\s.,'-]+$")
                .WithMessage("Ім'я автора містить неприпустимі символи.");

            RuleFor(x => x.Content)
                .NotEmpty().WithMessage("Зміст відгуку є обов'язковим.")
                .MinimumLength(10).WithMessage("Відгук має містити щонайменше 10 символів.")
                .MaximumLength(2000).WithMessage("Відгук не може перевищувати 2000 символів.");

            RuleFor(x => x.Rating)
                .InclusiveBetween(1, 10).WithMessage("Рейтинг має бути від 1 до 10.");

            RuleFor(x => x.MovieId)
                .GreaterThan(0).WithMessage("ID фільму має бути більше 0.");
        }
    }
}