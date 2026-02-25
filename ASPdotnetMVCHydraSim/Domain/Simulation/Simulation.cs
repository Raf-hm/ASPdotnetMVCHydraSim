using ASPdotnetMVCHydraSim.Domain.Components;
using System.Collections.Generic;

namespace YourProjectName.Domain.Simulation
{
    public class HydraulicSimulation
    {
        private List<HydraulicComponent> _components;

        public List<string> Results { get; private set; }

        public HydraulicSimulation()
        {
            _components = new List<HydraulicComponent>();
            Results = new List<string>();
        }

        public void AddComponent(HydraulicComponent component)
        {
            _components.Add(component);
        }

        public void Run()
        {
            Results.Clear();

            int currentPressure = 0;

            foreach (var component in _components)
            {
                currentPressure = component.Process(currentPressure);

                Results.Add(component.GetInfo());
            }
        }
    }
}