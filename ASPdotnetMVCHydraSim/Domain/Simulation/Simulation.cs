using ASPdotnetMVCHydraSim.Domain.Components;

namespace ASPdotnetMVCHydraSim.Domain.Simulation
{
    public class HydraulicSimulation
    {
        public List<HydraulicComponent> _components;

        public IReadOnlyList<HydraulicComponent> Components => _components;

        public int MaxPressure => _components.OfType<Pump>().FirstOrDefault()?.PressureOutput ?? 0;

        public HydraulicSimulation()
        {
            _components = new List<HydraulicComponent>();
        }

        public void AddComponent(HydraulicComponent component)
        {
            component.ComponentId = _components.Count;
            _components.Add(component);
        }

        public void SyncPump()
        {
            var pump = _components.OfType<Pump>().FirstOrDefault();
            var motor = _components.OfType<Motor>().FirstOrDefault();
            var reliefValve = _components.OfType<ReliefValve>().FirstOrDefault();

            if (pump == null) return;

            if (motor == null)
            {
                pump.PressureOutput = _components.OfType<Resistance>().Sum(r => r.PressureDrop);
                return;
            }

            if (reliefValve != null)
            {
                if (motor.RequiredPressure >= reliefValve.MaxPressure)
                {
                    reliefValve.IsOpen = true;
                    pump.PressureOutput = 0;
                }
                else
                {
                    reliefValve.IsOpen = false;
                    pump.PressureOutput = motor.RequiredPressure;
                }
                return;
            }

            pump.PressureOutput = motor.RequiredPressure;
        }

        public void Run()
        {
            SyncPump();

            var pump = _components.OfType<Pump>().First();
            var queue = new Queue<(HydraulicComponent comp, int pressure)>();
            var visited = new HashSet<int>();

            queue.Enqueue((pump, pump.PressureOutput));

            while (queue.Count > 0)
            {
                var (comp, pressure) = queue.Dequeue();

                if (visited.Contains(comp.ComponentId)) continue;
                visited.Add(comp.ComponentId);

                int outPressure = comp.Process(pressure);
                comp.CurrentPressure = pressure;

                foreach (var next in comp.Outputs)
                    queue.Enqueue((next, outPressure));
            }
        }
    }
}