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

            // Als geen session -> nieuwe simulatie bouwen
            if (string.IsNullOrEmpty(json))
            {
                simulation = BuildSimulation(id);
                SaveToSession(simulation, id);
            }
            else
            {
                // Anders laden uit session
                simulation = LoadFromSession(id);
            }

            simulation.Run();

            // id doorgeven aan view
            ViewBag.MaxPressure = simulation.MaxPressure;
            ViewBag.SimulationId = id;

            return View(simulation.Components);
        }

        [HttpPost]
        public IActionResult UpdateResistancePressureDrop(int simulationId, int componentId, int newPressureDrop)
        {
            // juiste simulatie laden
            var simulation = LoadFromSession(simulationId) ?? BuildSimulation(simulationId);

            var component = simulation.Components.FirstOrDefault(c => c.ComponentId == componentId);

            // weerstand aanpassen
            if (component is Resistance resistance)
            {
                resistance.PressureDrop = newPressureDrop;
            }

            // opslaan + opnieuw runnen
            SaveToSession(simulation, simulationId);

            simulation.Run();
            ViewBag.MaxPressure = simulation.MaxPressure;
            ViewBag.SimulationId = simulationId;

            return View("Run", simulation.Components);
        }

        [HttpPost]
        public IActionResult Reset(int id)
        {
            // reset: verwijder uit session
            HttpContext.Session.Remove(GetSessionKey(id));

            // reload juiste simulatie
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
                simulation.AddComponent(new Pump            { CX = 1, CY = 1 });
                simulation.AddComponent(new Pipe            { CX = 1, CY = 2 });
                simulation.AddComponent(new PressureGauge   { CX = 1, CY = 3 });
                simulation.AddComponent(new Pipe            { CX = 1, CY = 4 });
                simulation.AddComponent(new Resistance      { CX = 1, CY = 5, PressureDrop = 300 });
                simulation.AddComponent(new Pipe            { CX = 1, CY = 6 });
                simulation.AddComponent(new PressureGauge   { CX = 1, CY = 7 });
                simulation.AddComponent(new Pipe            { CX = 2, CY = 7, rotated = true });
                simulation.AddComponent(new Resistance      { CX = 3, CY = 7, PressureDrop = 200 });
                simulation.AddComponent(new Pipe            { CX = 4, CY = 7, rotated = true });
                simulation.AddComponent(new PressureGauge   { CX = 5, CY = 7 });
                simulation.AddComponent(new Pipe            { CX = 5, CY = 6 });
                simulation.AddComponent(new Resistance      { CX = 5, CY = 5, PressureDrop = 100 });
                simulation.AddComponent(new Pipe            { CX = 5, CY = 4 });
                simulation.AddComponent(new PressureGauge   { CX = 5, CY = 3 });
                simulation.AddComponent(new Pipe            { CX = 5, CY = 2 });
                simulation.AddComponent(new Tank            { CX = 5, CY = 1 });
                simulation.AddComponent(new Pipe            { CX = 4, CY = 1, rotated = true });
                simulation.AddComponent(new Pipe            { CX = 3, CY = 1, rotated = true });
                simulation.AddComponent(new Pipe            { CX = 2, CY = 1, rotated = true });
            }
            else if (id == 1)
            {
                simulation.AddComponent(new Pump            { CX = 1, CY = 1 });
                simulation.AddComponent(new Pipe            { CX = 1, CY = 2 });
                simulation.AddComponent(new PressureGauge   { CX = 1, CY = 3 });
                simulation.AddComponent(new Pipe            { CX = 1, CY = 4 });
                simulation.AddComponent(new Pipe            { CX = 2, CY = 3, rotated = true });
                simulation.AddComponent(new Motor           { CX = 3, CY = 3, RequiredPressure = 400 });
                simulation.AddComponent(new Pipe            { CX = 3, CY = 4 });
                simulation.AddComponent(new PressureGauge   { CX = 3, CY = 5 });
                simulation.AddComponent(new Pipe            { CX = 3, CY = 6 });
                simulation.AddComponent(new ReliefValve     { CX = 1, CY = 5, MaxPressure = 300 });
                simulation.AddComponent(new Pipe            { CX = 1, CY = 6 });
                simulation.AddComponent(new PressureGauge   { CX = 1, CY = 7 });
                simulation.AddComponent(new Pipe            { CX = 2, CY = 7, rotated = true });
                simulation.AddComponent(new Tank            { CX = 3, CY = 7 });
            }

            return simulation;
        }
    }
}