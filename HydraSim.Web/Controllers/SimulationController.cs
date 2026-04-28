using HydraSim.DAL.Repositories;
using HydraSim.Domain.Components;
using Microsoft.AspNetCore.Mvc;

namespace HydraSim.Web.Controllers
{
    public class SimulationController : Controller
    {
        private readonly ISimulationRepository _repo;

        public SimulationController(ISimulationRepository repo)
        {
            _repo = repo ?? throw new ArgumentNullException(nameof(repo));
        }

        private void Save(HydraSim.Domain.Simulation.HydraulicSimulation sim, int id)
            => _repo.SaveToSession(sim, id,
                (key, val) => HttpContext.Session.SetString(key, val));

        private HydraSim.Domain.Simulation.HydraulicSimulation? Load(int id)
            => _repo.LoadFromSession(id,
                key => HttpContext.Session.GetString(key));

        public IActionResult Run(int id)
        {
            var simulation = Load(id) ?? _repo.BuildSimulation(id);
            Save(simulation, id);
            simulation.Run();

            ViewBag.MaxPressure  = simulation.MaxPressure;
            ViewBag.SimulationId = id;
            return View(simulation.Components);
        }

        [HttpPost]
        public IActionResult UpdateResistancePressureDrop(int simulationId, int componentId, int newPressureDrop)
        {
            var simulation = Load(simulationId) ?? _repo.BuildSimulation(simulationId);
            var component  = simulation.Components.FirstOrDefault(c => c.ComponentId == componentId);

            if (component is Resistance resistance)
                resistance.PressureDrop = newPressureDrop;

            Save(simulation, simulationId);
            simulation.Run();

            ViewBag.MaxPressure  = simulation.MaxPressure;
            ViewBag.SimulationId = simulationId;
            return View("Run", simulation.Components);
        }

        [HttpPost]
        public IActionResult UpdateMotorRequiredPressure(int simulationId, int componentId, int newRequiredPressure)
        {
            var simulation = Load(simulationId) ?? _repo.BuildSimulation(simulationId);
            var component  = simulation.Components.FirstOrDefault(c => c.ComponentId == componentId);

            if (component is Motor motor)
                motor.RequiredPressure = newRequiredPressure;

            Save(simulation, simulationId);
            simulation.Run();

            ViewBag.MaxPressure  = simulation.MaxPressure;
            ViewBag.SimulationId = simulationId;
            return View("Run", simulation.Components);
        }

        [HttpPost]
        public IActionResult UpdateReliefValveMaxPressure(int simulationId, int componentId, int newMaxPressure)
        {
            var simulation = Load(simulationId) ?? _repo.BuildSimulation(simulationId);
            var component  = simulation.Components.FirstOrDefault(c => c.ComponentId == componentId);

            if (component is ReliefValve rv)
                rv.MaxPressure = newMaxPressure;

            Save(simulation, simulationId);
            simulation.Run();

            ViewBag.MaxPressure  = simulation.MaxPressure;
            ViewBag.SimulationId = simulationId;
            return View("Run", simulation.Components);
        }

        [HttpPost]
        public IActionResult Reset(int id)
        {
            HttpContext.Session.Remove($"SimulationComponents_{id}");
            return RedirectToAction("Run", new { id });
        }
    }
}
