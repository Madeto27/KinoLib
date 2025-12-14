using Microsoft.AspNetCore.Mvc;
using KinoLib.Api.Models;
using KinoLib.Api.Services;

namespace KinoLib.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ReviewController : ControllerBase
    {
        private readonly IReviewService _reviewService;

        public ReviewController(IReviewService reviewService)
        {
            _reviewService = reviewService;
        }

        [HttpGet]
        public IActionResult GetAllReviews()
        {
            var reviews = _reviewService.GetAllReviews();
            return Ok(reviews);
        }

        [HttpGet("{id}")]
        public IActionResult GetReview(int id)
        {
            var review = _reviewService.GetReviewById(id);
            if (review == null) return NotFound();
            return Ok(review);
        }

        [HttpPost]
        public IActionResult CreateReview([FromBody] ReviewCreateDto reviewDto)
        {
            try
            {
                var review = _reviewService.AddReview(reviewDto);
                return CreatedAtAction(nameof(GetReview), new { id = review.Id }, review);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPut("{id}")]
        public IActionResult UpdateReview(int id, [FromBody] ReviewUpdateDto reviewDto)
        {
            var updatedReview = _reviewService.UpdateReview(id, reviewDto);
            if (updatedReview == null) return NotFound();
            return Ok(updatedReview);
        }

        [HttpDelete("{id}")]
        public IActionResult DeleteReview(int id)
        {
            var result = _reviewService.DeleteReview(id);
            if (!result) return NotFound();
            return NoContent();
        }

        [HttpDelete]
        public IActionResult DeleteAllReviews()
        {
            _reviewService.DeleteAllReviews();
            return NoContent();
        }

        [HttpGet("movie/{movieId}")]
        public IActionResult GetReviewsForMovie(int movieId)
        {
            var reviews = _reviewService.GetReviewsByMovieId(movieId);
            return Ok(reviews);
        }

        [HttpGet("movie/{movieId}/average")]
        public IActionResult GetAverageRating(int movieId)
        {
            var average = _reviewService.GetAverageRatingForMovie(movieId);
            return Ok(new { MovieId = movieId, AverageRating = average });
        }
    }
}