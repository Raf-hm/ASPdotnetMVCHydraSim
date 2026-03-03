using ASPdotnetMVCHydraSim.Domain.Components;
using ASPdotnetMVCHydraSim.Domain.Simulation;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace ASPdotnetMVCHydraSim.Controllers
{
    public class SimulationController : Controller
    {
        private const string SessionKey = "SimulationComponents";

        public IActionResult Run(int id)
        {
            var json = HttpContext.Session.GetString(SessionKey);
            HydraulicSimulation simulation;

            if (string.IsNullOrEmpty(json))
            {
                simulation = BuildSimulation(id);
                SaveToSession(simulation);
            }
            else
            {
                simulation = LoadFromSession();
            }

            simulation.Run();

            ViewBag.MaxPressure = simulation.MaxPressure;

            return View(simulation.Components);
        }

        [HttpPost]
        public IActionResult UpdateResistancePressureDrop(int simulationId, int componentId, int newPressureDrop)
        {
            var simulation = LoadFromSession() ?? BuildSimulation(simulationId);

            var component = simulation.Components.FirstOrDefault(c => c.ComponentId == componentId);

            if (component is Resistance resistance)
            {
                resistance.PressureDrop = newPressureDrop;
            }

            SaveToSession(simulation);

            simulation.Run();

            ViewBag.MaxPressure = simulation.MaxPressure;

            return View("Run", simulation.Components);
        }

        private void SaveToSession(HydraulicSimulation simulation)
        {
            var settings = new JsonSerializerSettings
            {
                TypeNameHandling = TypeNameHandling.All
            };
            var json = JsonConvert.SerializeObject(simulation.Components, settings);
            HttpContext.Session.SetString(SessionKey, json);
        }

        [HttpPost]
        public IActionResult Reset(int id)
        {
            HttpContext.Session.Remove(SessionKey);
            return RedirectToAction("Run", new { id });
        }

        private HydraulicSimulation LoadFromSession()
        {
            var json = HttpContext.Session.GetString(SessionKey);
            if (string.IsNullOrEmpty(json)) return null;

            var settings = new JsonSerializerSettings
            {
                TypeNameHandling = TypeNameHandling.All
            };

            var components = JsonConvert.DeserializeObject<List<HydraulicComponent>>(json, settings);
            var simulation = new HydraulicSimulation();

            foreach (var component in components)
                simulation.AddComponent(component);

            return simulation;
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
            else
            {
                simulation.AddComponent(new Pump());
                simulation.AddComponent(new Pipe());
                simulation.AddComponent(new Tank());
            }

            return simulation;
        }
    }
}