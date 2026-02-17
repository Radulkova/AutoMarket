using AutoMarket.Models;
using AutoMarket.ViewModels;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace AutoMarket.Services
{
    public interface ICarService
    {
        Task<IEnumerable<Car>> GetAllAsync();
        Task<Car?> GetByIdAsync(int id);

        Task<List<SelectListItem>> GetCarModelsForSelectAsync();
        Task<bool> AddAsync(CarCreateViewModel model, string sellerId);

        Task<CarEditViewModel?> GetForEditAsync(int id);
        Task<bool> UpdateAsync(int id, CarEditViewModel model);
        Task<bool> DeleteAsync(int id);

        Task<List<(int id, string name)>> GetMakesAsync();
        Task<List<(int id, string name)>> GetModelsAsync(int? makeId);

        Task<(IEnumerable<Car> cars, int totalCount)> SearchAsync(CarsQueryViewModel query);
    }
}
