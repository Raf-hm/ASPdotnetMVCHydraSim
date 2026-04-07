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
            ViewBag.SimulationId = id;

            return View(simulation.Components);
        }

        [HttpPost]
        public IActionResult UpdateResistancePressureDrop(int simulationId, int componentId, int newPressureDrop)
        {
            var simulation = LoadFromSession(simulationId) ?? BuildSimulation(simulationId);
            var component = simulation.Components.FirstOrDefault(c => c.ComponentId == componentId);

            if (component is Resistance resistance)
                resistance.PressureDrop = newPressureDrop;

            SaveToSession(simulation, simulationId);
            simulation.Run();

            ViewBag.MaxPressure = simulation.MaxPressure;
            ViewBag.SimulationId = simulationId;

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
            var settings = new JsonSerializerSettings { TypeNameHandling = TypeNameHandling.All };
            var json = JsonConvert.SerializeObject(simulation.Components, settings);
            HttpContext.Session.SetString(GetSessionKey(id), json);
        }

        private HydraulicSimulation LoadFromSession(int id)
        {
            var json = HttpContext.Session.GetString(GetSessionKey(id));
            if (string.IsNullOrEmpty(json)) return null;

            var settings = new JsonSerializerSettings { TypeNameHandling = TypeNameHandling.All };
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
                var pump   = new Pump          { CX = 1, CY = 1 };
                var pipe1  = new Pipe          { CX = 1, CY = 2 };
                var gauge1 = new PressureGauge { CX = 1, CY = 3 };
                var pipe2  = new Pipe          { CX = 1, CY = 4 };
                var res1   = new Resistance    { CX = 1, CY = 5, PressureDrop = 300 };
                var pipe3  = new Pipe          { CX = 1, CY = 6 };
                var gauge2 = new PressureGauge { CX = 1, CY = 7 };
                var pipe4  = new Pipe          { CX = 2, CY = 7, Rotation = 90 };
                var res2   = new Resistance    { CX = 3, CY = 7, PressureDrop = 200 };
                var pipe5  = new Pipe          { CX = 4, CY = 7, Rotation = 90 };
                var gauge3 = new PressureGauge { CX = 5, CY = 7 };
                var pipe6  = new Pipe          { CX = 5, CY = 6 };
                var res3   = new Resistance    { CX = 5, CY = 5, PressureDrop = 100 };
                var pipe7  = new Pipe          { CX = 5, CY = 4 };
                var gauge4 = new PressureGauge { CX = 5, CY = 3 };
                var pipe8  = new Pipe          { CX = 5, CY = 2 };
                var tank   = new Tank          { CX = 5, CY = 1 };
                var pipe9  = new Pipe          { CX = 4, CY = 1, Rotation = 90 };
                var pipe10 = new Pipe          { CX = 3, CY = 1, Rotation = 90 };
                var pipe11 = new Pipe          { CX = 2, CY = 1, Rotation = 90 };

                // Hoofdlijn: pump → tank
                pump  .Outputs.Add(pipe1);
                pipe1 .Outputs.Add(gauge1);
                gauge1.Outputs.Add(pipe2);
                pipe2 .Outputs.Add(res1);
                res1  .Outputs.Add(pipe3);
                pipe3 .Outputs.Add(gauge2);
                gauge2.Outputs.Add(pipe4);
                pipe4 .Outputs.Add(res2);
                res2  .Outputs.Add(pipe5);
                pipe5 .Outputs.Add(gauge3);
                gauge3.Outputs.Add(pipe6);
                pipe6 .Outputs.Add(res3);
                res3  .Outputs.Add(pipe7);
                pipe7 .Outputs.Add(gauge4);
                gauge4.Outputs.Add(pipe8);
                pipe8 .Outputs.Add(tank);

                // Retourleiding: tank → pump (-1)
                tank  .Outputs.Add(pipe9);
                pipe9 .Outputs.Add(pipe10);
                pipe10.Outputs.Add(pipe11);
                //pipe11.Outputs.Add(pump); ///////////////////////////////////////////

                simulation.AddComponent(pump);
                simulation.AddComponent(pipe1);
                simulation.AddComponent(gauge1);
                simulation.AddComponent(pipe2);
                simulation.AddComponent(res1);
                simulation.AddComponent(pipe3);
                simulation.AddComponent(gauge2);
                simulation.AddComponent(pipe4);
                simulation.AddComponent(res2);
                simulation.AddComponent(pipe5);
                simulation.AddComponent(gauge3);
                simulation.AddComponent(pipe6);
                simulation.AddComponent(res3);
                simulation.AddComponent(pipe7);
                simulation.AddComponent(gauge4);
                simulation.AddComponent(pipe8);
                simulation.AddComponent(tank);
                simulation.AddComponent(pipe9);
                simulation.AddComponent(pipe10);
                simulation.AddComponent(pipe11);
            }
            else if (id == 1)
            {
                var pump    = new Pump         { CX = 1, CY = 1 };
                var pipe1   = new Pipe         { CX = 1, CY = 2 };
                var gauge1  = new PressureGauge{ CX = 1, CY = 3 };
                var pipe2   = new Pipe         { CX = 1, CY = 4 };
                var pipeRV  = new Pipe         { CX = 2, CY = 3, Rotation = 90 };
                var motor   = new Motor        { CX = 3, CY = 3, RequiredPressure = 200 };
                var pipe3   = new Pipe         { CX = 3, CY = 4 };
                var gauge2  = new PressureGauge{ CX = 3, CY = 5 };
                var pipe4   = new Pipe         { CX = 3, CY = 6 };
                var rv      = new ReliefValve  { CX = 1, CY = 5, MaxPressure = 300 };
                var pipe5   = new Pipe         { CX = 1, CY = 6 };
                var gauge3  = new PressureGauge{ CX = 1, CY = 7 };
                var pipe6   = new Pipe         { CX = 2, CY = 7, Rotation = 90 };
                var tank    = new Tank         { CX = 3, CY = 7 };
                var pipe7   = new Pipe         { CX = 4, CY = 7, Rotation = 90 };
                var pipe8   = new Pipe         { CX = 5, CY = 7, isCorner = true };
                var pipe9   = new Pipe         { CX = 5, CY = 6 };
                var pipe10  = new Pipe         { CX = 5, CY = 5 };
                var pipe11  = new Pipe         { CX = 5, CY = 4 };
                var pipe12  = new Pipe         { CX = 5, CY = 3 };
                var pipe13  = new Pipe         { CX = 5, CY = 2 };
                var pipe14  = new Pipe         { CX = 5, CY = 1, isCorner = true, Rotation = 90 };
                var pipe15  = new Pipe         { CX = 4, CY = 1, Rotation = 90 };
                var pipe16  = new Pipe         { CX = 3, CY = 1, Rotation = 90 };
                var pipe17  = new Pipe         { CX = 2, CY = 1, Rotation = 90 };

                // Splitsing na gauge1: rv open → via rv, rv dicht → via motor
                if (rv.IsOpen)
                {
                    // RV pad: gauge1  rv  pipe5  gauge3  pipe6 → tank
                    pump  .Outputs.Add(pipe1);
                    pipe1 .Outputs.Add(gauge1);
                    gauge1.Outputs.Add(rv);
                    rv    .Outputs.Add(pipe5);
                    pipe5 .Outputs.Add(gauge3);
                    gauge3.Outputs.Add(pipe6);
                    pipe6 .Outputs.Add(tank);
                }
                else
                {
                    // Motor pad: gauge1 → pipeRV → motor → pipe3 → gauge2 → pipe4 → tank
                    pump  .Outputs.Add(pipe1);
                    pipe1 .Outputs.Add(gauge1);
                    gauge1.Outputs.Add(pipeRV);
                    pipeRV.Outputs.Add(motor);
                    motor .Outputs.Add(pipe3);
                    pipe3 .Outputs.Add(gauge2);
                    gauge2.Outputs.Add(pipe4);
                    pipe4 .Outputs.Add(tank);
                }

                // Retourleiding: tank → pump (-1), altijd hetzelfde
                tank  .Outputs.Add(pipe7);
                pipe7 .Outputs.Add(pipe8);
                pipe8 .Outputs.Add(pipe9);
                pipe9 .Outputs.Add(pipe10);
                pipe10.Outputs.Add(pipe11);
                pipe11.Outputs.Add(pipe12);
                pipe12.Outputs.Add(pipe13);
                pipe13.Outputs.Add(pipe14);
                pipe14.Outputs.Add(pipe15);
                pipe15.Outputs.Add(pipe16);
                pipe16.Outputs.Add(pipe17);
                //pipe17.Outputs.Add(pump); ///////////////////////////////////////////

                simulation.AddComponent(pump);
                simulation.AddComponent(pipe1);
                simulation.AddComponent(gauge1);
                simulation.AddComponent(pipe2);
                simulation.AddComponent(pipeRV);
                simulation.AddComponent(motor);
                simulation.AddComponent(pipe3);
                simulation.AddComponent(gauge2);
                simulation.AddComponent(pipe4);
                simulation.AddComponent(rv);
                simulation.AddComponent(pipe5);
                simulation.AddComponent(gauge3);
                simulation.AddComponent(pipe6);
                simulation.AddComponent(tank);
                simulation.AddComponent(pipe7);
                simulation.AddComponent(pipe8);
                simulation.AddComponent(pipe9);
                simulation.AddComponent(pipe10);
                simulation.AddComponent(pipe11);
                simulation.AddComponent(pipe12);
                simulation.AddComponent(pipe13);
                simulation.AddComponent(pipe14);
                simulation.AddComponent(pipe15);
                simulation.AddComponent(pipe16);
                simulation.AddComponent(pipe17);
            }

            return simulation;
        }
    }
}