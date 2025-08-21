using PersonalDevelopment.Domain.Entities;
using PersonalDevelopment.Domain.Interfaces;
using PersonalDevelopment.Application.Interfaces;

namespace PersonalDevelopment.Application.Services
{
    public class HabitLogService : IHabitLogService
    {
        private readonly IHabitLogRepository _repo;
        public HabitLogService(IHabitLogRepository repo) => _repo = repo;

        public Task<IEnumerable<HabitLog>> GetLogsAsync(int habitId, int userId) => _repo.GetByHabitIdAsync(habitId, userId);
        public Task <bool>CreateLogAsync(HabitLog log, int userId) => _repo.CreateAsync(log, userId);
        public Task<bool> UpdateLogAsync(HabitLog log, int userId) => _repo.UpdateAsync(log, userId);
        public Task<bool> DeleteLogAsync(int habitId, DateTime logDate, int userId) => _repo.DeleteAsync(habitId, logDate, userId);
    }
}
