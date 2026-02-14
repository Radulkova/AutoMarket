using AutoMarket.Data;
using AutoMarket.Models;
using AutoMarket.ViewModels;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace AutoMarket.Services
{
    public class CarModelService : ICarModelService
    {
        private readonly ApplicationDbContext context;

        public CarModelService(ApplicationDbContext context)
        {
            this.context = context;
        }

        public async Task<List<SelectListItem>> GetMakesForSelectAsync()
        {
            return await context.Makes
                .OrderBy(m => m.Name)
                .Select(m => new SelectListItem
                {
                    Value = m.Id.ToString(),
                    Text = m.Name
                })
                .ToListAsync();
        }

        public async Task<(bool ok, string? error)> CreateModelAsync(CarModelCreateViewModel model)
        {
            var modelName = (model.Name ?? "").Trim();
            var newMakeName = (model.NewMakeName ?? "").Trim();

            if (string.IsNullOrWhiteSpace(modelName))
                return (false, "Моля, въведете модел.");

            // ✅ 1) Определяме марката:
            Make? make = null;

            // ако има въведена нова марка -> ползваме нея (създаваме/намираме)
            if (!string.IsNullOrWhiteSpace(newMakeName))
            {
                make = await context.Makes.FirstOrDefaultAsync(m => m.Name == newMakeName);
                if (make == null)
                {
                    make = new Make { Name = newMakeName };
                    context.Makes.Add(make);
                    await context.SaveChangesAsync();
                }
            }
            else
            {
                // иначе -> трябва да е избрана съществуваща
                if (model.MakeId == null || model.MakeId <= 0)
                    return (false, "Избери марка или въведи нова.");

                make = await context.Makes.FirstOrDefaultAsync(m => m.Id == model.MakeId.Value);
                if (make == null)
                    return (false, "Избраната марка не съществува.");
            }

            // ✅ 2) Проверка за дубликат (Марка + Модел)
            var exists = await context.CarModels.AnyAsync(cm =>
                cm.MakeId == make.Id && cm.Name == modelName);

            if (exists)
                return (false, "Този модел вече съществува за тази марка.");

            // ✅ 3) Създаваме модела
            var carModel = new CarModel
            {
                MakeId = make.Id,
                Name = modelName
            };

            context.CarModels.Add(carModel);
            await context.SaveChangesAsync();

            return (true, null);
        }
    }
}
