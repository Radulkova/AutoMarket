using AutoMarket.ViewModels;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace AutoMarket.Services
{
    public interface ICarModelService
    {
        Task<List<SelectListItem>> GetMakesForSelectAsync();
        Task<(bool ok, string? error)> CreateModelAsync(CarModelCreateViewModel model);
    }
}
