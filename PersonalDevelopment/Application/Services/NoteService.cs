using PersonalDevelopment.Domain.Entities;
using PersonalDevelopment.Domain.Interfaces;
using PersonalDevelopment.Application.Interfaces;

namespace PersonalDevelopment.Application.Services
{
    public class NoteService: INoteService
    {
        private readonly INoteRepository _repo;
        public NoteService(INoteRepository repo) => _repo = repo;

        public Task<IEnumerable<Note>> GetNotesAsync(int userId) => _repo.GetByUserIdAsync(userId);
        public Task<int> CreateNoteAsync(Note note) => _repo.CreateAsync(note);
        public Task<bool>UpdateNoteAsync(Note note) => _repo.UpdateAsync(note);
        public Task<bool>DeleteNoteAsync(int noteId, int userId) => _repo.DeleteAsync(noteId, userId);
    }
}
