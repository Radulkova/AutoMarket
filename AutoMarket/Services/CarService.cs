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
                .Include(c => c.Images) // ✅ Variant B
                .Include(c => c.CarModel)
                    .ThenInclude(cm => cm.Make)
                .OrderByDescending(c => c.Id)
                .ToListAsync();
        }

        public async Task<Car?> GetByIdAsync(int id)
        {
            return await context.Cars
                .AsNoTracking()
                .Include(c => c.Images) // ✅ Variant B (за галерия/главна снимка)
                .Include(c => c.CarModel)
                    .ThenInclude(cm => cm.Make)
                .FirstOrDefaultAsync(c => c.Id == id);
        }

        // --------------------------
        // CREATE helpers
        // --------------------------
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
            var exists = await context.CarModels.AnyAsync(cm => cm.Id == model.CarModelId);
            if (!exists) return false;

            var imageUrl = (model.ImageUrl ?? "").Trim();

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
                Description = model.Description ?? "",
                SellerId = sellerId,

                // оставяме го за съвместимост (ако някъде още го ползваш)
                ImageUrl = imageUrl
            };

            // ✅ Variant B: ако има URL, правим го главна снимка в CarImages
            if (!string.IsNullOrWhiteSpace(imageUrl))
            {
                car.Images.Add(new CarImage
                {
                    Url = imageUrl,
                    IsMain = true,
                    SortOrder = 0
                });
            }

            context.Cars.Add(car);
            await context.SaveChangesAsync();
            return true;
        }

        // --------------------------
        // EDIT
        // --------------------------
        public async Task<CarEditViewModel?> GetForEditAsync(int id)
        {
            var car = await context.Cars
                .AsNoTracking()
                .Include(c => c.Images) // ✅ Variant B
                .FirstOrDefaultAsync(c => c.Id == id);

            if (car == null) return null;

            // За edit полето ImageUrl ще показваме главната снимка от CarImages (ако има),
            // иначе fallback към старото car.ImageUrl
            var mainImg =
                car.Images?
                    .OrderByDescending(i => i.IsMain)
                    .ThenBy(i => i.SortOrder)
                    .FirstOrDefault()?.Url
                ?? car.ImageUrl;

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
                ImageUrl = mainImg
            };
        }

        public async Task<bool> UpdateAsync(int id, CarEditViewModel model)
        {
            var car = await context.Cars
                .Include(c => c.Images) // ✅ Variant B (трябва за update на снимката)
                .FirstOrDefaultAsync(c => c.Id == id);

            if (car == null) return false;

            var imageUrl = (model.ImageUrl ?? "").Trim();

            car.Year = model.Year;
            car.Price = model.Price;
            car.MileageKm = model.MileageKm;
            car.EngineCapacityCc = model.EngineCapacityCc;
            car.HorsePower = model.HorsePower;
            car.FuelType = model.FuelType;
            car.Transmission = model.Transmission;
            car.Description = model.Description ?? "";

            // оставяме го за съвместимост
            car.ImageUrl = imageUrl;

            // ✅ Variant B: обновяваме главната снимка
            if (!string.IsNullOrWhiteSpace(imageUrl))
            {
                var main = car.Images
                    .OrderByDescending(i => i.IsMain)
                    .ThenBy(i => i.SortOrder)
                    .FirstOrDefault(i => i.IsMain);

                if (main == null)
                {
                    // няма главна - създаваме
                    car.Images.Add(new CarImage
                    {
                        Url = imageUrl,
                        IsMain = true,
                        SortOrder = 0
                    });
                }
                else
                {
                    main.Url = imageUrl;
                }
            }

            await context.SaveChangesAsync();
            return true;
        }

        // --------------------------
        // DELETE
        // --------------------------
        public async Task<bool> DeleteAsync(int id)
        {
            var car = await context.Cars.FirstOrDefaultAsync(c => c.Id == id);
            if (car == null) return false;

            context.Cars.Remove(car);
            await context.SaveChangesAsync();
            return true;
        }

        // --------------------------
        // Dropdowns (Mobile.bg style)
        // --------------------------
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

        // --------------------------
        // Search + Paging
        // --------------------------
        public async Task<(IEnumerable<Car> cars, int totalCount)> SearchAsync(CarsQueryViewModel query)
        {
            var q = context.Cars
                .AsNoTracking()
                .Include(c => c.Images) // ✅ Variant B (за главната снимка в листинга)
                .Include(c => c.CarModel)
                    .ThenInclude(cm => cm.Make)
                .AsQueryable();

            if (query.MakeId.HasValue && query.MakeId.Value > 0)
                q = q.Where(c => c.CarModel.MakeId == query.MakeId.Value);

            if (query.CarModelId.HasValue && query.CarModelId.Value > 0)
                q = q.Where(c => c.CarModelId == query.CarModelId.Value);

            if (!string.IsNullOrWhiteSpace(query.FuelType))
                q = q.Where(c => c.FuelType == query.FuelType);

            if (!string.IsNullOrWhiteSpace(query.Transmission))
                q = q.Where(c => c.Transmission == query.Transmission);

            if (query.YearFrom.HasValue)
                q = q.Where(c => c.Year >= query.YearFrom.Value);

            if (query.YearTo.HasValue)
                q = q.Where(c => c.Year <= query.YearTo.Value);

            if (query.PriceFrom.HasValue)
                q = q.Where(c => c.Price >= query.PriceFrom.Value);

            if (query.PriceTo.HasValue)
                q = q.Where(c => c.Price <= query.PriceTo.Value);

            q = query.Sort switch
            {
                "price_asc" => q.OrderBy(c => c.Price),
                "price_desc" => q.OrderByDescending(c => c.Price),
                "km_asc" => q.OrderBy(c => c.MileageKm),
                "year_desc" => q.OrderByDescending(c => c.Year),
                _ => q.OrderByDescending(c => c.Id) // newest
            };

            var total = await q.CountAsync();

            var page = query.Page < 1 ? 1 : query.Page;
            var pageSize = query.PageSize <= 0 ? 9 : query.PageSize;
            var skip = (page - 1) * pageSize;

            var cars = await q.Skip(skip).Take(pageSize).ToListAsync();
            return (cars, total);
        }
    }
}