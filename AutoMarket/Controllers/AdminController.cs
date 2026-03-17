using AutoMarket.Data;
using AutoMarket.Models;
using AutoMarket.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace AutoMarket.Controllers
{
    [Authorize(Roles = "Admin")]
    public class AdminController : Controller
    {
        private readonly ApplicationDbContext context;

        public AdminController(ApplicationDbContext context)
        {
            this.context = context;
        }

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var model = new AdminDashboardViewModel
            {
                TotalCars = await context.Cars.CountAsync(),
                ActiveCars = await context.Cars.CountAsync(c => !c.IsDeleted && !c.IsSold),
                SoldCars = await context.Cars.CountAsync(c => c.IsSold && !c.IsDeleted),
                DeletedCars = await context.Cars.CountAsync(c => c.IsDeleted),

                TotalOrders = await context.Orders.CountAsync(o => !o.IsDeleted),
                PendingOrders = await context.Orders.CountAsync(o => !o.IsDeleted && o.Status == OrderStatus.Pending),
                ApprovedOrders = await context.Orders.CountAsync(o => !o.IsDeleted && o.Status == OrderStatus.Approved),
                RejectedOrders = await context.Orders.CountAsync(o => !o.IsDeleted && o.Status == OrderStatus.Rejected),

                LatestCars = await context.Cars
                    .AsNoTracking()
                    .Include(c => c.CarModel)
                    .ThenInclude(cm => cm.Make)
                    .OrderByDescending(c => c.Id)
                    .Take(8)
                    .Select(c => new AdminCarRowViewModel
                    {
                        Id = c.Id,
                        Make = c.CarModel.Make.Name,
                        Model = c.CarModel.Name,
                        Year = c.Year,
                        Price = c.Price,
                        IsSold = c.IsSold,
                        IsDeleted = c.IsDeleted
                    })
                    .ToListAsync(),

                LatestOrders = await context.Orders
                    .AsNoTracking()
                    .Where(o => !o.IsDeleted)
                    .Include(o => o.Car)
                    .ThenInclude(c => c.CarModel)
                    .ThenInclude(cm => cm.Make)
                    .OrderByDescending(o => o.CreatedOn)
                    .Take(8)
                    .Select(o => new AdminOrderRowViewModel
                    {
                        Id = o.Id,
                        CarName = o.Car.CarModel.Make.Name + " " + o.Car.CarModel.Name,
                        ClientName = o.ContactFirstName + " " + o.ContactLastName,
                        Phone = o.ContactPhone,
                        StatusText = o.Status == OrderStatus.Pending
                            ? "Чакаща"
                            : o.Status == OrderStatus.Approved
                                ? "Одобрена"
                                : "Отхвърлена",
                        CreatedOn = o.CreatedOn
                    })
                    .ToListAsync()
            };

            return View(model);
        }
    }
}