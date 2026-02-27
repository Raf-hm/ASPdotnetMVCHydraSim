using ASPdotnetMVCHydraSim.Domain.Components;
using ASPdotnetMVCHydraSim.Models;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using YourProjectName.Domain.Simulation;

namespace ASPdotnetMVCHydraSim.Controllers
{
    public class HomeController : Controller
    {
        public IActionResult Index()
        {
            var simulation = new HydraulicSimulation();

            simulation.AddComponent(new Pump { PressureOutput = 600 });
            simulation.AddComponent(new Pipe());
            simulation.AddComponent(new PressureGauge());
            simulation.AddComponent(new Pipe());
            simulation.AddComponent(new Resistance { PressureDrop = 300 });
            simulation.AddComponent(new Pipe());
            simulation.AddComponent(new PressureGauge());
            simulation.AddComponent(new Pipe());
            simulation.AddComponent(new Resistance { PressureDrop = 200 });
            simulation.AddComponent(new Pipe());
            simulation.AddComponent(new PressureGauge());
            simulation.AddComponent(new Pipe());
            simulation.AddComponent(new Resistance { PressureDrop = 100 });
            simulation.AddComponent(new Pipe());
            simulation.AddComponent(new PressureGauge());
            simulation.AddComponent(new Pipe());
            simulation.AddComponent(new Tank());

            simulation.Run();

            return View(simulation.Results);
        }

        public IActionResult Library()
        {
            return View();
        }

        public IActionResult Builder()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
