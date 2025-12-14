using KinoLib.Api.Models;

namespace KinoLib.Api.Repositories
{
    public interface IReviewRepository
    {
        List<Review> GetAll();
        Review? GetById(int id);
        void Add(Review review);
        void Update(Review review);
        void Delete(Review review);
        void SaveChanges();
    }
}
