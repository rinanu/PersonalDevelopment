using Npgsql;

namespace PersonalDevelopment.Infrastructure.Database
{
    public class DbConnectionFactory
    {
        private readonly string _connectionString;

        public DbConnectionFactory(IConfiguration config)
        {
            _connectionString = config.GetConnectionString("DefaultConnection")!;
        }

        public NpgsqlConnection Create() => new NpgsqlConnection(_connectionString);
    }
}
