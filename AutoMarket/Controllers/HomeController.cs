using AutoMarket.Services;
using AutoMarket.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace AutoMarket.Controllers
{
    public class HomeController : Controller
    {
        private readonly ICarService carService;

        public HomeController(ICarService carService)
        {
            this.carService = carService;
        }

        [HttpGet]
        public async Task<IActionResult> Index(
            int? makeId,
            int? carModelId,
            string? fuelType,
            string? transmission)
        {
            var makes = await carService.GetMakesAsync();
            var models = await carService.GetModelsAsync(makeId);

            var vm = new HomeViewModel
            {
                MakeId = makeId,
                CarModelId = carModelId,
                FuelType = fuelType,
                Transmission = transmission,

                Makes = makes
                    .Select(m => new SelectListItem(m.name, m.id.ToString()))
                    .ToList(),

                CarModels = models
                    .Select(m => new SelectListItem(m.name, m.id.ToString()))
                    .ToList(),

                LatestCars = await carService.GetLatestAsync(6)
            };

            return View(vm);
        }

        [HttpGet]
        public async Task<IActionResult> GetModelsByMake(int makeId)
        {
            var models = await carService.GetModelsAsync(makeId);

            var result = models.Select(m => new
            {
                id = m.id,
                name = m.name
            });

            return Json(result);
        }
    }
}