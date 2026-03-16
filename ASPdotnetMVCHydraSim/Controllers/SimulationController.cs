using ASPdotnetMVCHydraSim.Domain.Components;
using ASPdotnetMVCHydraSim.Domain.Simulation;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace ASPdotnetMVCHydraSim.Controllers
{
    public class SimulationController : Controller
    {
        private string GetSessionKey(int id) => $"SimulationComponents_{id}";

        public IActionResult Run(int id)
        {
            var sessionKey = GetSessionKey(id);
            var json = HttpContext.Session.GetString(sessionKey);
            HydraulicSimulation simulation;

            if (string.IsNullOrEmpty(json))
            {
                simulation = BuildSimulation(id);
                SaveToSession(simulation, id);
            }
            else
            {
                simulation = LoadFromSession(id);
            }

            simulation.Run();
            ViewBag.MaxPressure = simulation.MaxPressure;

            return View(simulation.Components);
        }

        [HttpPost]
        public IActionResult UpdateResistancePressureDrop(int simulationId, int componentId, int newPressureDrop)
        {
            var simulation = LoadFromSession(simulationId) ?? BuildSimulation(simulationId);

            var component = simulation.Components.FirstOrDefault(c => c.ComponentId == componentId);

            if (component is Resistance resistance)
            {
                resistance.PressureDrop = newPressureDrop;
            }

            SaveToSession(simulation, simulationId);

            simulation.Run();
            ViewBag.MaxPressure = simulation.MaxPressure;

            return View("Run", simulation.Components);
        }

        [HttpPost]
        public IActionResult Reset(int id)
        {
            HttpContext.Session.Remove(GetSessionKey(id));
            return RedirectToAction("Run", new { id });
        }

        private void SaveToSession(HydraulicSimulation simulation, int id)
        {
            var settings = new JsonSerializerSettings
            {
                TypeNameHandling = TypeNameHandling.All
            };

            var json = JsonConvert.SerializeObject(simulation.Components, settings);
            HttpContext.Session.SetString(GetSessionKey(id), json);
        }

        private HydraulicSimulation LoadFromSession(int id)
        {
            var json = HttpContext.Session.GetString(GetSessionKey(id));
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
                //simulation.AddComponent(new Pump());
                //simulation.AddComponent(new Pipe());
                //simulation.AddComponent(new PressureGauge());
                //simulation.AddComponent(new Pipe());
                //simulation.AddComponent(new Resistance { PressureDrop = 300 });
                //simulation.AddComponent(new Pipe());
                //simulation.AddComponent(new PressureGauge());
                //simulation.AddComponent(new Pipe());
                //simulation.AddComponent(new Resistance { PressureDrop = 200 });
                //simulation.AddComponent(new Pipe());
                //simulation.AddComponent(new PressureGauge());
                //simulation.AddComponent(new Pipe());
                //simulation.AddComponent(new Resistance { PressureDrop = 100 });
                //simulation.AddComponent(new Pipe());
                //simulation.AddComponent(new PressureGauge());
                //simulation.AddComponent(new Pipe());
                //simulation.AddComponent(new Tank());



                simulation.AddComponent(new Pump            { CX = 1, CY = 1 });
                simulation.AddComponent(new Pipe            { CX = 1, CY = 2 });
                simulation.AddComponent(new PressureGauge   { CX = 1, CY = 3 });
                simulation.AddComponent(new Pipe            { CX = 1, CY = 4 });
                simulation.AddComponent(new Resistance      { CX = 1, CY = 5, PressureDrop = 300 });
                simulation.AddComponent(new Pipe            { CX = 1, CY = 6 });
                simulation.AddComponent(new PressureGauge   { CX = 1, CY = 7 });
                simulation.AddComponent(new Pipe            { CX = 2, CY = 7, rotated = true});
                simulation.AddComponent(new Resistance      { CX = 3, CY = 7, PressureDrop = 200 });
                simulation.AddComponent(new Pipe            { CX = 4, CY = 7, rotated = true });
                simulation.AddComponent(new PressureGauge   { CX = 5, CY = 7 });
                simulation.AddComponent(new Pipe            { CX = 5, CY = 6 });
                simulation.AddComponent(new Resistance      { CX = 5, CY = 5, PressureDrop = 100 });
                simulation.AddComponent(new Pipe            { CX = 5, CY = 4 });
                simulation.AddComponent(new PressureGauge   { CX = 5, CY = 3 });
                simulation.AddComponent(new Pipe            { CX = 5, CY = 2 });
                simulation.AddComponent(new Tank            { CX = 5, CY = 1 });
                simulation.AddComponent(new Pipe            { CX = 4, CY = 1, rotated = true});
                simulation.AddComponent(new Pipe            { CX = 3, CY = 1, rotated = true});
                simulation.AddComponent(new Pipe            { CX = 2, CY = 1, rotated = true});
            }
            else if (id == 1)
            {
                simulation.AddComponent(new Pump());
                simulation.AddComponent(new Pipe());
                simulation.AddComponent(new Resistance { PressureDrop = 500 });
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