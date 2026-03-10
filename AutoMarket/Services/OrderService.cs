using AutoMarket.Data;
using AutoMarket.Models;
using Microsoft.EntityFrameworkCore;

namespace AutoMarket.Services
{
    public class OrderService : IOrderService
    {
        private readonly ApplicationDbContext _context;

        public OrderService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task CreateAsync(int carId, string buyerId, string firstName, string lastName, string phone, string? note)
        {
            var car = await _context.Cars
                .FirstOrDefaultAsync(c => c.Id == carId && !c.IsDeleted && !c.IsSold);

            if (car == null)
                throw new InvalidOperationException("Автомобилът не е наличен.");

            var hasPending = await _context.Orders.AnyAsync(o =>
                o.CarId == carId && o.Status == OrderStatus.Pending && !o.IsDeleted);

            if (hasPending)
                throw new InvalidOperationException("Вече има активна резервация за този автомобил.");

            var order = new Order
            {
                CarId = carId,
                BuyerId = buyerId,
                Status = OrderStatus.Pending,
                ContactFirstName = firstName.Trim(),
                ContactLastName = lastName.Trim(),
                ContactPhone = phone.Trim(),
                Note = string.IsNullOrWhiteSpace(note) ? null : note.Trim(),
                IsDeleted = false
            };

            _context.Orders.Add(order);
            await _context.SaveChangesAsync();
        }

        public async Task<List<Order>> GetMyOrdersAsync(string buyerId)
        {
            return await _context.Orders
                .AsNoTracking()
                .Include(o => o.Car)
                    .ThenInclude(c => c.CarModel)
                        .ThenInclude(cm => cm.Make)
                .Where(o => o.BuyerId == buyerId && !o.IsDeleted)
                .OrderByDescending(o => o.CreatedOn)
                .ToListAsync();
        }

        public async Task<List<Order>> GetPendingAsync()
        {
            return await _context.Orders
                .AsNoTracking()
                .Include(o => o.Car)
                    .ThenInclude(c => c.CarModel)
                        .ThenInclude(cm => cm.Make)
                .Include(o => o.Buyer)
                .Where(o => o.Status == OrderStatus.Pending && !o.IsDeleted) // ✅ NEW
                .OrderBy(o => o.CreatedOn)
                .ToListAsync();
        }

        public async Task<List<Order>> GetAllAsync()
        {
            return await _context.Orders
                .AsNoTracking()
                .Include(o => o.Car)
                    .ThenInclude(c => c.CarModel)
                        .ThenInclude(cm => cm.Make)
                .Include(o => o.Buyer)
                .Where(o => !o.IsDeleted) // ✅ NEW
                .OrderByDescending(o => o.CreatedOn)
                .ToListAsync();
        }

        public async Task<int> GetPendingCountAsync()
        {
            return await _context.Orders
                .CountAsync(o => o.Status == OrderStatus.Pending && !o.IsDeleted); // ✅ NEW
        }

        public async Task ApproveAsync(int orderId)
        {
            var order = await _context.Orders
                .Include(o => o.Car)
                .FirstOrDefaultAsync(o => o.Id == orderId && !o.IsDeleted);

            if (order == null) throw new InvalidOperationException("Резервацията не е намерена.");
            if (order.Status != OrderStatus.Pending) return;

            order.Status = OrderStatus.Approved;
            order.Car.IsSold = true;

            var others = await _context.Orders
                .Where(o => o.CarId == order.CarId
                            && o.Id != order.Id
                            && o.Status == OrderStatus.Pending
                            && !o.IsDeleted)
                .ToListAsync();

            foreach (var o in others)
                o.Status = OrderStatus.Rejected;

            await _context.SaveChangesAsync();
        }

        public async Task RejectAsync(int orderId)
        {
            var order = await _context.Orders
                .FirstOrDefaultAsync(o => o.Id == orderId && !o.IsDeleted);

            if (order == null) throw new InvalidOperationException("Резервацията не е намерена.");
            if (order.Status != OrderStatus.Pending) return;

            order.Status = OrderStatus.Rejected;
            await _context.SaveChangesAsync();
        }

        public async Task<bool> DeleteRejectedAsync(int orderId, string userId, bool isAdmin)
        {
            var order = await _context.Orders.FirstOrDefaultAsync(o => o.Id == orderId);
            if (order == null) return false;

            // only rejected can be "deleted"
            if (order.Status != OrderStatus.Rejected) return false;

            // admin can delete any, client only their own
            if (!isAdmin && order.BuyerId != userId) return false;

            order.IsDeleted = true;
            await _context.SaveChangesAsync();
            return true;
        }
    }
}