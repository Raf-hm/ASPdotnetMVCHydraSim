using ASPdotnetMVCHydraSim.Domain.Components;
using System.Collections.Generic;
using System.Linq;

namespace ASPdotnetMVCHydraSim.Domain.Simulation
{
    public class HydraulicSimulation
    {
        private List<HydraulicComponent> _components;

        // Wordt gebruikt in je View
        public IReadOnlyList<HydraulicComponent> Components => _components;

        // Veilig: crasht niet als er geen pump is
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
            // Zorg dat pomp altijd gelijk is aan totale weerstand
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