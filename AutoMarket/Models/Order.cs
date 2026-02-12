using System.ComponentModel.DataAnnotations;

namespace AutoMarket.Models
{
    public class Order
    {
        public int Id { get; set; }

        [Required]
        public int CarId { get; set; }
        public Car Car { get; set; } = null!;

        [Required]
        public string BuyerId { get; set; } = null!;

        public DateTime CreatedOn { get; set; } = DateTime.UtcNow;
    }
}

