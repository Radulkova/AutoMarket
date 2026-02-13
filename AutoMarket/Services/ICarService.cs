using AutoMarket.Models;
using AutoMarket.ViewModels;

namespace AutoMarket.Services
{
    public interface ICarService
    {
        Task<IEnumerable<Car>> GetAllAsync();
        Task<Car?> GetByIdAsync(int id);

        Task AddAsync(CarCreateViewModel model, string sellerId);

        // ✅ Edit/Delete
        Task<CarEditViewModel?> GetForEditAsync(int id);
        Task<bool> UpdateAsync(int id, CarEditViewModel model);
        Task<bool> DeleteAsync(int id);
    }
}

