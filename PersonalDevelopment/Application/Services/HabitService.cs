using PersonalDevelopment.Domain.Entities;
using PersonalDevelopment.Domain.Interfaces;
using PersonalDevelopment.Application.Interfaces;

namespace PersonalDevelopment.Application.Services
{
    public class HabitService : IHabitService
    {
        private readonly IHabitRepository _repo;
        public HabitService(IHabitRepository repo) => _repo = repo;

        public Task<IEnumerable<Habit>> GetHabitsAsync(int userId) => _repo.GetByUserIdAsync(userId);
        public Task<int> CreateHabitAsync(Habit habit) => _repo.CreateAsync(habit);
        public Task <bool>UpdateHabitAsync(Habit habit) => _repo.UpdateAsync(habit);
        public Task<bool> DeleteHabitAsync(int habitId, int userId) => _repo.DeleteAsync(habitId, userId);
    }
}
