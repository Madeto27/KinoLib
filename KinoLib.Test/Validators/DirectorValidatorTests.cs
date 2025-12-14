using Xunit;
using FluentValidation.TestHelper;
using KinoLib.Api.Validators;
using KinoLib.Api.Models;
using System;

namespace KinoLib.Test.Validators
{
    public class DirectorCreateDtoValidatorTests
    {
        private readonly DirectorCreateDtoValidator _validator;

        public DirectorCreateDtoValidatorTests()
        {
            _validator = new DirectorCreateDtoValidator();
        }

        [Fact]
        public void Validate_BirthDate_WhenFutureDate_ReturnsError()
        {
            var dto = new DirectorCreateDto
            {
                Name = "Test Director",
                BirthDate = DateTime.Now.AddDays(1),
                Nationality = "Ukrainian"
            };

            var result = _validator.TestValidate(dto);

            result.ShouldHaveValidationErrorFor(x => x.BirthDate)
                  .WithErrorMessage("Дата народження повинна бути у минулому.");
        }

        [Fact]
        public void Validate_BirthDate_WhenPastDate_IsValid()
        {
            var dto = new DirectorCreateDto
            {
                Name = "Test Director",
                BirthDate = DateTime.Now.AddYears(-30),
                Nationality = "Ukrainian"
            };

            var result = _validator.TestValidate(dto);

            result.ShouldNotHaveValidationErrorFor(x => x.BirthDate);
        }

        [Fact]
        public void Validate_Name_WhenEmpty_ReturnsError()
        {
            var dto = new DirectorCreateDto
            {
                Name = "",
                BirthDate = DateTime.Now.AddYears(-30),
                Nationality = "Ukrainian"
            };

            var result = _validator.TestValidate(dto);

            result.ShouldHaveValidationErrorFor(x => x.Name)
                  .WithErrorMessage("Ім'я режисера є обов'язковим.");
        }

        [Fact]
        public void Validate_Nationality_WhenEmpty_ReturnsError()
        {
            var dto = new DirectorCreateDto
            {
                Name = "Test Director",
                BirthDate = DateTime.Now.AddYears(-30),
                Nationality = ""
            };

            var result = _validator.TestValidate(dto);

            result.ShouldHaveValidationErrorFor(x => x.Nationality)
                  .WithErrorMessage("Національність є обов'язковою.");
        }
    }
}