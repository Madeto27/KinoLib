using KinoLib.Api.Models;
using KinoLib.Api.Validators;

namespace KinoLib.Test.Validators
{
    public class MovieValidatorTests
    {
        [Theory]
        [InlineData(1889, true)]
        [InlineData(2023, true)]
        [InlineData(2028, true)]
        [InlineData(1888, false)]
        [InlineData(1800, false)]
        public void MovieCreateDtoValidator_ValidatesYear(int year, bool isValid)
        {
            var validator = new MovieCreateDtoValidator();
            var dto = new MovieCreateDto { Title = "Test", Year = year, Duration = 120, DirectorId = 1 };

            var result = validator.Validate(dto);

            Assert.Equal(isValid, result.IsValid);
        }
    }
}
