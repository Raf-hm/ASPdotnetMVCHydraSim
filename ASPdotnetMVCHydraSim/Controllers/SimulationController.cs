using ASPdotnetMVCHydraSim.Domain.Components;
using Microsoft.AspNetCore.Mvc;
using YourProjectName.Domain.Simulation;

namespace ASPdotnetMVCHydraSim.Controllers
{
    public class SimulationController : Controller
    {
        public IActionResult Run(int id)
        {
            var simulation = BuildSimulation(id);

            simulation.Run();

            return View("Simulation", simulation.Results);
        }

        private HydraulicSimulation BuildSimulation(int id)
        {
            var simulation = new HydraulicSimulation();

            if (id == 1)
            {
                simulation.AddComponent(new Pump { PressureOutput = 600 });
                simulation.AddComponent(new Resistance { PressureDrop = 300 });
                simulation.AddComponent(new Tank());
            }
            else if (id == 2)
            {
                simulation.AddComponent(new Pump { PressureOutput = 800 });
                simulation.AddComponent(new Resistance { PressureDrop = 200 });
                simulation.AddComponent(new Tank());
            }

            return simulation;
        }
    }
}
