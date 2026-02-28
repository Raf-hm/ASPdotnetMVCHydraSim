using ASPdotnetMVCHydraSim.Domain.Components;
using ASPdotnetMVCHydraSim.Domain.Simulation;
using Microsoft.AspNetCore.Mvc;

namespace ASPdotnetMVCHydraSim.Controllers
{
    public class SimulationController : Controller
    {
        public IActionResult Run(int id)
        {
            var simulation = BuildSimulation(id);

            simulation.SyncPumpWithResistance();

            simulation.Run();

            ViewBag.MaxPressure = simulation.MaxPressure;

            return View(simulation.Components);
        }

        private HydraulicSimulation BuildSimulation(int id)
        {
            var simulation = new HydraulicSimulation();

            if (id == 0)
            {
                simulation.AddComponent(new Pump());
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
            }
            else if (id == 1)
            {
                simulation.AddComponent(new Pump());
                simulation.AddComponent(new Pipe());
                simulation.AddComponent(new Tank());
            }
            else if (id == 2)
            {
                simulation.AddComponent(new Pump());
                simulation.AddComponent(new Pipe());
                simulation.AddComponent(new Tank());
            }
            else if (id == 3)
            {
                simulation.AddComponent(new Pump());
                simulation.AddComponent(new Pipe());
                simulation.AddComponent(new Tank());
            }
            else if (id == 4)
            {
                simulation.AddComponent(new Pump());
                simulation.AddComponent(new Pipe());
                simulation.AddComponent(new Tank());
            }

            return simulation;
        }
    }
}