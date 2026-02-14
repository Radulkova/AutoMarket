using AutoMarket.Services;
using AutoMarket.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace AutoMarket.Controllers
{
    public class CarsController : Controller
    {
        private readonly ICarService service;

        public CarsController(ICarService service)
        {
            this.service = service;
        }

        public async Task<IActionResult> Index()
        {
            var cars = await service.GetAllAsync();
            return View(cars);
        }

        public async Task<IActionResult> Details(int id)
        {
            var car = await service.GetByIdAsync(id);
            if (car == null) return NotFound();
            return View(car);
        }

        // ==========================
        // CREATE
        // ==========================
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Create()
        {
            var model = new CarCreateViewModel
            {
                CarModels = await service.GetCarModelsForSelectAsync(),
                FuelType = "Бензин",
                Transmission = "Ръчна"
            };

            return View(model);
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Create(CarCreateViewModel model)
        {
            model.CarModels = await service.GetCarModelsForSelectAsync();

            if (!ModelState.IsValid)
                return View(model);

            var sellerId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            var ok = await service.AddAsync(model, sellerId!);
            if (!ok)
            {
                ModelState.AddModelError(nameof(model.CarModelId), "Моля, изберете модел от списъка.");
                return View(model);
            }

            return RedirectToAction(nameof(Index));
        }

        // ==========================
        // EDIT
        // ==========================
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Edit(int id)
        {
            var model = await service.GetForEditAsync(id);
            if (model == null) return NotFound();
            return View(model);
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Edit(int id, CarEditViewModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            var ok = await service.UpdateAsync(id, model);
            if (!ok) return NotFound();

            return RedirectToAction(nameof(Details), new { id });
        }

        // ==========================
        // DELETE
        // ==========================
        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Delete(int id)
        {
            var ok = await service.DeleteAsync(id);
            if (!ok) return NotFound();

            return RedirectToAction(nameof(Index));
        }
    }
}
