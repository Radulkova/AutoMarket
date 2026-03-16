using AutoMarket.Services;
using AutoMarket.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Security.Claims;

namespace AutoMarket.Controllers
{
    public class CarsController : Controller
    {
        private readonly ICarService service;
        private readonly IFavoriteService favoriteService;

        public CarsController(ICarService service, IFavoriteService favoriteService)
        {
            this.service = service;
            this.favoriteService = favoriteService;
        }

        [HttpGet]
        public async Task<IActionResult> Index([FromQuery] CarsQueryViewModel query)
        {
            var makes = await service.GetMakesAsync();
            query.Makes = makes
                .Select(m => new SelectListItem(m.name, m.id.ToString()))
                .ToList();

            var models = await service.GetModelsAsync(query.MakeId);
            query.CarModels = models
                .Select(m => new SelectListItem(m.name, m.id.ToString()))
                .ToList();

            var (cars, total) = await service.SearchAsync(query);
            query.Cars = cars;
            query.TotalCount = total;

            if (User.Identity?.IsAuthenticated == true)
            {
                var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                if (!string.IsNullOrWhiteSpace(userId))
                {
                    query.FavoriteCarIds = await favoriteService.GetFavoriteCarIdsAsync(userId);
                }
            }

            return View(query);
        }

        [HttpGet]
        public async Task<IActionResult> ModelsByMake(int makeId)
        {
            var models = await service.GetModelsAsync(makeId);
            return Json(models.Select(m => new { id = m.id, name = m.name }));
        }

        [HttpGet]
        public async Task<IActionResult> Details(int id)
        {
            var car = await service.GetByIdAsync(id);
            if (car == null)
            {
                return NotFound();
            }

            if (User.Identity?.IsAuthenticated == true)
            {
                var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                if (!string.IsNullOrWhiteSpace(userId))
                {
                    ViewBag.IsFavorite = await favoriteService.IsFavoriteAsync(id, userId);
                }
            }

            return View(car);
        }

        [Authorize(Roles = "Admin")]
        [HttpGet]
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

        [Authorize(Roles = "Admin")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(CarCreateViewModel model)
        {
            model.CarModels = await service.GetCarModelsForSelectAsync();

            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var sellerId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrWhiteSpace(sellerId))
            {
                return Challenge();
            }

            var ok = await service.AddAsync(model, sellerId);

            if (!ok)
            {
                ModelState.AddModelError(nameof(model.CarModelId), "Моля, изберете модел от списъка.");
                return View(model);
            }

            TempData["Success"] = "Автомобилът е добавен успешно.";
            return RedirectToAction(nameof(Index));
        }

        [Authorize(Roles = "Admin")]
        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            var model = await service.GetForEditAsync(id);
            if (model == null)
            {
                return NotFound();
            }

            return View(model);
        }

        [Authorize(Roles = "Admin")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, CarEditViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var ok = await service.UpdateAsync(id, model);
            if (!ok)
            {
                return NotFound();
            }

            TempData["Success"] = "Автомобилът е редактиран успешно.";
            return RedirectToAction(nameof(Details), new { id });
        }

        [Authorize(Roles = "Admin")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            var ok = await service.DeleteAsync(id);
            if (!ok)
            {
                return NotFound();
            }

            TempData["Success"] = "Автомобилът е изтрит успешно.";
            return RedirectToAction(nameof(Index));
        }
    }
}