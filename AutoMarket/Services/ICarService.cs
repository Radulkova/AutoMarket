using AutoMarket.Models;
using AutoMarket.ViewModels;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace AutoMarket.Services
{
    public interface ICarService
    {
        // LIST/DETAILS
        Task<IEnumerable<Car>> GetAllAsync();
        Task<Car?> GetByIdAsync(int id);

        // CREATE
        Task<List<SelectListItem>> GetCarModelsForSelectAsync();
        Task<bool> AddAsync(CarCreateViewModel model, string sellerId);

        // EDIT
        Task<CarEditViewModel?> GetForEditAsync(int id);
        Task<bool> UpdateAsync(int id, CarEditViewModel model);

        // DELETE
        Task<bool> DeleteAsync(int id);

        // ✅ Mobile.bg style search
        Task<(IEnumerable<Car> cars, int totalCount)> SearchAsync(CarsQueryViewModel query);
        Task<List<(int id, string name)>> GetMakesAsync();
        Task<List<(int id, string name)>> GetModelsAsync(int? makeId);
    }
}
