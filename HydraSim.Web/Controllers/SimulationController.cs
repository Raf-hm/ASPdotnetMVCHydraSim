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

        public async Task<IActionResult> Run(int id)
        {
            var simulation = await _repo.LoadAsync(id);
            if (simulation == null) return NotFound();

            simulation.Run();

            ViewBag.MaxPressure = simulation.MaxPressure;
            ViewBag.SimulationId = id;
            return View(simulation.Components);
        }

        [HttpPost]
        public async Task<IActionResult> UpdateResistancePressureDrop(int simulationId, int componentId, int newPressureDrop)
        {
            var simulation = await _repo.LoadAsync(simulationId);
            if (simulation == null) return NotFound();

            var component = simulation.Components.FirstOrDefault(c => c.ComponentId == componentId);
            if (component is Resistance resistance)
            {
                resistance.PressureDrop = newPressureDrop;
                await _repo.UpdateComponentAsync(resistance);
            }

            simulation.Run();

            ViewBag.MaxPressure = simulation.MaxPressure;
            ViewBag.SimulationId = simulationId;
            return View("Run", simulation.Components);
        }

        [HttpPost]
        public async Task<IActionResult> UpdateMotorRequiredPressure(int simulationId, int componentId, int newRequiredPressure)
        {
            var simulation = await _repo.LoadAsync(simulationId);
            if (simulation == null) return NotFound();

            var component = simulation.Components.FirstOrDefault(c => c.ComponentId == componentId);
            if (component is Motor motor)
            {
                motor.RequiredPressure = newRequiredPressure;
                await _repo.UpdateComponentAsync(motor);
            }

            simulation.Run();

            ViewBag.MaxPressure = simulation.MaxPressure;
            ViewBag.SimulationId = simulationId;
            return View("Run", simulation.Components);
        }

        [HttpPost]
        public async Task<IActionResult> UpdateReliefValveMaxPressure(int simulationId, int componentId, int newMaxPressure)
        {
            var simulation = await _repo.LoadAsync(simulationId);
            if (simulation == null) return NotFound();

            var component = simulation.Components.FirstOrDefault(c => c.ComponentId == componentId);
            if (component is ReliefValve rv)
            {
                rv.MaxPressure = newMaxPressure;
                await _repo.UpdateComponentAsync(rv);
            }

            simulation.Run();

            ViewBag.MaxPressure = simulation.MaxPressure;
            ViewBag.SimulationId = simulationId;
            return View("Run", simulation.Components);
        }

        [HttpPost]
        public async Task<IActionResult> Reset(int id)
        {
            var success = await _repo.ResetAsync(id);
            if (!success) return NotFound();

            return RedirectToAction("Run", new { id });
        }
    }
}
