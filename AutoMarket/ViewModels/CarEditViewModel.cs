using System.ComponentModel.DataAnnotations;

namespace AutoMarket.ViewModels
{
    public class CarEditViewModel
    {
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

        [Required]
        public string FuelType { get; set; } = "Бензин";

        [Required]
        public string Transmission { get; set; } = "Ръчна";

        public string? Description { get; set; }
        public string? ImageUrl { get; set; }
    }
}
