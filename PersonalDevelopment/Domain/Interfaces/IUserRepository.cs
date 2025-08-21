using PersonalDevelopment.Domain.Entities;

namespace PersonalDevelopment.Domain.Interfaces
{
    public interface IUserRepository
    {
        Task<User?> GetByUsernameAsync(string username);
        Task<User?> GetByIdAsync(int id);
        Task<int> CreateAsync(User user);
    }
}
