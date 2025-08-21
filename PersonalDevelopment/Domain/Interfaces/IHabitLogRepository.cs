using PersonalDevelopment.Domain.Entities;

namespace PersonalDevelopment.Domain.Interfaces
{
    public interface IHabitLogRepository
    {
        Task<IEnumerable<HabitLog>> GetByHabitIdAsync(int habitId, int userId);
        Task <bool>CreateAsync(HabitLog log, int userId);
        Task<bool> UpdateAsync(HabitLog log, int userId);
        Task<bool> DeleteAsync(int habitId, DateTime logDate, int userId);
    }
}
