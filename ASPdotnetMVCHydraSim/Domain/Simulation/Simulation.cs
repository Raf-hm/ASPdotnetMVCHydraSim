using ASPdotnetMVCHydraSim.Domain.Components;
using System.Collections.Generic;
using System.Linq;

namespace ASPdotnetMVCHydraSim.Domain.Simulation
{
    public class HydraulicSimulation
    {
        private List<HydraulicComponent> _components;

        public IReadOnlyList<HydraulicComponent> Components => _components;

        public int MaxPressure =>
            _components.OfType<Pump>().FirstOrDefault()?.PressureOutput ?? 0;

        public HydraulicSimulation()
        {
            _components = new List<HydraulicComponent>();
        }

        public void AddComponent(HydraulicComponent component)
        {
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

            int currentPressure = 0;

            foreach (var component in _components)
            {
                currentPressure = component.Process(currentPressure);
                component.CurrentPressure = currentPressure;
            }
        }
    }
}