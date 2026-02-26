using AutoMarket.Models;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace AutoMarket.ViewModels
{
    public class HomeViewModel
    {
        public int? MakeId { get; set; }
        public int? CarModelId { get; set; }
        public string? FuelType { get; set; }
        public string? Transmission { get; set; }

        public List<SelectListItem> Makes { get; set; } = new();
        public List<SelectListItem> CarModels { get; set; } = new();

        public List<Car> LatestCars { get; set; } = new();
    }
}

