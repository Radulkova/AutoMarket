using AutoMarket.Data;
using AutoMarket.Models;
using Microsoft.EntityFrameworkCore;

namespace AutoMarket.Services
{
    public class FavoriteService : IFavoriteService
    {
        private readonly ApplicationDbContext context;

        public FavoriteService(ApplicationDbContext context)
        {
            this.context = context;
        }

        public async Task<bool> CarExistsAsync(int carId)
        {
            return await context.Cars
                .AnyAsync(c => c.Id == carId && !c.IsDeleted);
        }

        public async Task<bool> ToggleAsync(int carId, string userId)
        {
            var favorite = await context.Favorites
                .FirstOrDefaultAsync(f => f.CarId == carId && f.UserId == userId);

            if (favorite != null)
            {
                context.Favorites.Remove(favorite);
                await context.SaveChangesAsync();
                return false;
            }

            var newFavorite = new Favorite
            {
                CarId = carId,
                UserId = userId,
                CreatedOn = DateTime.UtcNow
            };

            context.Favorites.Add(newFavorite);
            await context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> IsFavoriteAsync(int carId, string userId)
        {
            return await context.Favorites
                .AnyAsync(f => f.CarId == carId && f.UserId == userId);
        }

        public async Task<HashSet<int>> GetFavoriteCarIdsAsync(string userId)
        {
            var ids = await context.Favorites
                .AsNoTracking()
                .Where(f => f.UserId == userId)
                .Select(f => f.CarId)
                .ToListAsync();

            return ids.ToHashSet();
        }

        public async Task<List<Car>> GetFavoriteCarsAsync(string userId)
        {
            var favoriteCarIds = await context.Favorites
                .AsNoTracking()
                .Where(f => f.UserId == userId)
                .OrderByDescending(f => f.CreatedOn)
                .Select(f => f.CarId)
                .ToListAsync();

            if (!favoriteCarIds.Any())
            {
                return new List<Car>();
            }

            var cars = await context.Cars
                .AsNoTracking()
                .Where(c => favoriteCarIds.Contains(c.Id) && !c.IsDeleted)
                .Include(c => c.Images)
                .Include(c => c.Seller)
                .Include(c => c.CarModel)
                    .ThenInclude(cm => cm.Make)
                .ToListAsync();

            return cars
                .OrderBy(c => favoriteCarIds.IndexOf(c.Id))
                .ToList();
        }
    }
}