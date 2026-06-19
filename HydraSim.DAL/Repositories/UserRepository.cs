using HydraSim.DAL.Data;
using HydraSim.Domain.Auth;
using MySqlConnector;

namespace HydraSim.DAL.Repositories
{
    public class UserRepository : IUserRepository
    {
        private readonly MySqlConnectionFactory _factory;

        public UserRepository(MySqlConnectionFactory factory)
        {
            _factory = factory ?? throw new ArgumentNullException(nameof(factory));
        }

        public async Task<User?> GetByEmailAsync(string email)
        {
            using var connection = await _factory.CreateOpenConnectionAsync();

            const string sql = """
                SELECT Id, Email, PasswordHash
                FROM Users
                WHERE Email = @Email;
                """;

            using var cmd = new MySqlCommand(sql, connection);
            cmd.Parameters.AddWithValue("@Email", email);

            using var reader = await cmd.ExecuteReaderAsync();
            if (!await reader.ReadAsync()) return null;

            return new User
            {
                Id = reader.GetInt32("Id"),
                Email = reader.GetString("Email"),
                PasswordHash = reader.GetString("PasswordHash")
            };
        }

        public async Task AddAsync(User user)
        {
            using var connection = await _factory.CreateOpenConnectionAsync();

            const string sql = """
                INSERT INTO Users (Email, PasswordHash)
                VALUES (@Email, @PasswordHash);
                SELECT LAST_INSERT_ID();
                """;

            using var cmd = new MySqlCommand(sql, connection);
            cmd.Parameters.AddWithValue("@Email", user.Email);
            cmd.Parameters.AddWithValue("@PasswordHash", user.PasswordHash);

            user.Id = Convert.ToInt32(await cmd.ExecuteScalarAsync());
        }
    }
}
