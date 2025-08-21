using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using PersonalDevelopment.Api.Controllers;
using PersonalDevelopment.Application.Interfaces;

public class AuthControllerTests
{
    private readonly Mock<IAuthService> _mockAuth;
    private readonly Mock<ILogger<AuthController>> _mockLogger;
    private readonly AuthController _controller;

    public AuthControllerTests()
    {
        _mockAuth = new Mock<IAuthService>();
        _mockLogger = new Mock<ILogger<AuthController>>();
        _controller = new AuthController(_mockAuth.Object, _mockLogger.Object);
    }

    [Fact]
    public async Task Register_ReturnsOk_WhenRegistrationSucceeds()
    {
        var req = new RegisterRequest("testuser", "test@mail.com", "password");
        _mockAuth.Setup(a => a.RegisterAsync(req.Username, req.Email, req.Password))
                 .ReturnsAsync(1);

        var result = await _controller.Register(req);

        var ok = Assert.IsType<OkObjectResult>(result);
        var userIdProp = ok.Value!.GetType().GetProperty("UserId");
        var userId = (int)userIdProp!.GetValue(ok.Value)!;

        Assert.Equal(1, userId);
    }

    [Fact]
    public async Task Register_ReturnsBadRequest_WhenRegistrationFails()
    {
        var req = new RegisterRequest("testuser", "test@mail.com", "password");
        _mockAuth.Setup(a => a.RegisterAsync(req.Username, req.Email, req.Password))
                 .ReturnsAsync(0);

        var result = await _controller.Register(req);

        var bad = Assert.IsType<BadRequestObjectResult>(result);
        Assert.NotNull(bad.Value);
    }

    [Fact]
    public async Task Login_ReturnsOk_WhenLoginSucceeds()
    {
        var req = new LoginRequest("testuser", "password");
        _mockAuth.Setup(a => a.AuthenticateAsync(req.Username, req.Password))
                 .ReturnsAsync("token123");

        var result = await _controller.Login(req);

        var ok = Assert.IsType<OkObjectResult>(result);
        var tokenProp = ok.Value!.GetType().GetProperty("Token");
        var token = (string)tokenProp!.GetValue(ok.Value)!;

        Assert.Equal("token123", token);
    }

    [Fact]
    public async Task Login_ReturnsUnauthorized_WhenLoginFails()
    {
        var req = new LoginRequest("testuser", "wrongpass");
        _mockAuth.Setup(a => a.AuthenticateAsync(req.Username, req.Password))
                 .ReturnsAsync((string?)null);

        var result = await _controller.Login(req);

        Assert.IsType<UnauthorizedObjectResult>(result);
    }

}