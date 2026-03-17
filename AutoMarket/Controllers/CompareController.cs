using AutoMarket.Services;
using AutoMarket.ViewModels;
using Microsoft.AspNetCore.Mvc;

namespace AutoMarket.Controllers
{
    public class CompareController : Controller
    {
        private readonly ICompareService compareService;
        private readonly ICarService carService;

        public CompareController(ICompareService compareService, ICarService carService)
        {
            this.compareService = compareService;
            this.carService = carService;
        }

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var ids = compareService.GetComparedCarIds();

            var cars = new List<AutoMarket.Models.Car>();

            foreach (var id in ids)
            {
                var car = await carService.GetByIdAsync(id);
                if (car != null && !car.IsDeleted)
                {
                    cars.Add(car);
                }
            }

            var model = new CompareViewModel
            {
                Cars = cars
            };

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Add(int carId, string? returnUrl)
        {
            var car = await carService.GetByIdAsync(carId);
            if (car == null || car.IsDeleted)
            {
                TempData["Error"] = "Автомобилът не беше намерен.";
                return RedirectToLocal(returnUrl);
            }

            var result = compareService.Add(carId);
            if (result.success)
            {
                TempData["Success"] = result.message;
            }
            else
            {
                TempData["Error"] = result.message;
            }

            return RedirectToLocal(returnUrl);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Remove(int carId, string? returnUrl)
        {
            compareService.Remove(carId);
            TempData["Success"] = "Автомобилът е премахнат от сравнението.";

            return RedirectToLocal(returnUrl, "Index");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Clear()
        {
            compareService.Clear();
            TempData["Success"] = "Списъкът за сравнение е изчистен.";

            return RedirectToAction(nameof(Index));
        }

        private IActionResult RedirectToLocal(string? returnUrl, string fallbackAction = "Index")
        {
            if (!string.IsNullOrWhiteSpace(returnUrl) && Url.IsLocalUrl(returnUrl))
            {
                return Redirect(returnUrl);
            }

            return RedirectToAction(fallbackAction, "Compare");
        }
    }
}