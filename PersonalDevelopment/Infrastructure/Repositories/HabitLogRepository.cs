using PersonalDevelopment.Domain.Entities;
using PersonalDevelopment.Infrastructure.Database;
using PersonalDevelopment.Domain.Interfaces;
using Npgsql;

namespace PersonalDevelopment.Infrastructure.Repositories;

public class HabitLogRepository : IHabitLogRepository
{
    private readonly DbConnectionFactory _factory;
    public HabitLogRepository(DbConnectionFactory factory) => _factory = factory;

    public async Task<IEnumerable<HabitLog>> GetByHabitIdAsync(int habitId, int userId)
    {
        var logs = new List<HabitLog>();
        await using var conn = _factory.Create();
        await conn.OpenAsync();
        var cmd = new NpgsqlCommand(@"
            SELECT hl.habit_id, hl.log_date, hl.status, hl.note
            FROM habit_logs hl
            JOIN habits h ON hl.habit_id = h.habit_id
            WHERE hl.habit_id=@h AND h.user_id=@u", conn);
        cmd.Parameters.AddWithValue("h", habitId);
        cmd.Parameters.AddWithValue("u", userId);
        await using var reader = await cmd.ExecuteReaderAsync();
        while (await reader.ReadAsync())
        {
            logs.Add(new HabitLog
            {
                HabitId = reader.GetInt32(0),
                LogDate = reader.GetDateTime(1),
                Status = reader.GetBoolean(2),
                Note = reader.IsDBNull(3) ? null : reader.GetString(3)
            });
        }
        return logs;
    }
    public async Task<bool> CreateAsync(HabitLog log, int userId)
    {
        await using var conn = _factory.Create();
        await conn.OpenAsync();

        var cmd = new NpgsqlCommand(@"
        INSERT INTO habit_logs(habit_id, log_date, status, note)
        SELECT @h, @d, @s, @n
        WHERE EXISTS (SELECT 1 FROM habits WHERE habit_id=@h AND user_id=@u)", conn);

        cmd.Parameters.AddWithValue("h", log.HabitId);
        cmd.Parameters.AddWithValue("d", log.LogDate);
        cmd.Parameters.AddWithValue("s", log.Status);
        cmd.Parameters.AddWithValue("n", (object?)log.Note ?? DBNull.Value);
        cmd.Parameters.AddWithValue("u", userId);

        var rows = await cmd.ExecuteNonQueryAsync();
        return rows > 0; // true if insert happened, false otherwise
    }


    public async Task<bool> UpdateAsync(HabitLog log, int userId)
    {
        await using var conn = _factory.Create();
        await conn.OpenAsync();
        var cmd = new NpgsqlCommand(@"
            UPDATE habit_logs hl
            SET status=@s, note=@n
            FROM habits h
            WHERE hl.habit_id=@h AND hl.log_date=@d AND h.user_id=@u AND hl.habit_id=h.habit_id", conn);
        cmd.Parameters.AddWithValue("s", log.Status);
        cmd.Parameters.AddWithValue("n", (object?)log.Note ?? DBNull.Value);
        cmd.Parameters.AddWithValue("h", log.HabitId);
        cmd.Parameters.AddWithValue("d", log.LogDate);
        cmd.Parameters.AddWithValue("u", userId);
        var rows = await cmd.ExecuteNonQueryAsync();
        return rows > 0;
    }

    public async Task<bool> DeleteAsync(int habitId, DateTime logDate, int userId)
    {
        await using var conn = _factory.Create();
        await conn.OpenAsync();
        var cmd = new NpgsqlCommand(@"
            DELETE FROM habit_logs hl
            USING habits h
            WHERE hl.habit_id=@h AND hl.log_date=@d AND h.user_id=@u AND hl.habit_id=h.habit_id", conn);
        cmd.Parameters.AddWithValue("h", habitId);
        cmd.Parameters.AddWithValue("d", logDate);
        cmd.Parameters.AddWithValue("u", userId);
        var rows = await cmd.ExecuteNonQueryAsync();
        return rows > 0;
    }
}