using AutoMarket.Services;
using AutoMarket.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace AutoMarket.Controllers
{
    [Authorize]
    public class OrdersController : Controller
    {
        private readonly IOrderService _orders;
        private readonly UserManager<IdentityUser> _userManager;

        public OrdersController(IOrderService orders, UserManager<IdentityUser> userManager)
        {
            _orders = orders;
            _userManager = userManager;
        }

        // POST: /Orders/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(ReserveViewingViewModel model)
        {
            if (!ModelState.IsValid)
            {
                TempData["Error"] = "Моля, попълни коректно данните за огледа.";
                return RedirectToAction("Details", "Cars", new { id = model.CarId });
            }

            var userId = _userManager.GetUserId(User);

            try
            {
                await _orders.CreateAsync(model.CarId, userId!, model.FirstName, model.LastName, model.Phone, model.Note);
                TempData["Success"] = "Успешно резервирахте оглед на автомобила.";
            }
            catch (Exception ex)
            {
                TempData["Error"] = ex.Message;
            }

            return RedirectToAction("Details", "Cars", new { id = model.CarId });
        }

        // GET: /Orders/My
        public async Task<IActionResult> My()
        {
            var userId = _userManager.GetUserId(User);
            var orders = await _orders.GetMyOrdersAsync(userId!);
            return View(orders);
        }

        // GET: /Orders/Pending  (Admin)
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Pending()
        {
            var orders = await _orders.GetPendingAsync();
            return View(orders);
        }

        // GET: /Orders/All (Admin)
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> All()
        {
            var orders = await _orders.GetAllAsync();
            return View(orders);
        }

        // POST: /Orders/Approve/5  (Admin)
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Approve(int id)
        {
            await _orders.ApproveAsync(id);
            TempData["Success"] = "Резервацията е одобрена.";
            return RedirectToAction(nameof(All));
        }

        // POST: /Orders/Reject/5  (Admin)
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Reject(int id)
        {
            await _orders.RejectAsync(id);
            TempData["Success"] = "Резервацията е отхвърлена.";
            return RedirectToAction(nameof(All));
        }

        // ✅ NEW: delete rejected (client + admin)
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteRejected(int id)
        {
            var userId = _userManager.GetUserId(User);
            var isAdmin = User.IsInRole("Admin");

            var ok = await _orders.DeleteRejectedAsync(id, userId!, isAdmin);

            TempData[ok ? "Success" : "Error"] = ok
                ? "Резервацията е премахната."
                : "Не може да премахнете тази резервация.";

            if (isAdmin) return RedirectToAction(nameof(All));
            return RedirectToAction(nameof(My));
        }
    }
}