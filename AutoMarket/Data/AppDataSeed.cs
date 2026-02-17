using AutoMarket.Data;
using AutoMarket.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace AutoMarket.Data
{
    public static class AppDataSeed
    {
        public static async Task SeedAsync(IServiceProvider services)
        {
            using var scope = services.CreateScope();

            var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
            var userManager = scope.ServiceProvider.GetRequiredService<UserManager<IdentityUser>>();

            // 1) Миграции (важно при LocalDB/ново устройство)
            await db.Database.MigrateAsync();

            // 2) Взимаме admin-а като "продавач" (SellerId)
            var admin = await userManager.FindByEmailAsync("admin@automarket.bg");
            if (admin == null)
            {
                // ако няма admin, значи IdentitySeed не е минал/няма миграции за identity
                return;
            }

            // 3) Seed на марки + модели (ако ги няма)
            //    (Използваме GetOrCreate за да няма "Sequence contains no elements")
            var makeModels = new Dictionary<string, string[]>
            {
                ["Audi"] = new[] { "A3", "A4", "A6", "Q5", "Q7", "TT" },
                ["BMW"] = new[] { "1 Series", "3 Series", "5 Series", "X3", "X5", "M3" },
                ["Mercedes-Benz"] = new[] { "C-Class", "E-Class", "S-Class", "GLC", "GLE", "A-Class" },
                ["Volkswagen"] = new[] { "Golf", "Passat", "Tiguan", "Touareg", "Polo" },
                ["Toyota"] = new[] { "Corolla", "Camry", "RAV4", "Yaris", "Land Cruiser" },
                ["Honda"] = new[] { "Civic", "Accord", "CR-V", "Jazz", "HR-V" },
                ["Ford"] = new[] { "Focus", "Fiesta", "Mustang GT", "Kuga", "Ranger" },
                ["Opel"] = new[] { "Astra", "Insignia", "Corsa", "Mokka", "Zafira" },
                ["Peugeot"] = new[] { "208", "308", "508", "3008", "5008" },
                ["Renault"] = new[] { "Clio", "Megane", "Captur", "Kadjar", "Talisman" },
                ["Skoda"] = new[] { "Octavia", "Superb", "Kodiaq", "Fabia", "Karoq" },
                ["Hyundai"] = new[] { "i20", "i30", "Tucson", "Santa Fe", "Elantra" },
                ["Kia"] = new[] { "Ceed", "Sportage", "Sorento", "Rio", "Stinger" },
                ["Nissan"] = new[] { "Qashqai", "X-Trail", "Juke", "Micra", "Navara" },
                ["Mazda"] = new[] { "Mazda3", "Mazda6", "CX-5", "CX-30", "MX-5" },
            };

            foreach (var kvp in makeModels)
            {
                var makeName = kvp.Key;
                var modelNames = kvp.Value;

                var makeId = await GetOrCreateMakeId(db, makeName);

                foreach (var modelName in modelNames)
                {
                    await GetOrCreateModelId(db, makeId, modelName);
                }
            }

            // 4) Ако вече имаш коли - НЕ добавяме пак (за да не дублира)
            if (await db.Cars.AnyAsync())
                return;

            // helper за лесно взимане на CarModelId
            async Task<int> M(string make, string model)
            {
                var makeEntity = await db.Makes.FirstOrDefaultAsync(x => x.Name == make);
                if (makeEntity == null) throw new InvalidOperationException($"Missing make: {make}");

                var modelEntity = await db.CarModels.FirstOrDefaultAsync(x => x.MakeId == makeEntity.Id && x.Name == model);
                if (modelEntity == null) throw new InvalidOperationException($"Missing model: {make} {model}");

                return modelEntity.Id;
            }

            // 5) Seed на обяви (примерни реалистични описания + локални снимки)
            //    ImageUrl сочи към /img/cars/.... => файловете трябва да са в wwwroot/img/cars/
            var cars = new List<Car>
            {
                new Car {
                    CarModelId = await M("BMW", "3 Series"),
                    Year = 2020, Price = 58900, MileageKm = 72000, EngineCapacityCc = 1998, HorsePower = 190,
                    FuelType = "Бензин", Transmission = "Автоматична",
                    Description = "BMW 3 Series 320i. Отлично състояние, сервизна история, 2 ключа, без забележки по интериора.",
                    ImageUrl = "/img/cars/bmw-3-series.jpg",
                    SellerId = admin.Id
                },
                new Car {
                    CarModelId = await M("Audi", "A4"),
                    Year = 2019, Price = 54500, MileageKm = 98000, EngineCapacityCc = 1968, HorsePower = 150,
                    FuelType = "Дизел", Transmission = "Автоматична",
                    Description = "Audi A4 2.0 TDI S-tronic. Икономична, обслужена, нови гуми, реални километри.",
                    ImageUrl = "/img/cars/audi-a4.jpg",
                    SellerId = admin.Id
                },
                new Car {
                    CarModelId = await M("Mercedes-Benz", "E-Class"),
                    Year = 2018, Price = 69900, MileageKm = 110000, EngineCapacityCc = 2143, HorsePower = 170,
                    FuelType = "Дизел", Transmission = "Автоматична",
                    Description = "Mercedes E-Class. Комфортен автомобил, обслужван навреме, много екстри и отлична динамика.",
                    ImageUrl = "/img/cars/mercedes-e-class.jpg",
                    SellerId = admin.Id
                },
                new Car {
                    CarModelId = await M("Volkswagen", "Passat"),
                    Year = 2017, Price = 32900, MileageKm = 149000, EngineCapacityCc = 1968, HorsePower = 150,
                    FuelType = "Дизел", Transmission = "Ръчна",
                    Description = "VW Passat 2.0 TDI. Просторен семеен автомобил, климатроник, поддържан интериор.",
                    ImageUrl = "/img/cars/vw-passat.jpg",
                    SellerId = admin.Id
                },
                new Car {
                    CarModelId = await M("Toyota", "RAV4"),
                    Year = 2020, Price = 72000, MileageKm = 47000, EngineCapacityCc = 2487, HorsePower = 218,
                    FuelType = "Хибрид", Transmission = "Автоматична",
                    Description = "Toyota RAV4 Hybrid. Много икономична, перфектна за град и път, без удари, пълно обслужване.",
                    ImageUrl = "/img/cars/toyota-rav4.jpg",
                    SellerId = admin.Id
                },
                new Car {
                    CarModelId = await M("Ford", "Mustang GT"),
                    Year = 2021, Price = 99000, MileageKm = 32000, EngineCapacityCc = 5038, HorsePower = 450,
                    FuelType = "Бензин", Transmission = "Автоматична",
                    Description = "Ford Mustang GT 5.0 V8. Уникален звук, поддържан, гаражен, без компромиси.",
                    ImageUrl = "/img/cars/ford-mustang-gt.jpg",
                    SellerId = admin.Id
                },

                // още обяви - за да има „истински магазин“
                new Car {
                    CarModelId = await M("Honda", "Civic"),
                    Year = 2016, Price = 21900, MileageKm = 136000, EngineCapacityCc = 1597, HorsePower = 120,
                    FuelType = "Бензин", Transmission = "Ръчна",
                    Description = "Honda Civic. Надежден автомобил, нисък разход, поддържан, отличен за начинаещи.",
                    ImageUrl = "/img/cars/honda-civic.jpg",
                    SellerId = admin.Id
                },
                new Car {
                    CarModelId = await M("Skoda", "Octavia"),
                    Year = 2019, Price = 27900, MileageKm = 118000, EngineCapacityCc = 1598, HorsePower = 115,
                    FuelType = "Дизел", Transmission = "Ръчна",
                    Description = "Skoda Octavia 1.6 TDI. Просторна, практична, обслужена, много добър багажник.",
                    ImageUrl = "/img/cars/skoda-octavia.jpg",
                    SellerId = admin.Id
                },
                new Car {
                    CarModelId = await M("Hyundai", "Tucson"),
                    Year = 2020, Price = 46800, MileageKm = 82000, EngineCapacityCc = 1598, HorsePower = 177,
                    FuelType = "Бензин", Transmission = "Автоматична",
                    Description = "Hyundai Tucson. Стилен SUV, висок клас оборудване, камера, парктроник, без забележки.",
                    ImageUrl = "/img/cars/hyundai-tucson.jpg",
                    SellerId = admin.Id
                },
                new Car {
                    CarModelId = await M("Kia", "Sportage"),
                    Year = 2018, Price = 38900, MileageKm = 105000, EngineCapacityCc = 1995, HorsePower = 136,
                    FuelType = "Дизел", Transmission = "Автоматична",
                    Description = "Kia Sportage. Комфортен SUV, отличен за семейство, обслужен, реални километри.",
                    ImageUrl = "/img/cars/kia-sportage.jpg",
                    SellerId = admin.Id
                },
                new Car {
                    CarModelId = await M("Nissan", "Qashqai"),
                    Year = 2017, Price = 29800, MileageKm = 132000, EngineCapacityCc = 1598, HorsePower = 130,
                    FuelType = "Бензин", Transmission = "Ръчна",
                    Description = "Nissan Qashqai. Висока позиция на шофиране, много удобен, поддържан салон.",
                    ImageUrl = "/img/cars/nissan-qashqai.jpg",
                    SellerId = admin.Id
                },
                new Car {
                    CarModelId = await M("Mazda", "CX-5"),
                    Year = 2019, Price = 49900, MileageKm = 93000, EngineCapacityCc = 1998, HorsePower = 165,
                    FuelType = "Бензин", Transmission = "Автоматична",
                    Description = "Mazda CX-5. Много добро управление, богат пакет екстри, обслужена, без удари.",
                    ImageUrl = "/img/cars/mazda-cx5.jpg",
                    SellerId = admin.Id
                },
                new Car {
                    CarModelId = await M("Renault", "Megane"),
                    Year = 2018, Price = 19900, MileageKm = 141000, EngineCapacityCc = 1461, HorsePower = 110,
                    FuelType = "Дизел", Transmission = "Ръчна",
                    Description = "Renault Megane. Икономична, поддържана, климатроник, нов акумулатор.",
                    ImageUrl = "/img/cars/renault-megane.jpg",
                    SellerId = admin.Id
                },
                new Car {
                    CarModelId = await M("Peugeot", "3008"),
                    Year = 2019, Price = 41900, MileageKm = 99000, EngineCapacityCc = 1499, HorsePower = 130,
                    FuelType = "Дизел", Transmission = "Автоматична",
                    Description = "Peugeot 3008. Модерен интериор, високо ниво на безопасност, обслужен, отличен външен вид.",
                    ImageUrl = "/img/cars/peugeot-3008.jpg",
                    SellerId = admin.Id
                },
                new Car {
                    CarModelId = await M("Opel", "Astra"),
                    Year = 2017, Price = 16800, MileageKm = 152000, EngineCapacityCc = 1598, HorsePower = 136,
                    FuelType = "Дизел", Transmission = "Ръчна",
                    Description = "Opel Astra. Практичен и евтин за поддръжка, 2 комплекта гуми, много запазен.",
                    ImageUrl = "/img/cars/opel-astra.jpg",
                    SellerId = admin.Id
                },
                new Car {
                    
                    CarModelId = await M("Honda", "Civic"),
                    Year = 2018,
                    Price = 18900m,
                    MileageKm = 98000,
                    EngineCapacityCc = 1598,
                    HorsePower = 120,
                    FuelType = "Бензин",
                    Transmission = "Ръчна",
                    Description = "Honda Civic 1.6 i-DTEC, климатроник, поддържана, сервизна история.",
                    ImageUrl = "/img/cars/honda-civic.jpg",
                    SellerId = admin.Id
},
            };

            db.Cars.AddRange(cars);
            await db.SaveChangesAsync();
        }

        private static async Task<int> GetOrCreateMakeId(ApplicationDbContext db, string makeName)
        {
            var make = await db.Makes.FirstOrDefaultAsync(m => m.Name == makeName);
            if (make != null) return make.Id;

            make = new Make { Name = makeName };
            db.Makes.Add(make);
            await db.SaveChangesAsync();
            return make.Id;
        }

        private static async Task<int> GetOrCreateModelId(ApplicationDbContext db, int makeId, string modelName)
        {
            var model = await db.CarModels.FirstOrDefaultAsync(cm => cm.MakeId == makeId && cm.Name == modelName);
            if (model != null) return model.Id;

            model = new CarModel { MakeId = makeId, Name = modelName };
            db.CarModels.Add(model);
            await db.SaveChangesAsync();
            return model.Id;
        }
    }
}
