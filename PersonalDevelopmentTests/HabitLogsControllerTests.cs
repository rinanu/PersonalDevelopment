using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using PersonalDevelopment.Api.Controllers;
using PersonalDevelopment.Application.Interfaces;
using PersonalDevelopment.Domain.Entities;
using System.Security.Claims;

public class HabitLogsControllerTests
{
    private readonly Mock<IHabitLogService> _mockService;
    private readonly Mock<ILogger<HabitLogsController>> _mockLogger;
    private readonly HabitLogsController _controller;

    public HabitLogsControllerTests()
    {
        _mockService = new Mock<IHabitLogService>();
        _mockLogger = new Mock<ILogger<HabitLogsController>>();
        _controller = new HabitLogsController(_mockService.Object, _mockLogger.Object);

        // Fake authenticated user with Id = 1
        var user = new ClaimsPrincipal(new ClaimsIdentity(new[]
        {
            new Claim(ClaimTypes.NameIdentifier, "1")
        }, "mock"));

        _controller.ControllerContext = new ControllerContext
        {
            HttpContext = new DefaultHttpContext { User = user }
        };
    }

    [Fact]
    public async Task GetLogs_ReturnsOk_WithLogs()
    {
        var logs = new List<HabitLog>
        {
            new HabitLog { HabitId = 1, LogDate = DateTime.Today, Status = true }
        };

        _mockService.Setup(s => s.GetLogsAsync(1, 1)).ReturnsAsync(logs);

        var result = await _controller.GetLogs(1);

        var ok = Assert.IsType<OkObjectResult>(result);
        var returned = Assert.IsAssignableFrom<IEnumerable<HabitLog>>(ok.Value);
        Assert.Single(returned);
    }

    [Fact]
    public async Task Create_ReturnsOk_WhenSuccessful()
    {
        var log = new HabitLog { HabitId = 1, LogDate = DateTime.Today, Status = true };
        _mockService.Setup(s => s.CreateLogAsync(log, 1)).ReturnsAsync(true);

        var result = await _controller.Create(log);

        Assert.IsType<OkResult>(result);
    }

    [Fact]
    public async Task Create_ReturnsForbid_WhenFailed()
    {
        var log = new HabitLog { HabitId = 1, LogDate = DateTime.Today, Status = true };
        _mockService.Setup(s => s.CreateLogAsync(log, 1)).ReturnsAsync(false);

        var result = await _controller.Create(log);

        Assert.IsType<ForbidResult>(result);
    }

    [Fact]
    public async Task Update_ReturnsNoContent_WhenSuccessful()
    {
        var log = new HabitLog { HabitId = 1, LogDate = DateTime.Today, Status = false };
        _mockService.Setup(s => s.UpdateLogAsync(log, 1)).ReturnsAsync(true);

        var result = await _controller.Update(log);

        Assert.IsType<NoContentResult>(result);
    }

    [Fact]
    public async Task Update_ReturnsForbid_WhenFailed()
    {
        var log = new HabitLog { HabitId = 1, LogDate = DateTime.Today, Status = false };
        _mockService.Setup(s => s.UpdateLogAsync(log, 1)).ReturnsAsync(false);

        var result = await _controller.Update(log);

        Assert.IsType<ForbidResult>(result);
    }

    [Fact]
    public async Task Delete_ReturnsNoContent_WhenSuccessful()
    {
        var date = DateTime.Today;
        _mockService.Setup(s => s.DeleteLogAsync(1, date, 1)).ReturnsAsync(true);

        var result = await _controller.Delete(1, date);

        Assert.IsType<NoContentResult>(result);
    }

    [Fact]
    public async Task Delete_ReturnsForbid_WhenFailed()
    {
        var date = DateTime.Today;
        _mockService.Setup(s => s.DeleteLogAsync(1, date, 1)).ReturnsAsync(false);

        var result = await _controller.Delete(1, date);

        Assert.IsType<ForbidResult>(result);
    }
}
