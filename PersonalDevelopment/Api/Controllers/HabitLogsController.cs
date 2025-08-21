using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PersonalDevelopment.Application.Interfaces;
using PersonalDevelopment.Domain.Entities;
using System.Security.Claims;

namespace PersonalDevelopment.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class HabitLogsController : ControllerBase
{
    private readonly IHabitLogService _service;
    private readonly ILogger<HabitLogsController> _logger;
    public HabitLogsController(IHabitLogService service, ILogger<HabitLogsController> logger)
    {
        _service = service;
        _logger = logger;
    }

    private int UserId => int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

    [HttpGet("{habitId}")]
    public async Task<IActionResult> GetLogs(int habitId)
    {
        var logs = await _service.GetLogsAsync(habitId, UserId);
        if (!logs.Any())
        {
            _logger.LogWarning("User {UserId} requested habit logs but none found", UserId);
        }
        else
            _logger.LogInformation("User {UserId} retrieved {Count} habit logs", UserId, logs.Count());
        return Ok(logs);
    }

    [HttpPost]
    public async Task<IActionResult> Create(HabitLog log)
    {
        var success = await _service.CreateLogAsync(log, UserId);
        if (!success)
        {
            _logger.LogWarning("Failed to create log for habit {HabitId} at {LogDate}", log.HabitId, log.LogDate);
            return Forbid();
        }

        _logger.LogInformation("Created log for habit {HabitId} at {LogDate}", log.HabitId, log.LogDate);
        return Ok();
    }

    [HttpPut]
    public async Task<IActionResult> Update(HabitLog log)
    {
        var success = await _service.UpdateLogAsync(log, UserId);
        if (!success)
        {
            _logger.LogWarning("Failed to update log for habit {HabitId} at {LogDate}", log.HabitId, log.LogDate);
            return Forbid();
        }

        _logger.LogInformation("Updated log for habit {HabitId} at {LogDate}", log.HabitId, log.LogDate);
        return NoContent();
    }

    [HttpDelete("{habitId}/{date}")]
    public async Task<IActionResult> Delete(int habitId, DateTime date)
    {
        var success = await _service.DeleteLogAsync(habitId, date, UserId);
        if (!success)
        {
            _logger.LogWarning("Failed to delete log for habit {HabitId} at {LogDate}", habitId, date);
            return Forbid();
        }

        _logger.LogInformation("Deleted log for habit {HabitId} at {LogDate}", habitId, date);
        return NoContent();
    }
}
