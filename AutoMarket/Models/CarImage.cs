using AutoMarket.Models;
using System.ComponentModel.DataAnnotations;

namespace AutoMarket.Models
{
    public class CarImage
    {
        public int Id { get; set; }

        public int CarId { get; set; }
        public Car Car { get; set; } = null!;

        [Required]
        [MaxLength(500)]
        public string Url { get; set; } = string.Empty;

        public bool IsMain { get; set; }
        public int SortOrder { get; set; }
    }
}
