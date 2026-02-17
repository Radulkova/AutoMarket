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

        // Mobile.bg style Index
        [HttpGet]
        public async Task<IActionResult> Index([FromQuery] CarsQueryViewModel query)
        {
            var makes = await service.GetMakesAsync();
            query.Makes = makes.Select(m => new Microsoft.AspNetCore.Mvc.Rendering.SelectListItem(m.name, m.id.ToString())).ToList();

            var models = await service.GetModelsAsync(query.MakeId);
            query.CarModels = models.Select(m => new Microsoft.AspNetCore.Mvc.Rendering.SelectListItem(m.name, m.id.ToString())).ToList();

            var (cars, total) = await service.SearchAsync(query);
            query.Cars = cars;
            query.TotalCount = total;

            return View(query);
        }

        // AJAX: /Cars/ModelsByMake?makeId=1
        [HttpGet]
        public async Task<IActionResult> ModelsByMake(int makeId)
        {
            var models = await service.GetModelsAsync(makeId);
            return Json(models.Select(m => new { id = m.id, name = m.name }));
        }

        public async Task<IActionResult> Details(int id)
        {
            var car = await service.GetByIdAsync(id);
            if (car == null) return NotFound();
            return View(car);
        }

        // CREATE (Admin only)
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
        [ValidateAntiForgeryToken]
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

        // EDIT (Admin only)
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Edit(int id)
        {
            var model = await service.GetForEditAsync(id);
            if (model == null) return NotFound();
            return View(model);
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, CarEditViewModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            var ok = await service.UpdateAsync(id, model);
            if (!ok) return NotFound();

            return RedirectToAction(nameof(Details), new { id });
        }

        // DELETE (Admin only)
        [HttpPost]
        [Authorize(Roles = "Admin")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            var ok = await service.DeleteAsync(id);
            if (!ok) return NotFound();
            return RedirectToAction(nameof(Index));
        }
    }
}
