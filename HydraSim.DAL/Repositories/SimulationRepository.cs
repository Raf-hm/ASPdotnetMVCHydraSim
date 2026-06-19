using HydraSim.DAL.Data;
using HydraSim.Domain.Components;
using HydraSim.Domain.Simulation;
using MySqlConnector;

namespace HydraSim.DAL.Repositories
{
    public class SimulationRepository : ISimulationRepository
    {
        private readonly MySqlConnectionFactory _factory;

        public SimulationRepository(MySqlConnectionFactory factory)
        {
            _factory = factory ?? throw new ArgumentNullException(nameof(factory));
        }

        public async Task<HydraulicSimulation?> LoadAsync(int id)
        {
            HydraulicSimulation? simulation = null;
            using (var connection = await _factory.CreateOpenConnectionAsync())
            {

                const string simSql = """
                SELECT Id, Name, Description, ImagePath
                FROM Simulations
                WHERE Id = @Id;
                """;

                using (var cmd = new MySqlCommand(simSql, connection))
                {
                    cmd.Parameters.AddWithValue("@Id", id);
                    using var reader = await cmd.ExecuteReaderAsync();
                    if (await reader.ReadAsync())
                    {
                        simulation = new HydraulicSimulation
                        {
                            Id = reader.GetInt32("Id"),
                            Name = reader.GetString("Name"),
                            Description = reader.GetString("Description"),
                            ImagePath = reader.GetString("ImagePath")
                        };
                    }
                }


                if (simulation == null) return null;

                simulation.Components.AddRange(await LoadComponentsAsync(connection, id));

                simulation.AutoConnect();
            }
            return simulation;
        }

        public async Task<List<HydraulicSimulation>> ListAsync()
        {
            using var connection = await _factory.CreateOpenConnectionAsync();

            const string sql = """
                SELECT Id, Name, Description, ImagePath
                FROM Simulations
                ORDER BY Id;
                """;

            var result = new List<HydraulicSimulation>();

            using var cmd = new MySqlCommand(sql, connection);
            using var reader = await cmd.ExecuteReaderAsync();
            while (await reader.ReadAsync())
            {
                result.Add(new HydraulicSimulation
                {
                    Id = reader.GetInt32("Id"),
                    Name = reader.GetString("Name"),
                    Description = reader.GetString("Description"),
                    ImagePath = reader.GetString("ImagePath")
                });
            }

            return result;
        }

        public async Task<bool> ResetAsync(int id)
        {
            var template = SeedData.BuildTemplate(id);
            if (template == null) return false;

            using var connection = await _factory.CreateOpenConnectionAsync();

            const string existsSql = "SELECT COUNT(*) FROM Simulations WHERE Id = @Id;";
            using (var existsCmd = new MySqlCommand(existsSql, connection))
            {
                existsCmd.Parameters.AddWithValue("@Id", id);
                var count = Convert.ToInt64(await existsCmd.ExecuteScalarAsync());
                if (count == 0) return false;
            }

            using var transaction = await connection.BeginTransactionAsync();

            const string deleteSql = "DELETE FROM Components WHERE SimulationId = @SimulationId;";
            using (var deleteCmd = new MySqlCommand(deleteSql, connection, (MySqlTransaction)transaction))
            {
                deleteCmd.Parameters.AddWithValue("@SimulationId", id);
                await deleteCmd.ExecuteNonQueryAsync();
            }

            const string insertSql = """
                INSERT INTO Components
                    (SimulationId, ComponentType, CX, CY, ComponentId, IsCorner, Rotation, RequiredPressure, PressureDrop, MaxPressure)
                VALUES
                    (@SimulationId, @ComponentType, @CX, @CY, @ComponentId, @IsCorner, @Rotation, @RequiredPressure, @PressureDrop, @MaxPressure);
                """;

            foreach (var component in template.Components)
            {
                using var insertCmd = new MySqlCommand(insertSql, connection, (MySqlTransaction)transaction);
                insertCmd.Parameters.AddWithValue("@SimulationId", id);
                insertCmd.Parameters.AddWithValue("@ComponentType", SeedData.GetComponentType(component));
                insertCmd.Parameters.AddWithValue("@CX", component.CX);
                insertCmd.Parameters.AddWithValue("@CY", component.CY);
                insertCmd.Parameters.AddWithValue("@ComponentId", component.ComponentId);
                insertCmd.Parameters.AddWithValue("@IsCorner", component is Pipe pipe ? pipe.IsCorner : (object)DBNull.Value);
                insertCmd.Parameters.AddWithValue("@Rotation", component is Pipe pipe2 ? pipe2.Rotation : (object)DBNull.Value);
                insertCmd.Parameters.AddWithValue("@RequiredPressure", component is Motor motor ? motor.RequiredPressure : (object)DBNull.Value);
                insertCmd.Parameters.AddWithValue("@PressureDrop", component is Resistance resistance ? resistance.PressureDrop : (object)DBNull.Value);
                insertCmd.Parameters.AddWithValue("@MaxPressure", component is ReliefValve relief ? relief.MaxPressure : (object)DBNull.Value);

                await insertCmd.ExecuteNonQueryAsync();
            }

            await transaction.CommitAsync();
            return true;
        }

