using HydraSim.Domain.Components;

namespace HydraSim.Domain.Simulation
{
    public class HydraulicSimulation
    {
        private enum Dir { N, E, S, W }

        public int Id { get; set; }
        public string Name { get; set; } = "";
        public string Description { get; set; } = "";
        public string ImagePath { get; set; } = "";

        public List<HydraulicComponent> Components { get; set; } = new();

        public int MaxPressure => Components.OfType<Pump>().FirstOrDefault()?.PressureOutput ?? 0;

        public HydraulicSimulation()
        {
            Components = new List<HydraulicComponent>();
        }

        public void AddComponent(HydraulicComponent component)
        {
            component.ComponentId = Components.Count;
            Components.Add(component);
        }

        public void SyncPump()
        {
            var pump = Components.OfType<Pump>().FirstOrDefault();
            var motor = Components.OfType<Motor>().FirstOrDefault();
            var reliefValve = Components.OfType<ReliefValve>().FirstOrDefault();

            if (pump == null) return;

            if (motor == null)
            {
                pump.PressureOutput = Components.OfType<Resistance>().Sum(r => r.PressureDrop);
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

        public void AutoConnect()
        {
            foreach (var comp in Components)
                comp.Outputs.Clear();

            var grid = new Dictionary<(int x, int y), HydraulicComponent>();
            foreach (var comp in Components)
                grid[(comp.CX, comp.CY)] = comp;

            var neighbors = new Dictionary<HydraulicComponent, List<HydraulicComponent>>();
            foreach (var comp in Components)
                neighbors[comp] = new List<HydraulicComponent>();

            Dir[] sides = { Dir.N, Dir.E, Dir.S, Dir.W };

            foreach (var comp in Components)
            {
                foreach (var side in sides)
                {
                    if (!HasPort(comp, side)) continue;

                    int nx = comp.CX + DeltaX(side);
                    int ny = comp.CY + DeltaY(side);

                    if (!grid.TryGetValue((nx, ny), out var neighbor)) continue;
                    if (!HasPort(neighbor, Opposite(side))) continue;
                    if (!neighbors[comp].Contains(neighbor)) neighbors[comp].Add(neighbor);
                }
            }

            var pump = Components.OfType<Pump>().FirstOrDefault();
            if (pump == null) return;

            var distance = new Dictionary<HydraulicComponent, int>();
            distance[pump] = 0;

            var queue = new Queue<HydraulicComponent>();
            queue.Enqueue(pump);

            while (queue.Count > 0)
            {
                var current = queue.Dequeue();
                foreach (var n in neighbors[current])
                {
                    if (!distance.ContainsKey(n))
                    {
                        distance[n] = distance[current] + 1;
                        queue.Enqueue(n);
                    }
                }
            }

            foreach (var comp in Components)
            {
                if (!distance.ContainsKey(comp)) continue;

                foreach (var n in neighbors[comp])
                {
                    if (distance.ContainsKey(n) && distance[n] > distance[comp])
                        comp.Outputs.Add(n);
                }
            }
        }

        public void Run()
        {
            SyncPump();

            var pump = Components.OfType<Pump>().First();
            var queue = new Queue<(HydraulicComponent comp, int pressure)>();
            var visited = new HashSet<int>();

            queue.Enqueue((pump, pump.PressureOutput));

            while (queue.Count > 0)
            {
                var (comp, pressure) = queue.Dequeue();

                if (visited.Contains(comp.ComponentId)) continue;
                visited.Add(comp.ComponentId);

                int outPressure = comp.Process(pressure);

                foreach (var next in comp.Outputs)
                    queue.Enqueue((next, outPressure));
            }
        }

        private static Dir Opposite(Dir d)
        {
            if (d == Dir.N) return Dir.S;
            if (d == Dir.S) return Dir.N;
            if (d == Dir.E) return Dir.W;
            return Dir.E;
        }

        private static int DeltaX(Dir d)
        {
            if (d == Dir.E) return 1;
            if (d == Dir.W) return -1;
            return 0;
        }

        private static int DeltaY(Dir d)
        {
            if (d == Dir.S) return 1;
            if (d == Dir.N) return -1;
            return 0;
        }

        private static bool HasPort(HydraulicComponent c, Dir side)
        {
            if (c is Pump)
                return side == Dir.S;

            if (c is Pipe pipe)
            {
                if (pipe.IsCorner)
                {
                    if (pipe.Rotation == 0)   return side == Dir.W || side == Dir.N;
                    if (pipe.Rotation == 90)  return side == Dir.S || side == Dir.W;
                    if (pipe.Rotation == 180) return side == Dir.E || side == Dir.S;
                    if (pipe.Rotation == 270) return side == Dir.N || side == Dir.E;
                    return false;
                }

                if (pipe.Rotation == 90 || pipe.Rotation == 270)
                    return side == Dir.E || side == Dir.W;

                return side == Dir.N || side == Dir.S;
            }

            return true;
        }
    }
}
