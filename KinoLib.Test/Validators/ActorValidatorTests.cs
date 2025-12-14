using KinoLib.Api.Models;
using KinoLib.Api.Validators;

namespace KinoLib.Test.Validators
{
    public class ActorValidatorTests
    {
        [Fact]
        public void ActorCreateDtoValidator_ValidatesMovieIds()
        {
            var validator = new ActorCreateDtoValidator();
            var dto = new ActorCreateDto
            {
                Name = "Test",
                BirthDate = DateTime.Now.AddYears(-30),
                MovieIds = new List<int> { 1, 0, -1 }
            };

            var result = validator.Validate(dto);

            Assert.False(result.IsValid);
        }
    }
}