        public async Task UpdateComponentAsync(HydraulicComponent component)
        {
            using var connection = await _factory.CreateOpenConnectionAsync();

            const string sql = """
                UPDATE Components
                SET RequiredPressure = @RequiredPressure,
                    PressureDrop = @PressureDrop,
                    MaxPressure = @MaxPressure
                WHERE SimulationId = @SimulationId AND ComponentId = @ComponentId;
                """;

            using var cmd = new MySqlCommand(sql, connection);
            cmd.Parameters.AddWithValue("@RequiredPressure", component is Motor motor ? motor.RequiredPressure : (object)DBNull.Value);
            cmd.Parameters.AddWithValue("@PressureDrop", component is Resistance resistance ? resistance.PressureDrop : (object)DBNull.Value);
            cmd.Parameters.AddWithValue("@MaxPressure", component is ReliefValve relief ? relief.MaxPressure : (object)DBNull.Value);
            cmd.Parameters.AddWithValue("@SimulationId", component.SimulationId);
            cmd.Parameters.AddWithValue("@ComponentId", component.ComponentId);

            await cmd.ExecuteNonQueryAsync();
        }

        private static async Task<List<HydraulicComponent>> LoadComponentsAsync(MySqlConnection connection, int simulationId)
        {
            const string sql = """
                SELECT Id, SimulationId, ComponentType, CX, CY, ComponentId, IsCorner, Rotation, RequiredPressure, PressureDrop, MaxPressure
                FROM Components
                WHERE SimulationId = @SimulationId
                ORDER BY ComponentId;
                """;

            var result = new List<HydraulicComponent>();

            using var cmd = new MySqlCommand(sql, connection);
            cmd.Parameters.AddWithValue("@SimulationId", simulationId);

            using var reader = await cmd.ExecuteReaderAsync();
            while (await reader.ReadAsync())
            {
                result.Add(MapComponent(reader));
            }

            return result;
        }

        private static HydraulicComponent MapComponent(MySqlDataReader reader)
        {
            var type = reader.GetString("ComponentType");
            var cx = reader.GetInt32("CX");
            var cy = reader.GetInt32("CY");

            HydraulicComponent component = type switch
            {
                "Pipe" => new Pipe(cx, cy,
                    isCorner: !reader.IsDBNull(reader.GetOrdinal("IsCorner")) && reader.GetBoolean("IsCorner"),
                    rotation: reader.IsDBNull(reader.GetOrdinal("Rotation")) ? 0 : reader.GetInt32("Rotation")),

                "Pump" => new Pump(cx, cy, pressureOutput: 0),

                "Motor" => new Motor(cx, cy,
                    requiredPressure: reader.IsDBNull(reader.GetOrdinal("RequiredPressure")) ? 0 : reader.GetInt32("RequiredPressure")),

                "ReliefValve" => new ReliefValve(cx, cy,
                    maxPressure: reader.IsDBNull(reader.GetOrdinal("MaxPressure")) ? 0 : reader.GetInt32("MaxPressure")),

                "Resistance" => new Resistance(cx, cy,
                    pressureDrop: reader.IsDBNull(reader.GetOrdinal("PressureDrop")) ? 0 : reader.GetInt32("PressureDrop")),

                "Tank" => new Tank(cx, cy),

                "PressureGauge" => new PressureGauge(cx, cy),

                _ => throw new InvalidOperationException($"Unknown ComponentType '{type}' in database.")
            };

            component.Id = reader.GetInt32("Id");
            component.SimulationId = reader.GetInt32("SimulationId");
            component.ComponentId = reader.GetInt32("ComponentId");

            return component;
        }
    }
}
