using HydraSim.DAL.Repositories;
using HydraSim.Web.Models;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;

namespace HydraSim.Web.Controllers
{
    public class HomeController : Controller
    {
        private readonly ISimulationRepository _repo;

        public HomeController(ISimulationRepository repo)
        {
            _repo = repo ?? throw new ArgumentNullException(nameof(repo));
        }

        public IActionResult Index() => View();

        public async Task<IActionResult> Library()
        {
            var simulations = await _repo.ListAsync();
            return View(simulations);
        }

        public IActionResult Builder() => View();

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel
            {
                RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier
            });
        }
    }
}
