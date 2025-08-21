using PersonalDevelopment.Domain.Entities;

namespace PersonalDevelopment.Domain.Interfaces
{
    public interface INoteRepository
    {
        Task<IEnumerable<Note>> GetByUserIdAsync(int userId);
        Task<int> CreateAsync(Note note);
        Task<bool>UpdateAsync(Note note);
        Task<bool>DeleteAsync(int noteId, int userId);
    }
}
