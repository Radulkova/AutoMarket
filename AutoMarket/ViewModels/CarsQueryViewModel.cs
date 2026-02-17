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

        public string? Sort { get; set; }

        // Paging
        public int Page { get; set; } = 1;
        public int PageSize { get; set; } = 9;

        // Dropdown data
        public List<SelectListItem> Makes { get; set; } = new();
        public List<SelectListItem> CarModels { get; set; } = new();

        // Results
        public IEnumerable<Models.Car> Cars { get; set; } = new List<Models.Car>();
        public int TotalCount { get; set; }

        public int TotalPages => PageSize <= 0 ? 1 : (int)Math.Ceiling((double)TotalCount / PageSize);
    }
}
