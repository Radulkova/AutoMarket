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

        // LIST
        public async Task<IActionResult> Index()
        {
            var cars = await service.GetAllAsync();
            return View(cars);
        }

        // DETAILS
        public async Task<IActionResult> Details(int id)
        {
            var car = await service.GetByIdAsync(id);
            if (car == null) return NotFound();
            return View(car);
        }

        // CREATE (GET)
        [Authorize(Roles = "Admin")]
        public IActionResult Create()
        {
            return View();
        }

        // CREATE (POST)
        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Create(CarCreateViewModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            var sellerId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            await service.AddAsync(model, sellerId!);

            return RedirectToAction(nameof(Index));
        }

        // ==========================
        // ✅ EDIT (GET)
        // ==========================
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Edit(int id)
        {
            var model = await service.GetForEditAsync(id);
            if (model == null) return NotFound();

            return View(model);
        }

        // ==========================
        // ✅ EDIT (POST)
        // ==========================
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
        // ✅ DELETE (POST)
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
