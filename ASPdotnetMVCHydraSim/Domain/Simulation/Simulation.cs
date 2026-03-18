using ASPdotnetMVCHydraSim.Domain.Components;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Linq;

namespace ASPdotnetMVCHydraSim.Domain.Simulation
{
    public class HydraulicSimulation
    {
        public List<HydraulicComponent> _components;

        public void BuildConnections()
        {
            foreach (var comp in _components)
            {
                if (comp is Pump pump)
                {
                    var rightNeighbor = _components.FirstOrDefault(c =>
                        c.CX == comp.CX && c.CY == comp.CY + 1);

                    if (rightNeighbor != null)
                        comp.Outputs.Add(rightNeighbor);

                    continue;
                }

                comp.Outputs.Clear();

                var neighbors = _components.Where(c =>
                    (c.CX == comp.CX && Math.Abs(c.CY - comp.CY) == 1) ||
                    (c.CY == comp.CY && Math.Abs(c.CX - comp.CX) == 1)
                );

                foreach (var n in neighbors)
                {
                    comp.Outputs.Add(n);
                }
            }
        }

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

        public int GetTotalResistance()
        {
            return _components
                .OfType<Resistance>()
                .Sum(r => r.PressureDrop);
        }

        public void SyncPumpWithResistance()
        {
            int totalResistance = GetTotalResistance();

            var pump = _components.OfType<Pump>().FirstOrDefault();

            if (pump != null)
            {
                pump.PressureOutput = totalResistance;
            }
        }

        public void Run()
        {
            SyncPumpWithResistance();

            BuildConnections();

            var pump = _components.OfType<Pump>().First();

            var queue = new Queue<(HydraulicComponent comp, int pressure)>();
            var visited = new Dictionary<int, int>();

            queue.Enqueue((pump, pump.PressureOutput));

            while (queue.Count > 0)
            {
                var (comp, pressure) = queue.Dequeue();

                int newPressure = comp.Process(pressure);

                if (visited.ContainsKey(comp.ComponentId) &&
                    visited[comp.ComponentId] >= newPressure)
                    continue;

                visited[comp.ComponentId] = newPressure;
                comp.CurrentPressure = newPressure;

                foreach (var next in comp.Outputs)
                {
                    queue.Enqueue((next, newPressure));
                }
            }
        }
    }
}