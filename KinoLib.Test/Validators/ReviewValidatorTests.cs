using KinoLib.Api.Models;
using KinoLib.Api.Validators;

namespace KinoLib.Test.Validators
{
    public class ReviewValidatorTests
    {
        [Theory]
        [InlineData(1, true)]
        [InlineData(5, true)]
        [InlineData(10, true)]
        [InlineData(0, false)]
        [InlineData(11, false)]
        public void ReviewCreateDtoValidator_ValidatesRating(int rating, bool isValid)
        {
            var validator = new ReviewCreateDtoValidator();
            var dto = new ReviewCreateDto
            {
                Author = "Test",
                Content = "Valid content",
                Rating = rating,
                MovieId = 1
            };

            var result = validator.Validate(dto);

            Assert.Equal(isValid, result.IsValid);
        }

        [Fact]
        public void ReviewCreateDtoValidator_ValidatesContentLength()
        {
            var validator = new ReviewCreateDtoValidator();
            var dto = new ReviewCreateDto
            {
                Author = "Test",
                Content = "Short",
                Rating = 5,
                MovieId = 1
            };

            var result = validator.Validate(dto);

            Assert.False(result.IsValid);
            Assert.Contains("має містити щонайменше 10 символів", result.Errors[0].ErrorMessage);
        }
    }
}
