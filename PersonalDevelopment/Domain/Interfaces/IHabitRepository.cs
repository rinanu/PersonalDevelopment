using PersonalDevelopment.Domain.Entities;

namespace PersonalDevelopment.Domain.Interfaces
{
    public interface IHabitRepository
    {
        Task<IEnumerable<Habit>> GetByUserIdAsync(int userId);
        Task<int> CreateAsync(Habit habit);
        Task<bool> UpdateAsync(Habit habit);
        Task<bool> DeleteAsync(int habitId, int userId);
    }
}
