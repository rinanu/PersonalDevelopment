using PersonalDevelopment.Domain.Entities;
using System.Security.Claims;
using System.Text;
using PersonalDevelopment.Application.Interfaces;
using PersonalDevelopment.Domain.Interfaces;
using PersonalDevelopment.Application.Helpers;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;


namespace PersonalDevelopment.Application.Services
{
    public class AuthService: IAuthService
    {
        private readonly IUserRepository _userRepo;
        private readonly IConfiguration _config;

        public AuthService(IUserRepository userRepo, IConfiguration config)
        {
            _userRepo = userRepo;
            _config = config;
        }

        public async Task<string?> AuthenticateAsync(string username, string password)
        {
            var user = await _userRepo.GetByUsernameAsync(username);
            if (user == null || !PasswordHasher.Verify(password, user.PasswordHash))
                return null;

            var claims = new[]
            {
            new Claim(ClaimTypes.NameIdentifier, user.UserId.ToString()),
            new Claim(ClaimTypes.Name, user.Username),
            new Claim(ClaimTypes.Role, "user")
        };

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["Jwt:Key"]!));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
            var token = new JwtSecurityToken(
                _config["Jwt:Issuer"],
                _config["Jwt:Audience"],
                claims,
                expires: DateTime.Now.AddHours(2),
                signingCredentials: creds
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        public async Task<int> RegisterAsync(string username, string email, string password)
        {
            var existing = await _userRepo.GetByUsernameAsync(username);
            if (existing != null) throw new Exception("User exists");

            var user = new User
            {
                Username = username,
                Email = email,
                PasswordHash = PasswordHasher.Hash(password)
            };
            return await _userRepo.CreateAsync(user);
        }
    }
}
