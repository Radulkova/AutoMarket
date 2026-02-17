using AutoMarket.Data;
using AutoMarket.Models;
using AutoMarket.ViewModels;
using Microsoft.AspNetCore.Mvc.Rendering;
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
                .AsNoTracking()
                .Include(c => c.CarModel)
                    .ThenInclude(cm => cm.Make)
                .OrderByDescending(c => c.Id)
                .ToListAsync();
        }

        public async Task<Car?> GetByIdAsync(int id)
        {
            return await context.Cars
                .AsNoTracking()
                .Include(c => c.CarModel)
                    .ThenInclude(cm => cm.Make)
                .FirstOrDefaultAsync(c => c.Id == id);
        }

        // ==========================
        // CREATE helpers
        // ==========================
        public async Task<List<SelectListItem>> GetCarModelsForSelectAsync()
        {
            return await context.CarModels
                .AsNoTracking()
                .Include(cm => cm.Make)
                .OrderBy(cm => cm.Make.Name)
                .ThenBy(cm => cm.Name)
                .Select(cm => new SelectListItem
                {
                    Value = cm.Id.ToString(),
                    Text = $"{cm.Make.Name} {cm.Name}"
                })
                .ToListAsync();
        }

        public async Task<bool> AddAsync(CarCreateViewModel model, string sellerId)
        {
            // защита: да не се записва кола с невалиден модел
            var exists = await context.CarModels.AnyAsync(cm => cm.Id == model.CarModelId);
            if (!exists) return false;

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
            return true;
        }

        // ==========================
        // EDIT
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
        // DELETE
        // ==========================
        public async Task<bool> DeleteAsync(int id)
        {
            var car = await context.Cars.FirstOrDefaultAsync(c => c.Id == id);
            if (car == null) return false;

            context.Cars.Remove(car);
            await context.SaveChangesAsync();
            return true;
        }

        // ==========================
        // Mobile.bg style dropdowns
        // ==========================
        public async Task<List<(int id, string name)>> GetMakesAsync()
        {
            return await context.Makes
                .AsNoTracking()
                .OrderBy(m => m.Name)
                .Select(m => new ValueTuple<int, string>(m.Id, m.Name))
                .ToListAsync();
        }

        public async Task<List<(int id, string name)>> GetModelsAsync(int? makeId)
        {
            var q = context.CarModels.AsNoTracking().AsQueryable();

            if (makeId.HasValue && makeId.Value > 0)
                q = q.Where(cm => cm.MakeId == makeId.Value);

            return await q
                .OrderBy(cm => cm.Name)
                .Select(cm => new ValueTuple<int, string>(cm.Id, cm.Name))
                .ToListAsync();
        }

        // ==========================
        // SEARCH (без paging, за да няма грешки)
        // ==========================
        public async Task<(IEnumerable<Car> cars, int totalCount)> SearchAsync(CarsQueryViewModel query)
        {
            var q = context.Cars
                .AsNoTracking()
                .Include(c => c.CarModel).ThenInclude(cm => cm.Make)
                .AsQueryable();

            // Марка
            if (query.MakeId.HasValue && query.MakeId.Value > 0)
                q = q.Where(c => c.CarModel.MakeId == query.MakeId.Value);

            // Модел
            if (query.CarModelId.HasValue && query.CarModelId.Value > 0)
                q = q.Where(c => c.CarModelId == query.CarModelId.Value);

            // Гориво (ако е "Всички" или празно -> не филтрираме)
            if (!string.IsNullOrWhiteSpace(query.FuelType) && query.FuelType != "Всички")
                q = q.Where(c => c.FuelType == query.FuelType);

            // Скорости
            if (!string.IsNullOrWhiteSpace(query.Transmission) && query.Transmission != "Всички")
                q = q.Where(c => c.Transmission == query.Transmission);

            // Години
            if (query.YearFrom.HasValue)
                q = q.Where(c => c.Year >= query.YearFrom.Value);

            if (query.YearTo.HasValue)
                q = q.Where(c => c.Year <= query.YearTo.Value);

            // Цена
            if (query.PriceFrom.HasValue)
                q = q.Where(c => c.Price >= query.PriceFrom.Value);

            if (query.PriceTo.HasValue)
                q = q.Where(c => c.Price <= query.PriceTo.Value);

            // Сортиране
            q = query.Sort switch
            {
                "price_asc" => q.OrderBy(c => c.Price),
                "price_desc" => q.OrderByDescending(c => c.Price),
                "km_asc" => q.OrderBy(c => c.MileageKm),
                "year_desc" => q.OrderByDescending(c => c.Year),
                _ => q.OrderByDescending(c => c.Id) // newest
            };

            var total = await q.CountAsync();
            var cars = await q.ToListAsync();

            return (cars, total);
        }
    }
}
