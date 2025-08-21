using PersonalDevelopment.Domain.Entities;

namespace PersonalDevelopment.Application.Interfaces
{
    public interface INoteService
    {
        Task<IEnumerable<Note>> GetNotesAsync(int userId);
        Task<int> CreateNoteAsync(Note note);
        Task<bool> UpdateNoteAsync(Note note);
        Task<bool> DeleteNoteAsync(int noteId, int userId);
    }
}
