using AutoMarket.Services;
using AutoMarket.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

public class HomeController : Controller
{
    private readonly ICarService carService;

    public HomeController(ICarService carService)
    {
        this.carService = carService;
    }

    [HttpGet]
    public async Task<IActionResult> Index()
    {
        var makes = await carService.GetMakesAsync();

        var vm = new HomeViewModel
        {
            Makes = makes.Select(m => new SelectListItem(m.name, m.id.ToString())).ToList(),
            LatestCars = await carService.GetLatestAsync(6)
        };

        return View(vm);
    }
}
