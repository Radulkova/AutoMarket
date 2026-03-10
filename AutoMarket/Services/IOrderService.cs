using AutoMarket.Models;

namespace AutoMarket.Services
{
    public interface IOrderService
    {
        Task CreateAsync(int carId, string buyerId, string firstName, string lastName, string phone, string? note);

        Task<List<Order>> GetMyOrdersAsync(string buyerId);
        Task<List<Order>> GetPendingAsync();
        Task<List<Order>> GetAllAsync();

        Task<int> GetPendingCountAsync();

        Task ApproveAsync(int orderId);
        Task RejectAsync(int orderId);

        // ✅ NEW: soft delete rejected
        Task<bool> DeleteRejectedAsync(int orderId, string userId, bool isAdmin);
    }
}