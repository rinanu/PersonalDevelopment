using PersonalDevelopment.Domain.Entities;
using PersonalDevelopment.Infrastructure.Database;
using PersonalDevelopment.Domain.Interfaces;
using Npgsql;

namespace PersonalDevelopment.Infrastructure.Repositories;

public class NoteRepository : INoteRepository
{
    private readonly DbConnectionFactory _factory;
    public NoteRepository(DbConnectionFactory factory) => _factory = factory;

    public async Task<IEnumerable<Note>> GetByUserIdAsync(int userId)
    {
        var notes = new List<Note>();
        await using var conn = _factory.Create();
        await conn.OpenAsync();
        var cmd = new NpgsqlCommand("SELECT note_id, user_id, title, note_content FROM notes WHERE user_id=@u", conn);
        cmd.Parameters.AddWithValue("u", userId);
        await using var reader = await cmd.ExecuteReaderAsync();
        while (await reader.ReadAsync())
        {
            notes.Add(new Note
            {
                NoteId = reader.GetInt32(0),
                UserId = reader.GetInt32(1),
                Title = reader.IsDBNull(2) ? null : reader.GetString(2),
                Content = reader.GetString(3)
            });
        }
        return notes;
    }

    public async Task<int> CreateAsync(Note note)
    {
        await using var conn = _factory.Create();
        await conn.OpenAsync();
        var cmd = new NpgsqlCommand("INSERT INTO notes(user_id, title, note_content) VALUES (@u, @t, @c) RETURNING note_id", conn);
        cmd.Parameters.AddWithValue("u", note.UserId);
        cmd.Parameters.AddWithValue("t", (object?)note.Title ?? DBNull.Value);
        cmd.Parameters.AddWithValue("c", note.Content);
        return (int)await cmd.ExecuteScalarAsync();
    }

    public async Task<bool> UpdateAsync(Note note)
    {
        await using var conn = _factory.Create();
        await conn.OpenAsync();
        var cmd = new NpgsqlCommand(
            "UPDATE notes SET title=@t, note_content=@c WHERE note_id=@id AND user_id=@u",
            conn
        );
        cmd.Parameters.AddWithValue("t", (object?)note.Title ?? DBNull.Value);
        cmd.Parameters.AddWithValue("c", (object?)note.Content ?? DBNull.Value);
        cmd.Parameters.AddWithValue("id", note.NoteId);
        cmd.Parameters.AddWithValue("u", note.UserId);

        var rows = await cmd.ExecuteNonQueryAsync();
        return rows > 0; // true = updated, false = no permission or not found
    }

    public async Task<bool> DeleteAsync(int noteId, int userId)
    {
        await using var conn = _factory.Create();
        await conn.OpenAsync();
        var cmd = new NpgsqlCommand(
            "DELETE FROM notes WHERE note_id=@id AND user_id=@u",
            conn
        );
        cmd.Parameters.AddWithValue("id", noteId);
        cmd.Parameters.AddWithValue("u", userId);

        var rows = await cmd.ExecuteNonQueryAsync();
        return rows > 0; // true if deleted, false otherwise
    } 
}
