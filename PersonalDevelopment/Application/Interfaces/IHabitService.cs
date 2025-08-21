using PersonalDevelopment.Domain.Entities;

namespace PersonalDevelopment.Application.Interfaces
{
    public interface IHabitService
    {
        Task<IEnumerable<Habit>> GetHabitsAsync(int userId);
        Task<int> CreateHabitAsync(Habit habit);
        Task<bool> UpdateHabitAsync(Habit habit);
        Task<bool> DeleteHabitAsync(int habitId, int userId);
    }
}
