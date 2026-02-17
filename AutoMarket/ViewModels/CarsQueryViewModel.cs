using AutoMarket.Models;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace AutoMarket.ViewModels
{
    public class CarsQueryViewModel
    {
        // Filters
        public int? MakeId { get; set; }
        public int? CarModelId { get; set; }
        public string? FuelType { get; set; }
        public string? Transmission { get; set; }
        public int? YearFrom { get; set; }
        public int? YearTo { get; set; }
        public decimal? PriceFrom { get; set; }
        public decimal? PriceTo { get; set; }

        // Sorting
        public string? Sort { get; set; }

        // Paging (по желание)
        public int Page { get; set; } = 1;
        public int PageSize { get; set; } = 12;

        // Dropdown data
        public List<SelectListItem> Makes { get; set; } = new();
        public List<SelectListItem> CarModels { get; set; } = new();

        // Results
        public IEnumerable<Car> Cars { get; set; } = new List<Car>();
        public int TotalCount { get; set; }
    }
}
