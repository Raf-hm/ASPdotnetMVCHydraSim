using MySqlConnector;

namespace HydraSim.DAL.Data
{
    public static class DatabaseInitializer
    {
        public static async Task EnsureCreatedAsync(MySqlConnectionFactory factory)
        {
            using var connection = await factory.CreateOpenConnectionAsync();

            const string createSimulations = """
                CREATE TABLE IF NOT EXISTS Simulations (
                    Id INT NOT NULL AUTO_INCREMENT,
                    Name VARCHAR(100) NOT NULL,
                    Description VARCHAR(1000) NOT NULL,
                    ImagePath VARCHAR(200) NOT NULL,
                    PRIMARY KEY (Id)
                );
                """;

            const string createComponents = """
                CREATE TABLE IF NOT EXISTS Components (
                    Id INT NOT NULL AUTO_INCREMENT,
                    SimulationId INT NOT NULL,
                    ComponentType VARCHAR(50) NOT NULL,
                    CX INT NOT NULL,
                    CY INT NOT NULL,
                    ComponentId INT NOT NULL,
                    IsCorner TINYINT(1) NULL,
                    Rotation INT NULL,
                    RequiredPressure INT NULL,
                    PressureDrop INT NULL,
                    MaxPressure INT NULL,
                    PRIMARY KEY (Id),
                    CONSTRAINT FK_Components_Simulations FOREIGN KEY (SimulationId)
                        REFERENCES Simulations (Id) ON DELETE CASCADE
                );
                """;

            const string createUsers = """
                CREATE TABLE IF NOT EXISTS Users (
                    Id INT NOT NULL AUTO_INCREMENT,
                    Email VARCHAR(255) NOT NULL,
                    PasswordHash VARCHAR(500) NOT NULL,
                    PRIMARY KEY (Id),
                    UNIQUE KEY UX_Users_Email (Email)
                );
                """;

            foreach (var sql in new[] { createSimulations, createComponents, createUsers })
            {
                using var command = new MySqlCommand(sql, connection);
                await command.ExecuteNonQueryAsync();
            }
        }
    }
}
