using Moq;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using KinoLib.Api.Controllers;
using KinoLib.Api.Services;
using KinoLib.Api.Models;


namespace KinoLib.Test.Controllers
{
    public class ActorControllerTests
    {
        private readonly Mock<IActorService> _mockService;
        private readonly ActorController _controller;

        public ActorControllerTests()
        {
            _mockService = new Mock<IActorService>();
            _controller = new ActorController(_mockService.Object);
        }

        [Fact]
        public void GetActor_ReturnsOk_WhenActorExists()
        {
            var actor = new Actor { Id = 1, Name = "John Doe" };
            _mockService.Setup(s => s.GetActorById(1)).Returns(actor);

            var result = _controller.GetActor(1);

            var okResult = Assert.IsType<OkObjectResult>(result);
            Assert.Equal(actor, okResult.Value);
        }

        [Fact]
        public void GetActor_ReturnsNotFound_WhenActorDoesNotExist()
        {
            _mockService.Setup(s => s.GetActorById(999)).Returns((Actor)null);

            var result = _controller.GetActor(999);

            Assert.IsType<NotFoundResult>(result);
        }

        [Fact]
        public void CreateActor_ReturnsCreated_WhenValid()
        {
            var actorDto = new ActorCreateDto { Name = "New Actor", BirthDate = DateTime.Now.AddYears(-30) };
            var createdActor = new Actor { Id = 1, Name = "New Actor" };
            _mockService.Setup(s => s.AddActor(actorDto)).Returns(createdActor);

            var result = _controller.CreateActor(actorDto);

            var createdAtResult = Assert.IsType<CreatedAtActionResult>(result);
            Assert.Equal("GetActor", createdAtResult.ActionName);
        }

        [Fact]
        public void CreateActor_ReturnsBadRequest_WhenServiceThrowsArgumentException()
        {
            var actorDto = new ActorCreateDto { Name = "Invalid" };
            _mockService.Setup(s => s.AddActor(actorDto))
                .Throws(new ArgumentException("Invalid data"));

            var result = _controller.CreateActor(actorDto);

            Assert.IsType<BadRequestObjectResult>(result);
        }

        [Fact]
        public void DeleteActor_ReturnsNoContent_WhenSuccessful()
        {
            _mockService.Setup(s => s.DeleteActor(1)).Returns(true);

            var result = _controller.DeleteActor(1);

            Assert.IsType<NoContentResult>(result);
        }
    }
}