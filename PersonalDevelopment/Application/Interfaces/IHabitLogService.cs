using PersonalDevelopment.Domain.Entities;

namespace PersonalDevelopment.Application.Interfaces
{
    public interface IHabitLogService
    {
        Task<IEnumerable<HabitLog>> GetLogsAsync(int habitId, int userId);
        Task<bool> CreateLogAsync(HabitLog log, int userId);
        Task<bool> UpdateLogAsync(HabitLog log, int userId);
        Task<bool> DeleteLogAsync(int habitId, DateTime date, int userId);
    }
}
