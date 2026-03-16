using AutoMarket.Models;

namespace AutoMarket.Services
{
    public interface IFavoriteService
    {
        Task<bool> CarExistsAsync(int carId);
        Task<bool> ToggleAsync(int carId, string userId);
        Task<bool> IsFavoriteAsync(int carId, string userId);
        Task<HashSet<int>> GetFavoriteCarIdsAsync(string userId);
        Task<List<Car>> GetFavoriteCarsAsync(string userId);
    }
}