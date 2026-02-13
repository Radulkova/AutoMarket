using AutoMarket.Data;
using AutoMarket.Models;
using AutoMarket.ViewModels;
using Microsoft.EntityFrameworkCore;

namespace AutoMarket.Services
{
    public class CarService : ICarService
    {
        private readonly ApplicationDbContext context;

        public CarService(ApplicationDbContext context)
        {
            this.context = context;
        }

        public async Task<IEnumerable<Car>> GetAllAsync()
        {
            return await context.Cars
                .Include(c => c.CarModel)
                .ThenInclude(cm => cm.Make)
                .ToListAsync();
        }

        public async Task<Car?> GetByIdAsync(int id)
        {
            return await context.Cars
                .Include(c => c.CarModel)
                .ThenInclude(cm => cm.Make)
                .FirstOrDefaultAsync(c => c.Id == id);
        }

        public async Task AddAsync(CarCreateViewModel model, string sellerId)
        {
            var exists = await context.CarModels.AnyAsync(cm => cm.Id == model.CarModelId);
            if (!exists)
                throw new ArgumentException("CarModelId is invalid or missing.");

            var car = new Car
            {
                CarModelId = model.CarModelId,
                Year = model.Year,
                Price = model.Price,
                MileageKm = model.MileageKm,
                EngineCapacityCc = model.EngineCapacityCc,
                HorsePower = model.HorsePower,
                FuelType = model.FuelType,
                Transmission = model.Transmission,
                Description = model.Description,
                ImageUrl = model.ImageUrl,
                SellerId = sellerId
            };

            context.Cars.Add(car);
            await context.SaveChangesAsync();
        }

        // ==========================
        // ✅ EDIT (GET data)
        // ==========================
        public async Task<CarEditViewModel?> GetForEditAsync(int id)
        {
            var car = await context.Cars.FirstOrDefaultAsync(c => c.Id == id);
            if (car == null) return null;

            return new CarEditViewModel
            {
                Year = car.Year,
                Price = car.Price,
                MileageKm = car.MileageKm,
                EngineCapacityCc = car.EngineCapacityCc,
                HorsePower = car.HorsePower,
                FuelType = car.FuelType,
                Transmission = car.Transmission,
                Description = car.Description,
                ImageUrl = car.ImageUrl
            };
        }

        // ==========================
        // ✅ EDIT (UPDATE)
        // ==========================
        public async Task<bool> UpdateAsync(int id, CarEditViewModel model)
        {
            var car = await context.Cars.FirstOrDefaultAsync(c => c.Id == id);
            if (car == null) return false;

            car.Year = model.Year;
            car.Price = model.Price;
            car.MileageKm = model.MileageKm;
            car.EngineCapacityCc = model.EngineCapacityCc;
            car.HorsePower = model.HorsePower;
            car.FuelType = model.FuelType;
            car.Transmission = model.Transmission;
            car.Description = model.Description;
            car.ImageUrl = model.ImageUrl;

            await context.SaveChangesAsync();
            return true;
        }

        // ==========================
        // ✅ DELETE
        // ==========================
        public async Task<bool> DeleteAsync(int id)
        {
            var car = await context.Cars.FirstOrDefaultAsync(c => c.Id == id);
            if (car == null) return false;

            context.Cars.Remove(car);
            await context.SaveChangesAsync();
            return true;
        }
    }
}
