using HydraSim.Domain.Components;
using HydraSim.Domain.Simulation;
using MySqlConnector;

namespace HydraSim.DAL.Data
{
    public static class SeedData
    {
        public static async Task SeedAsync(MySqlConnectionFactory factory)
        {
            using var connection = await factory.CreateOpenConnectionAsync();

            const string countSql = "SELECT COUNT(*) FROM Simulations;";
            using (var countCmd = new MySqlCommand(countSql, connection))
            {
                var count = Convert.ToInt64(await countCmd.ExecuteScalarAsync());
                if (count > 0) return;
            }

            var templates = new[]
            {
                BuildSandbox(),
                BuildPressureDropsInSeries(),
                BuildDirectActingReliefValve(),
                BuildBalancedPilotReliefValve(),
                BuildPressureCompensatedFlowControl()
            };

            foreach (var sim in templates)
            {
                await InsertSimulationAsync(connection, sim);
            }
        }

        private static async Task InsertSimulationAsync(MySqlConnection connection, HydraulicSimulation sim)
        {
            const string insertSimSql = """
                INSERT INTO Simulations (Name, Description, ImagePath)
                VALUES (@Name, @Description, @ImagePath);
                SELECT LAST_INSERT_ID();
                """;

            int simulationId;
            using (var cmd = new MySqlCommand(insertSimSql, connection))
            {
                cmd.Parameters.AddWithValue("@Name", sim.Name);
                cmd.Parameters.AddWithValue("@Description", sim.Description);
                cmd.Parameters.AddWithValue("@ImagePath", sim.ImagePath);
                simulationId = Convert.ToInt32(await cmd.ExecuteScalarAsync());
            }

            foreach (var component in sim.Components)
            {
                await InsertComponentAsync(connection, simulationId, component);
            }
        }

        private static async Task InsertComponentAsync(MySqlConnection connection, int simulationId, HydraulicComponent component)
        {
            const string insertComponentSql = """
                INSERT INTO Components
                    (SimulationId, ComponentType, CX, CY, ComponentId, IsCorner, Rotation, RequiredPressure, PressureDrop, MaxPressure)
                VALUES
                    (@SimulationId, @ComponentType, @CX, @CY, @ComponentId, @IsCorner, @Rotation, @RequiredPressure, @PressureDrop, @MaxPressure);
                """;

            using var cmd = new MySqlCommand(insertComponentSql, connection);
            cmd.Parameters.AddWithValue("@SimulationId", simulationId);
            cmd.Parameters.AddWithValue("@ComponentType", GetComponentType(component));
            cmd.Parameters.AddWithValue("@CX", component.CX);
            cmd.Parameters.AddWithValue("@CY", component.CY);
            cmd.Parameters.AddWithValue("@ComponentId", component.ComponentId);
            cmd.Parameters.AddWithValue("@IsCorner", component is Pipe pipe ? pipe.IsCorner : (object)DBNull.Value);
            cmd.Parameters.AddWithValue("@Rotation", component is Pipe pipe2 ? pipe2.Rotation : (object)DBNull.Value);
            cmd.Parameters.AddWithValue("@RequiredPressure", component is Motor motor ? motor.RequiredPressure : (object)DBNull.Value);
            cmd.Parameters.AddWithValue("@PressureDrop", component is Resistance resistance ? resistance.PressureDrop : (object)DBNull.Value);
            cmd.Parameters.AddWithValue("@MaxPressure", component is ReliefValve relief ? relief.MaxPressure : (object)DBNull.Value);

            await cmd.ExecuteNonQueryAsync();
        }

        internal static string GetComponentType(HydraulicComponent component) => component switch
        {
            Pipe => "Pipe",
            Pump => "Pump",
            Motor => "Motor",
            ReliefValve => "ReliefValve",
            Resistance => "Resistance",
            Tank => "Tank",
            PressureGauge => "PressureGauge",
            _ => throw new InvalidOperationException($"Unknown component type: {component.GetType().Name}")
        };

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
                ImagePath = "/Sprites/sims/sim1.png"
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
                ImagePath = "/Sprites/sims/sim2.png"
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
                ImagePath = "/Sprites/sims/sim3.png"
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
                ImagePath = "/Sprites/sims/sim4.png"
            };

            sim.AddComponent(new Pump(1, 1, 0));
            sim.AddComponent(new Pipe(1, 2));
            sim.AddComponent(new Tank(1, 3));

            return sim;
        }
    }
}
