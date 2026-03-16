using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;

namespace AutoMarket.Models
{
    public class Favorite
    {
        [Required]
        public string UserId { get; set; } = null!;

        public IdentityUser User { get; set; } = null!;

        [Required]
        public int CarId { get; set; }

        public Car Car { get; set; } = null!;

        public DateTime CreatedOn { get; set; } = DateTime.UtcNow;
    }
}