using KinoLib.Api.Models;

namespace KinoLib.Api.Services
{
    public interface IReviewService
    {
        List<Review> GetAllReviews();
        Review? GetReviewById(int id);
        Review AddReview(ReviewCreateDto reviewDto);
        Review? UpdateReview(int id, ReviewUpdateDto reviewDto);
        bool DeleteReview(int id);
        void DeleteAllReviews();
        
        List<Review> GetReviewsByMovieId(int movieId);
        double GetAverageRatingForMovie(int movieId);
    }
}
