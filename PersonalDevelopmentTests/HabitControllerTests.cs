using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using PersonalDevelopment.Api.Controllers;
using PersonalDevelopment.Application.Interfaces;
using PersonalDevelopment.Domain.Entities;
using System.Security.Claims;

public class HabitsControllerTests
{
    private readonly Mock<IHabitService> _mockService;
    private readonly Mock<ILogger<HabitsController>> _mockLogger;
    private readonly HabitsController _controller;

    public HabitsControllerTests()
    {
        _mockService = new Mock<IHabitService>();
        _mockLogger = new Mock<ILogger<HabitsController>>();
        _controller = new HabitsController(_mockService.Object, _mockLogger.Object);

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
    public async Task GetHabits_ReturnsOk_WithHabits()
    {
        var habits = new List<Habit>
        {
            new Habit { HabitId = 1, UserId = 1, Name = "Exercise" }
        };

        _mockService.Setup(s => s.GetHabitsAsync(1)).ReturnsAsync(habits);

        var result = await _controller.GetHabits();

        var ok = Assert.IsType<OkObjectResult>(result);
        var returned = Assert.IsAssignableFrom<IEnumerable<Habit>>(ok.Value);
        Assert.Single(returned);
    }

    [Fact]
    public async Task Create_ReturnsOk_WithHabitId()
    {
        var habit = new Habit { Name = "Read", Description = "Read 20 mins" };
        _mockService.Setup(s => s.CreateHabitAsync(It.IsAny<Habit>())).ReturnsAsync(99);

        var result = await _controller.Create(habit);

        var ok = Assert.IsType<OkObjectResult>(result);
        var habitIdProp = ok.Value!.GetType().GetProperty("HabitId");
        var habitId = (int)habitIdProp!.GetValue(ok.Value)!;
        Assert.Equal(99, habitId);
    }

    [Fact]
    public async Task Update_ReturnsNoContent_WhenSuccessful()
    {
        var habit = new Habit { Name = "Updated Habit" };
        _mockService.Setup(s => s.UpdateHabitAsync(It.IsAny<Habit>())).ReturnsAsync(true);

        var result = await _controller.Update(5, habit);

        Assert.IsType<NoContentResult>(result);
    }

    [Fact]
    public async Task Update_ReturnsForbid_WhenFailed()
    {
        var habit = new Habit { Name = "Updated Habit" };
        _mockService.Setup(s => s.UpdateHabitAsync(It.IsAny<Habit>())).ReturnsAsync(false);

        var result = await _controller.Update(5, habit);

        Assert.IsType<ForbidResult>(result);
    }

    [Fact]
    public async Task Delete_ReturnsNoContent_WhenSuccessful()
    {
        _mockService.Setup(s => s.DeleteHabitAsync(5, 1)).ReturnsAsync(true);

        var result = await _controller.Delete(5);

        Assert.IsType<NoContentResult>(result);
    }

    [Fact]
    public async Task Delete_ReturnsForbid_WhenFailed()
    {
        _mockService.Setup(s => s.DeleteHabitAsync(5, 1)).ReturnsAsync(false);

        var result = await _controller.Delete(5);

        Assert.IsType<ForbidResult>(result);
    }
}
