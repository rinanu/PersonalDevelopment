using PersonalDevelopment.Domain.Entities;
using PersonalDevelopment.Domain.Interfaces;
using PersonalDevelopment.Infrastructure.Database;
using Npgsql;

namespace PersonalDevelopment.Infrastructure.Repositories
{
    public class UserRepository : IUserRepository
    {
        private readonly DbConnectionFactory _factory;
        public UserRepository(DbConnectionFactory factory) => _factory = factory;

        public async Task<User?> GetByUsernameAsync(string username)
        {
            await using var conn = _factory.Create();
            await conn.OpenAsync();
            var cmd = new NpgsqlCommand("SELECT user_id, username, email, password_hash FROM users WHERE username=@u", conn);
            cmd.Parameters.AddWithValue("u", username);
            await using var reader = await cmd.ExecuteReaderAsync();
            if (await reader.ReadAsync())
            {
                return new User
                {
                    UserId = reader.GetInt32(0),
                    Username = reader.GetString(1),
                    Email = reader.GetString(2),
                    PasswordHash = reader.GetString(3)
                };
            }
            return null;
        }

        public async Task<User?> GetByIdAsync(int id)
        {
            await using var conn = _factory.Create();
            await conn.OpenAsync();
            var cmd = new NpgsqlCommand("SELECT user_id, username, email, password_hash FROM users WHERE user_id=@id", conn);
            cmd.Parameters.AddWithValue("id", id);
            await using var reader = await cmd.ExecuteReaderAsync();
            if (await reader.ReadAsync())
            {
                return new User
                {
                    UserId = reader.GetInt32(0),
                    Username = reader.GetString(1),
                    Email = reader.GetString(2),
                    PasswordHash = reader.GetString(3)
                };
            }
            return null;
        }

        public async Task<int> CreateAsync(User user)
        {
            await using var conn = _factory.Create();
            await conn.OpenAsync();
            var cmd = new NpgsqlCommand("INSERT INTO users(username, email, password_hash) VALUES (@u, @e, @p) RETURNING user_id", conn);
            cmd.Parameters.AddWithValue("u", user.Username);
            cmd.Parameters.AddWithValue("e", user.Email);
            cmd.Parameters.AddWithValue("p", user.PasswordHash);
            return (int)await cmd.ExecuteScalarAsync();
        }
    }
}
