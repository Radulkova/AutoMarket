using AutoMarket.Services;
using AutoMarket.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AutoMarket.Controllers
{
    [Authorize(Roles = "Admin")]
    public class CarModelsController : Controller
    {
        private readonly ICarModelService service;

        public CarModelsController(ICarModelService service)
        {
            this.service = service;
        }

        [HttpGet]
        public async Task<IActionResult> Create()
        {
            var model = new CarModelCreateViewModel
            {
                Makes = await service.GetMakesForSelectAsync()
            };

            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> Create(CarModelCreateViewModel model)
        {
            model.Makes = await service.GetMakesForSelectAsync();

            if (!ModelState.IsValid)
                return View(model);

            var (ok, error) = await service.CreateModelAsync(model);

            if (!ok)
            {
                ModelState.AddModelError(string.Empty, error ?? "Грешка при запис.");
                return View(model);
            }

            return RedirectToAction("Create", "Cars");
        }
    }
}
