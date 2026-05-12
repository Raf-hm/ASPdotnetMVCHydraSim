using HydraSim.Domain.Components;
using HydraSim.Domain.Simulation;
using Newtonsoft.Json;

namespace HydraSim.DAL.Repositories
{
    public class SimulationRepository : ISimulationRepository
    {
        private string GetSessionKey(int id) => $"SimulationComponents_{id}";

        public void SaveToSession(HydraulicSimulation simulation, int id, Action<string, string> sessionSetter)
        {
            var settings = new JsonSerializerSettings { TypeNameHandling = TypeNameHandling.All };
            var json = JsonConvert.SerializeObject(simulation.Components, settings);
            sessionSetter(GetSessionKey(id), json);
        }

        public HydraulicSimulation? LoadFromSession(int id, Func<string, string?> sessionGetter)
        {
            var json = sessionGetter(GetSessionKey(id));
            if (string.IsNullOrEmpty(json)) return null;

            var settings = new JsonSerializerSettings { TypeNameHandling = TypeNameHandling.All };
            var components = JsonConvert.DeserializeObject<List<HydraulicComponent>>(json, settings);

            var simulation = new HydraulicSimulation();
            foreach (var component in components)
                simulation.AddComponent(component);

            simulation.AutoConnect();
            return simulation;
        }

        public HydraulicSimulation BuildSimulation(int id)
        {
            var simulation = new HydraulicSimulation();

            if (id == 1)
            {
                simulation.AddComponent(new Pump(1, 1, 0));
                simulation.AddComponent(new Pipe(1, 2));
                simulation.AddComponent(new PressureGauge(1, 3));
                simulation.AddComponent(new Pipe(1, 4));
                simulation.AddComponent(new Resistance(1, 5, 300));
                simulation.AddComponent(new Pipe(1, 6));
                simulation.AddComponent(new PressureGauge(1, 7));
                simulation.AddComponent(new Pipe(2, 7, rotation: 90));
                simulation.AddComponent(new Resistance(3, 7, 200));
                simulation.AddComponent(new Pipe(4, 7, rotation: 90));
                simulation.AddComponent(new PressureGauge(5, 7));
                simulation.AddComponent(new Pipe(5, 6));
                simulation.AddComponent(new Resistance(5, 5, 100));
                simulation.AddComponent(new Pipe(5, 4));
                simulation.AddComponent(new PressureGauge(5, 3));
                simulation.AddComponent(new Pipe(5, 2));
                simulation.AddComponent(new Tank(5, 1));
                simulation.AddComponent(new Pipe(4, 1, rotation: 90));
                simulation.AddComponent(new Pipe(3, 1, rotation: 90));
                simulation.AddComponent(new Pipe(2, 1, rotation: 90));
            }
            else if (id == 2)
            {
                simulation.AddComponent(new Pump(1, 1, 0));
                simulation.AddComponent(new Pipe(1, 2));
                simulation.AddComponent(new PressureGauge(1, 3));
                simulation.AddComponent(new Pipe(1, 4));
                simulation.AddComponent(new Pipe(2, 3, rotation: 90));
                simulation.AddComponent(new Motor(3, 3, 200));
                simulation.AddComponent(new Pipe(3, 4));
                simulation.AddComponent(new PressureGauge(3, 5));
                simulation.AddComponent(new Pipe(3, 6));
                simulation.AddComponent(new ReliefValve(1, 5, 300));
                simulation.AddComponent(new Pipe(1, 6));
                simulation.AddComponent(new PressureGauge(1, 7));
                simulation.AddComponent(new Pipe(2, 7, rotation: 90));
                simulation.AddComponent(new Tank(3, 7));
                simulation.AddComponent(new Pipe(4, 7, rotation: 90));
                simulation.AddComponent(new Pipe(5, 7, isCorner: true));
                simulation.AddComponent(new Pipe(5, 6));
                simulation.AddComponent(new Pipe(5, 5));
                simulation.AddComponent(new Pipe(5, 4));
                simulation.AddComponent(new Pipe(5, 3));
                simulation.AddComponent(new Pipe(5, 2));
                simulation.AddComponent(new Pipe(5, 1, isCorner: true, rotation: 90));
                simulation.AddComponent(new Pipe(4, 1, rotation: 90));
                simulation.AddComponent(new Pipe(3, 1, rotation: 90));
                simulation.AddComponent(new Pipe(2, 1, rotation: 90));
            }

            simulation.AutoConnect();
            return simulation;
        }
    }
}

