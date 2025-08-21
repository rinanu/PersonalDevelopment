using PersonalDevelopment.Domain.Entities;
using PersonalDevelopment.Infrastructure.Database;
using PersonalDevelopment.Domain.Interfaces;
using Npgsql;

namespace PersonalDevelopment.Infrastructure.Repositories;

public class HabitRepository : IHabitRepository
{
    private readonly DbConnectionFactory _factory;
    public HabitRepository(DbConnectionFactory factory) => _factory = factory;

    public async Task<IEnumerable<Habit>> GetByUserIdAsync(int userId)
    {
        var habits = new List<Habit>();
        await using var conn = _factory.Create();
        await conn.OpenAsync();
        var cmd = new NpgsqlCommand("SELECT habit_id, user_id, habit_name, description, frequency, is_active FROM habits WHERE user_id=@u", conn);
        cmd.Parameters.AddWithValue("u", userId);
        await using var reader = await cmd.ExecuteReaderAsync();
        while (await reader.ReadAsync())
        {
            habits.Add(new Habit
            {
                HabitId = reader.GetInt32(0),
                UserId = reader.GetInt32(1),
                Name = reader.GetString(2),
                Description = reader.IsDBNull(3) ? null : reader.GetString(3),
                Frequency = reader.GetString(4),
                IsActive = reader.GetBoolean(5)
            });
        }
        return habits;
    }

    public async Task<int> CreateAsync(Habit habit)
    {
        await using var conn = _factory.Create();
        await conn.OpenAsync();
        var cmd = new NpgsqlCommand("INSERT INTO habits(user_id, habit_name, description, frequency, is_active) VALUES (@u, @n, @d, @f, @a) RETURNING habit_id", conn);
        cmd.Parameters.AddWithValue("u", habit.UserId);
        cmd.Parameters.AddWithValue("n", habit.Name);
        cmd.Parameters.AddWithValue("d", (object?)habit.Description ?? DBNull.Value);
        cmd.Parameters.AddWithValue("f", habit.Frequency);
        cmd.Parameters.AddWithValue("a", habit.IsActive);
        return (int)await cmd.ExecuteScalarAsync();
    }

    public async Task<bool> UpdateAsync(Habit habit)
    {
        await using var conn = _factory.Create();
        await conn.OpenAsync();
        var cmd = new NpgsqlCommand(
            "UPDATE habits SET habit_name=@n, description=@d, frequency=@f, is_active=@a " +
            "WHERE habit_id=@id AND user_id=@u",
            conn
        );
        cmd.Parameters.AddWithValue("n", habit.Name);
        cmd.Parameters.AddWithValue("d", (object?)habit.Description ?? DBNull.Value);
        cmd.Parameters.AddWithValue("f", (object?)habit.Frequency ?? DBNull.Value);
        cmd.Parameters.AddWithValue("a", habit.IsActive);
        cmd.Parameters.AddWithValue("id", habit.HabitId);
        cmd.Parameters.AddWithValue("u", habit.UserId);

        var rows = await cmd.ExecuteNonQueryAsync();
        return rows > 0;
    }

    public async Task<bool> DeleteAsync(int habitId, int userId)
    {
        await using var conn = _factory.Create();
        await conn.OpenAsync();
        var cmd = new NpgsqlCommand(
            "DELETE FROM habits WHERE habit_id=@id AND user_id=@u",
            conn
        );
        cmd.Parameters.AddWithValue("id", habitId);
        cmd.Parameters.AddWithValue("u", userId);

        var rows = await cmd.ExecuteNonQueryAsync();
        return rows > 0;
    }

}
