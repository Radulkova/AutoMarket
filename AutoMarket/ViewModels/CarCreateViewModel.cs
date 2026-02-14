using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace AutoMarket.ViewModels
{
    public class CarCreateViewModel
    {
        [Required(ErrorMessage = "Моля, изберете модел.")]
        [Range(1, int.MaxValue, ErrorMessage = "Моля, изберете модел от списъка.")]
        public int CarModelId { get; set; }

        public List<SelectListItem> CarModels { get; set; } = new();

        [Range(1950, 2100, ErrorMessage = "Годината трябва да е между 1950 и 2100.")]
        public int Year { get; set; }

        [Range(0, 2000000, ErrorMessage = "Цената трябва да е между 0 и 2 000 000.")]
        public decimal Price { get; set; }

        [Range(0, 2000000, ErrorMessage = "Пробегът трябва да е между 0 и 2 000 000.")]
        public int MileageKm { get; set; }

        [Range(600, 10000, ErrorMessage = "Кубатурата трябва да е между 600 и 10 000.")]
        public int EngineCapacityCc { get; set; }

        [Range(1, 3000, ErrorMessage = "Мощността трябва да е между 1 и 3000.")]
        public int HorsePower { get; set; }

        [Required]
        public string FuelType { get; set; } = "Бензин";

        [Required]
        public string Transmission { get; set; } = "Ръчна";

        public string? Description { get; set; }
        public string? ImageUrl { get; set; }
    }
}
