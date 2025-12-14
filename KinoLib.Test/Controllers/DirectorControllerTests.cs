using Moq;
using Microsoft.AspNetCore.Mvc;
using KinoLib.Api.Controllers;
using KinoLib.Api.Services;
using KinoLib.Api.Models;

namespace KinoLib.Test.Controllers
{
    public class DirectorControllerTests
    {
        private readonly Mock<IDirectorService> _mockService;
        private readonly DirectorController _controller;

        public DirectorControllerTests()
        {
            _mockService = new Mock<IDirectorService>();
            _controller = new DirectorController(_mockService.Object);
        }

        [Fact]
        public void GetAllDirectors_ReturnsOk()
        {
            var directors = new List<Director> { new Director { Id = 1, Name = "Director 1" } };
            _mockService.Setup(s => s.GetAllDirectors()).Returns(directors);

            var result = _controller.GetAllDirectors();

            Assert.IsType<OkObjectResult>(result);
        }

        [Fact]
        public void CreateDirector_ReturnsCreated()
        {
            var dto = new DirectorCreateDto { Name = "New Director" };
            var created = new Director { Id = 1, Name = "New Director" };
            _mockService.Setup(s => s.AddDirector(dto)).Returns(created);

            var result = _controller.CreateDirector(dto);

            Assert.IsType<CreatedAtActionResult>(result);
        }

        [Fact]
        public void DeleteDirector_ReturnsNotFound_WhenUnsuccessful()
        {
            _mockService.Setup(s => s.DeleteDirector(999)).Returns(false);

            var result = _controller.DeleteDirector(999);

            Assert.IsType<NotFoundResult>(result);
        }
    }
}