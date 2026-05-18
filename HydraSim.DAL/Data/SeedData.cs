using HydraSim.Domain.Components;
using HydraSim.Domain.Simulation;

namespace HydraSim.DAL.Data
{
    public static class SeedData
    {
        public static void Seed(HydraSimDbContext context)
        {
            if (context.Simulations.Any()) return;

            context.Simulations.Add(BuildSandbox());
            context.Simulations.Add(BuildPressureDropsInSeries());
            context.Simulations.Add(BuildDirectActingReliefValve());
            context.Simulations.Add(BuildBalancedPilotReliefValve());
            context.Simulations.Add(BuildPressureCompensatedFlowControl());
            context.SaveChanges();
        }

        public static HydraulicSimulation? BuildTemplate(int id)
        {
            if (id == 1) return BuildSandbox();
            if (id == 2) return BuildPressureDropsInSeries();
            if (id == 3) return BuildDirectActingReliefValve();
            if (id == 4) return BuildBalancedPilotReliefValve();
            if (id == 5) return BuildPressureCompensatedFlowControl();
            return null;
        }

        private static HydraulicSimulation BuildSandbox()
        {
            var sim = new HydraulicSimulation
            {
                Name = "Build your own circuit",
                Description = "A sandbox for you to experiment with building your own simulations.",
                ImagePath = "/Sprites/sims/sim0.png"
            };

            sim.AddComponent(new Pump(1, 1, 0));

            return sim;
        }

        private static HydraulicSimulation BuildPressureDropsInSeries()
        {
            var sim = new HydraulicSimulation
            {
                Name = "Pressure Drops in Series Circuits",
                Description = "When several hydraulic components are connected in series, and each component adds some resistance to flow, what effect does that have on the upstream pressure? The answer is surprising to some people, but it's actually very simple.",
                ImagePath = "/Sprites/sims/sim0.png"
            };

            sim.AddComponent(new Pump(1, 1, 0));
            sim.AddComponent(new Pipe(1, 2));
            sim.AddComponent(new PressureGauge(1, 3));
            sim.AddComponent(new Pipe(1, 4));
            sim.AddComponent(new Resistance(1, 5, 300));
            sim.AddComponent(new Pipe(1, 6));
            sim.AddComponent(new PressureGauge(1, 7));
            sim.AddComponent(new Pipe(2, 7, rotation: 90));
            sim.AddComponent(new Resistance(3, 7, 200));
            sim.AddComponent(new Pipe(4, 7, rotation: 90));
            sim.AddComponent(new PressureGauge(5, 7));
            sim.AddComponent(new Pipe(5, 6));
            sim.AddComponent(new Resistance(5, 5, 100));
            sim.AddComponent(new Pipe(5, 4));
            sim.AddComponent(new PressureGauge(5, 3));
            sim.AddComponent(new Pipe(5, 2));
            sim.AddComponent(new Tank(5, 1));
            sim.AddComponent(new Pipe(4, 1, rotation: 90));
            sim.AddComponent(new Pipe(3, 1, rotation: 90));
            sim.AddComponent(new Pipe(2, 1, rotation: 90));

            return sim;
        }

        private static HydraulicSimulation BuildDirectActingReliefValve()
        {
            var sim = new HydraulicSimulation
            {
                Name = "Direct Acting Relief Valve",
                Description = "This style of relief valve has only one poppet and one spring. It might seem nice and simple, but there is a downside to that simplicity.",
                ImagePath = "/Sprites/sims/sim1.png"
            };

            sim.AddComponent(new Pump(1, 1, 0));
            sim.AddComponent(new Pipe(1, 2));
            sim.AddComponent(new PressureGauge(1, 3));
            sim.AddComponent(new Pipe(1, 4));
            sim.AddComponent(new Pipe(2, 3, rotation: 90));
            sim.AddComponent(new Motor(3, 3, 200));
            sim.AddComponent(new Pipe(3, 4));
            sim.AddComponent(new PressureGauge(3, 5));
            sim.AddComponent(new Pipe(3, 6));
            sim.AddComponent(new ReliefValve(1, 5, 300));
            sim.AddComponent(new Pipe(1, 6));
            sim.AddComponent(new PressureGauge(1, 7));
            sim.AddComponent(new Pipe(2, 7, rotation: 90));
            sim.AddComponent(new Tank(3, 7));
            sim.AddComponent(new Pipe(4, 7, rotation: 90));
            sim.AddComponent(new Pipe(5, 7, isCorner: true));
            sim.AddComponent(new Pipe(5, 6));
            sim.AddComponent(new Pipe(5, 5));
            sim.AddComponent(new Pipe(5, 4));
            sim.AddComponent(new Pipe(5, 3));
            sim.AddComponent(new Pipe(5, 2));
            sim.AddComponent(new Pipe(5, 1, isCorner: true, rotation: 90));
            sim.AddComponent(new Pipe(4, 1, rotation: 90));
            sim.AddComponent(new Pipe(3, 1, rotation: 90));
            sim.AddComponent(new Pipe(2, 1, rotation: 90));

            return sim;
        }

        private static HydraulicSimulation BuildBalancedPilotReliefValve()
        {
            var sim = new HydraulicSimulation
            {
                Name = "Balanced, Pilot Operated Relief Valve",
                Description = "We cover the features and benefits of the Balanced, Pilot Operated Relief Valve. This valve is a little more complicated than a simple Direct Acting Relief Valve.",
                ImagePath = "/Sprites/sims/sim0.png"
            };


            sim.AddComponent(new Pump(1, 1, 0));
            sim.AddComponent(new Pipe(1, 2));
            sim.AddComponent(new Tank(1, 3));

            return sim;
        }

        private static HydraulicSimulation BuildPressureCompensatedFlowControl()
        {
            var sim = new HydraulicSimulation
            {
                Name = "Pressure Compensated Flow Control",
                Description = "This valve has a fairly simple design, and it produces a simple effect, but how the effect happens is quite fascinating.",
                ImagePath = "/Sprites/sims/sim0.png"
            };

            sim.AddComponent(new Pump(1, 1, 0));
            sim.AddComponent(new Pipe(1, 2));
            sim.AddComponent(new Tank(1, 3));

            return sim;
        }
    }
}

