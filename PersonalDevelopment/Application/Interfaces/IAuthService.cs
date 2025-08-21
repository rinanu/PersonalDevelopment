namespace PersonalDevelopment.Application.Interfaces
{
    public interface IAuthService
    {
        Task<int> RegisterAsync(string username, string email, string password);
        Task<string?> AuthenticateAsync(string username, string password);
    }
}
