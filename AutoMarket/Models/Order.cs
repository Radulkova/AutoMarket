using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;

namespace AutoMarket.Models
{
    public class Order
    {
        public int Id { get; set; }

        public int CarId { get; set; }
        public Car Car { get; set; } = null!;

        public string BuyerId { get; set; } = null!;
        public IdentityUser Buyer { get; set; } = null!;

        public DateTime CreatedOn { get; set; } = DateTime.UtcNow;

        public OrderStatus Status { get; set; } = OrderStatus.Pending;

        // Contact info (from modal)
        public string ContactFirstName { get; set; } = null!;
        public string ContactLastName { get; set; } = null!;
        public string ContactPhone { get; set; } = null!;
        public string? Note { get; set; }

        // ✅ Soft delete (hide from lists)
        public bool IsDeleted { get; set; } = false;
    }

    public enum OrderStatus
    {
        [Display(Name = "Чакаща")]
        Pending = 0,

        [Display(Name = "Одобрена")]
        Approved = 1,

        [Display(Name = "Отхвърлена")]
        Rejected = 2
    }
}