using AutoMarket.Data.Models;
using AutoMarket.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AutoMarket.Data
{
    public static class AppDataSeed
    {
        public static async Task SeedAsync(IServiceProvider services)
        {
            using var scope = services.CreateScope();

            var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
            var userManager = scope.ServiceProvider.GetRequiredService<UserManager<IdentityUser>>();

            // важно: да има миграции/таблици
            await db.Database.MigrateAsync();

            var admin = await userManager.FindByEmailAsync("admin@automarket.bg");
            if (admin == null) return;

            // Ако вече имаш коли - не seed-ваме повторно
            if (await db.Cars.AnyAsync()) return;

            // 15 марки
            var makes = new[]
            {
                "Audi","BMW","Mercedes-Benz","Volkswagen","Toyota","Honda","Ford","Hyundai","Kia","Skoda",
                "Peugeot","Renault","Opel","Mazda","Nissan"
            };

            foreach (var m in makes)
                if (!await db.Makes.AnyAsync(x => x.Name == m))
                    db.Makes.Add(new Make { Name = m });

            await db.SaveChangesAsync();

            async Task<int> EnsureModel(string makeName, string modelName)
            {
                var make = await db.Makes.FirstAsync(m => m.Name == makeName);
                var existing = await db.CarModels.FirstOrDefaultAsync(cm => cm.MakeId == make.Id && cm.Name == modelName);
                if (existing != null) return existing.Id;

                var cm = new CarModel { MakeId = make.Id, Name = modelName };
                db.CarModels.Add(cm);
                await db.SaveChangesAsync();
                return cm.Id;
            }

            // Helper: създава Car + добавя 1 главна снимка (от твоя стар ImageUrl)
            Car CreateCar(
                int carModelId,
                int year,
                int price,
                int mileageKm,
                int engineCapacityCc,
                int horsePower,
                string fuelType,
                string transmission,
                string description,
                string imageUrl,
                string sellerId)
            {
                var car = new Car
                {
                    CarModelId = carModelId,
                    Year = year,
                    Price = price,
                    MileageKm = mileageKm,
                    EngineCapacityCc = engineCapacityCc,
                    HorsePower = horsePower,
                    FuelType = fuelType,
                    Transmission = transmission,
                    Description = description,
                    SellerId = sellerId,

                    // Старото поле може да остане, но вече не го ползваме:
                    // ImageUrl = imageUrl
                };

                // Вариант B: записваме снимката в CarImages
                car.Images.Add(new CarImage
                {
                    Url = imageUrl,
                    IsMain = true,
                    SortOrder = 0
                });

                return car;
            }

            var cars = new List<Car>
            {
                // Audi
                CreateCar(await EnsureModel("Audi","A4"), 2019, 52000, 98000, 1968, 190, "Дизел", "Автоматична",
                    "Audi A4 2.0 TDI, обслужена, реални км.", "/img/cars/audi-a4.jpg", admin.Id),

                CreateCar(await EnsureModel("Audi","A6"), 2020, 78000, 72000, 1984, 245, "Бензин", "Автоматична",
                    "Audi A6, Quattro, много запазена.", "/img/cars/audi-a6.jpg", admin.Id),

                CreateCar(await EnsureModel("Audi","Q5"), 2018, 69000, 110000, 1968, 190, "Дизел", "Автоматична",
                    "Audi Q5, 4x4, отлична за семейство.", "/img/cars/audi-q5.jpg", admin.Id),

                CreateCar(await EnsureModel("Audi","Q7"), 2017, 88000, 145000, 2967, 272, "Дизел", "Автоматична",
                    "Audi Q7, 7 места, голяма и удобна.", "/img/cars/audi-q7.jpg", admin.Id),

                CreateCar(await EnsureModel("Audi","A3"), 2021, 46000, 54000, 1498, 150, "Бензин", "Ръчна",
                    "Audi A3, икономична и пъргава.", "/img/cars/audi-a3.jpg", admin.Id),

                // BMW
                CreateCar(await EnsureModel("BMW","3 Series"), 2019, 61000, 89000, 1998, 184, "Бензин", "Автоматична",
                    "BMW 3 Series, спортен пакет, без забележки.", "/img/cars/bmw-3-series.jpg", admin.Id),

                CreateCar(await EnsureModel("BMW","5 Series"), 2018, 72000, 125000, 1995, 190, "Дизел", "Автоматична",
                    "BMW 5 Series, комфорт и динамика.", "/img/cars/bmw-5-series.jpg", admin.Id),

                CreateCar(await EnsureModel("BMW","X5"), 2017, 79000, 158000, 2993, 258, "Дизел", "Автоматична",
                    "BMW X5, 4x4, обслужен.", "/img/cars/bmw-x5.jpg", admin.Id),

                CreateCar(await EnsureModel("BMW","X3"), 2020, 83000, 68000, 1995, 190, "Дизел", "Автоматична",
                    "BMW X3, xDrive, отлична визия.", "/img/cars/bmw-x3.jpg", admin.Id),

                CreateCar(await EnsureModel("BMW","1 Series"), 2021, 42000, 41000, 1499, 140, "Бензин", "Автоматична",
                    "BMW 1 Series, градска и модерна.", "/img/cars/bmw-1-series.jpg", admin.Id),

                // Mercedes-Benz
                CreateCar(await EnsureModel("Mercedes-Benz","C-Class"), 2019, 69000, 99000, 1950, 194, "Дизел", "Автоматична",
                    "C-Class, AMG line, топ състояние.", "/img/cars/mercedes-benz-c-class.jpg", admin.Id),

                CreateCar(await EnsureModel("Mercedes-Benz","E-Class"), 2018, 99000, 110000, 1950, 194, "Дизел", "Автоматична",
                    "E-Class, много екстри, комфорт.", "/img/cars/mercedes-benz-e-class.jpg", admin.Id),

                CreateCar(await EnsureModel("Mercedes-Benz","GLC"), 2020, 118000, 68000, 1991, 245, "Бензин", "Автоматична",
                    "GLC, 4Matic, отлично семейно SUV.", "/img/cars/mercedes-benz-glc.jpg", admin.Id),

                CreateCar(await EnsureModel("Mercedes-Benz","S-Class"), 2017, 145000, 160000, 2925, 286, "Дизел", "Автоматична",
                    "S-Class, лукс и тишина.", "/img/cars/mercedes-benz-s-class.jpg", admin.Id),

                CreateCar(await EnsureModel("Mercedes-Benz","A-Class"), 2021, 56000, 42000, 1332, 163, "Бензин", "Автоматична",
                    "A-Class, много запазена, перфектна за град.", "/img/cars/mercedes-benz-a-class.jpg", admin.Id),

                // VW
                CreateCar(await EnsureModel("Volkswagen","Passat"), 2019, 45000, 89000, 1968, 150, "Дизел", "Автоматична",
                    "Passat, икономичен и просторен.", "/img/cars/volkswagen-passat.jpg", admin.Id),

                CreateCar(await EnsureModel("Volkswagen","Golf"), 2020, 39000, 72000, 1498, 150, "Бензин", "Ръчна",
                    "Golf, перфектен за всеки ден.", "/img/cars/volkswagen-golf.jpg", admin.Id),

                CreateCar(await EnsureModel("Volkswagen","Tiguan"), 2018, 52000, 123000, 1968, 190, "Дизел", "Автоматична",
                    "Tiguan, висок клас, удобно SUV.", "/img/cars/volkswagen-tiguan.jpg", admin.Id),

                CreateCar(await EnsureModel("Volkswagen","Touareg"), 2017, 79000, 170000, 2967, 262, "Дизел", "Автоматична",
                    "Touareg, мощен 4x4.", "/img/cars/volkswagen-touareg.jpg", admin.Id),

                CreateCar(await EnsureModel("Volkswagen","Polo"), 2021, 25000, 42000, 999, 95, "Бензин", "Ръчна",
                    "Polo, много икономична.", "/img/cars/volkswagen-polo.jpg", admin.Id),

                // Toyota
                CreateCar(await EnsureModel("Toyota","Corolla"), 2020, 42000, 54000, 1798, 122, "Хибрид", "Автоматична",
                    "Corolla Hybrid, супер разход.", "/img/cars/toyota-corolla.jpg", admin.Id),

                CreateCar(await EnsureModel("Toyota","RAV4"), 2020, 72000, 47000, 2487, 222, "Хибрид", "Автоматична",
                    "RAV4 Hybrid, 4x4.", "/img/cars/toyota-rav4.jpg", admin.Id),

                CreateCar(await EnsureModel("Toyota","Camry"), 2019, 56000, 82000, 2487, 178, "Бензин", "Автоматична",
                    "Camry, надеждна и комфортна.", "/img/cars/toyota-camry.jpg", admin.Id),

                CreateCar(await EnsureModel("Toyota","Yaris"), 2021, 23000, 31000, 1490, 111, "Бензин", "Ръчна",
                    "Yaris, малка и практична.", "/img/cars/toyota-yaris.jpg", admin.Id),

                CreateCar(await EnsureModel("Toyota","Land Cruiser"), 2017, 135000, 190000, 2755, 177, "Дизел", "Автоматична",
                    "Land Cruiser, легендарен 4x4.", "/img/cars/toyota-land-cruiser.jpg", admin.Id),

                // Honda
                CreateCar(await EnsureModel("Honda","Civic"), 2018, 19000, 128000, 1597, 120, "Бензин", "Ръчна",
                    "Civic, надеждна и икономична.", "/img/cars/honda-civic.jpg", admin.Id),

                CreateCar(await EnsureModel("Honda","Accord"), 2017, 26000, 145000, 1997, 155, "Бензин", "Автоматична",
                    "Accord, простор и комфорт.", "/img/cars/honda-accord.jpg", admin.Id),

                CreateCar(await EnsureModel("Honda","CR-V"), 2019, 52000, 98000, 1997, 155, "Бензин", "Автоматична",
                    "CR-V, семейно SUV.", "/img/cars/honda-cr-v.jpg", admin.Id),

                CreateCar(await EnsureModel("Honda","Jazz"), 2020, 22000, 64000, 1318, 102, "Бензин", "Ръчна",
                    "Jazz, практична за града.", "/img/cars/honda-jazz.jpg", admin.Id),

                CreateCar(await EnsureModel("Honda","HR-V"), 2021, 41000, 39000, 1498, 130, "Бензин", "Автоматична",
                    "HR-V, компактно SUV.", "/img/cars/honda-hr-v.jpg", admin.Id),

                // Ford
                CreateCar(await EnsureModel("Ford","Mustang"), 2021, 99000, 32000, 5000, 450, "Бензин", "Автоматична",
                    "Mustang 5.0 V8, звук и емоция.", "/img/cars/ford-mustang.jpg", admin.Id),

                CreateCar(await EnsureModel("Ford","Focus"), 2019, 24000, 98000, 1498, 150, "Бензин", "Ръчна",
                    "Focus, икономичен хечбек.", "/img/cars/ford-focus.jpg", admin.Id),

                CreateCar(await EnsureModel("Ford","Kuga"), 2018, 35000, 124000, 1997, 150, "Дизел", "Автоматична",
                    "Kuga, удобен SUV.", "/img/cars/ford-kuga.jpg", admin.Id),

                CreateCar(await EnsureModel("Ford","Fiesta"), 2020, 18500, 52000, 998, 100, "Бензин", "Ръчна",
                    "Fiesta, супер за град.", "/img/cars/ford-fiesta.jpg", admin.Id),

                CreateCar(await EnsureModel("Ford","Ranger"), 2017, 59000, 160000, 3198, 200, "Дизел", "Автоматична",
                    "Ranger, пикап за работа и офроуд.", "/img/cars/ford-ranger.jpg", admin.Id),
            };

            // Гарантираме минимум 50 обяви
            var rnd = new Random(42);
            while (cars.Count < 50)
            {
                var baseCar = cars[rnd.Next(cars.Count)];

                var clone = new Car
                {
                    CarModelId = baseCar.CarModelId,
                    Year = Math.Max(2005, Math.Min(2024, baseCar.Year + rnd.Next(-3, 4))),
                    Price = Math.Max(3000, baseCar.Price + rnd.Next(-5000, 7000)),
                    MileageKm = Math.Max(1000, baseCar.MileageKm + rnd.Next(-20000, 30000)),
                    EngineCapacityCc = baseCar.EngineCapacityCc,
                    HorsePower = baseCar.HorsePower,
                    FuelType = baseCar.FuelType,
                    Transmission = baseCar.Transmission,
                    Description = baseCar.Description,
                    SellerId = admin.Id
                };

                // копираме снимките (НО като нови записи, не същите обекти!)
                foreach (var img in baseCar.Images.OrderBy(i => i.SortOrder))
                {
                    clone.Images.Add(new CarImage
                    {
                        Url = img.Url,
                        IsMain = img.IsMain,
                        SortOrder = img.SortOrder
                    });
                }

                cars.Add(clone);
            }

            db.Cars.AddRange(cars);
            await db.SaveChangesAsync();
        }
    }
}
