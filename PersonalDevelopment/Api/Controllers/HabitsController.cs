using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PersonalDevelopment.Application.Interfaces;
using PersonalDevelopment.Domain.Entities;
using System.Security.Claims;

namespace PersonalDevelopment.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class HabitsController : ControllerBase
{
    private readonly IHabitService _service;
    private readonly ILogger<HabitsController> _logger;
    public HabitsController(IHabitService service, ILogger<HabitsController> logger)
    {
        _service = service;
        _logger = logger;
    }

    private int UserId => int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

    [HttpGet]
    public async Task<IActionResult> GetHabits()
    {
        var habits = await _service.GetHabitsAsync(UserId);
        if (!habits.Any())
        {
            _logger.LogWarning("User {UserId} requested habits but none found", UserId);           
        }
        else
            _logger.LogInformation("User {UserId} retrieved {Count} habits", UserId, habits.Count());
        return Ok(habits);
    }

    [HttpPost]
    public async Task<IActionResult> Create(Habit habit)
    {
        // enforce UserId
        habit.UserId = UserId;

        var id = await _service.CreateHabitAsync(habit);
        _logger.LogInformation("User {UserId} created habit {HabitId}", UserId, id);
        return Ok(new { HabitId = id });
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Update(int id, Habit habit)
    {
        habit.HabitId = id;
        habit.UserId = UserId;

        var success = await _service.UpdateHabitAsync(habit);
        if (!success)
        {
            _logger.LogWarning("User {UserId} attempted to update habit {HabitId} but was denied", UserId, id);
            return Forbid();
        }

        _logger.LogInformation("User {UserId} updated habit {HabitId}", UserId, id);
        return NoContent();
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        var success = await _service.DeleteHabitAsync(id, UserId);
        if (!success)
        {
            _logger.LogWarning("User {UserId} attempted to delete habit {HabitId} but was denied", UserId, id);
            return Forbid();
        }

        _logger.LogInformation("User {UserId} deleted habit {HabitId}", UserId, id);
        return NoContent();
    }

}
