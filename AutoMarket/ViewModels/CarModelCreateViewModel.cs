using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace AutoMarket.ViewModels
{
    public class CarModelCreateViewModel
    {
        // ✅ ако избираш вече съществуваща марка
        public int? MakeId { get; set; }

        // ✅ ако пишеш нова марка
        [StringLength(40, MinimumLength = 2, ErrorMessage = "Марката трябва да е между 2 и 40 символа.")]
        public string? NewMakeName { get; set; }

        public List<SelectListItem> Makes { get; set; } = new();

        [Required(ErrorMessage = "Моля, въведете модел.")]
        [StringLength(60, MinimumLength = 2, ErrorMessage = "Моделът трябва да е между 2 и 60 символа.")]
        public string Name { get; set; } = string.Empty;
    }
}
