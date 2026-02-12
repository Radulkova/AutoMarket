using System.ComponentModel.DataAnnotations;

namespace AutoMarket.Models
{
    public class Car
    {
        public int Id { get; set; }

        [Required]
        public int CarModelId { get; set; }
        public CarModel CarModel { get; set; } = null!;

        [Range(1950, 2100)]
        public int Year { get; set; }

        [Range(0, 2000000)]
        public decimal Price { get; set; }

        [Range(0, 2000000)]
        public int MileageKm { get; set; }

        [Range(600, 10000)]
        public int EngineCapacityCc { get; set; } 

        [Range(1, 3000)]
        public int HorsePower { get; set; } 

        [Required, MaxLength(30)]
        public string FuelType { get; set; } = "Petrol";

        [Required, MaxLength(30)]
        public string Transmission { get; set; } = "Manual";

        [MaxLength(500)]
        public string? Description { get; set; }

        [MaxLength(300)]
        public string? ImageUrl { get; set; }

        
        [Required]
        public string SellerId { get; set; } = null!;
    }
}

