using Microsoft.AspNetCore.Mvc;
using PersonalDevelopment.Application.Interfaces;

namespace PersonalDevelopment.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly IAuthService _auth;
    private readonly ILogger<AuthController> _logger;

    public AuthController(IAuthService auth, ILogger<AuthController> logger)
    {
        _auth = auth;
        _logger = logger;
    }

    [HttpPost("register")]
    public async Task<IActionResult> Register([FromBody] RegisterRequest req)
    {
        var id = await _auth.RegisterAsync(req.Username, req.Email, req.Password);

        if (id <= 0)
        {
            _logger.LogWarning("Registration failed for {Username}", req.Username);
            return BadRequest(new { Message = "Registration failed" });
        }

        _logger.LogInformation("User {UserId} ({Username}) registered successfully", id, req.Username);
        return Ok(new { UserId = id });
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginRequest req)
    {
        var token = await _auth.AuthenticateAsync(req.Username, req.Password);

        if (token == null)
        {
            _logger.LogWarning("Failed login attempt for {Username}", req.Username);
            return Unauthorized(new { Message = "Invalid credentials" });
        }

        _logger.LogInformation("User {Username} logged in successfully", req.Username);
        return Ok(new { Token = token });
    }
}

public record RegisterRequest(string Username, string Email, string Password);
public record LoginRequest(string Username, string Password);
