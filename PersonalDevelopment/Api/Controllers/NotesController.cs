using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PersonalDevelopment.Application.Interfaces;
using PersonalDevelopment.Domain.Entities;
using System.Security.Claims;

namespace PersonalDevelopment.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class NotesController : ControllerBase
{
    private readonly INoteService _service;
    private readonly ILogger<NotesController> _logger;
    public NotesController(INoteService service, ILogger<NotesController> logger)
    {
        _service = service;
        _logger = logger;
    }
    private int UserId => int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

    [HttpGet]
    public async Task<IActionResult> GetNotes()
    {
        var notes = await _service.GetNotesAsync(UserId);
        if (!notes.Any())
        {
            _logger.LogWarning("User {UserId} requested notes but none found", UserId);
        }
        else
            _logger.LogInformation("User {UserId} retrieved {Count} notes", UserId, notes.Count());
        return Ok(notes);
    }

    [HttpPost]
    public async Task<IActionResult> Create(Note note)
    {
        note.UserId = UserId;
        var id = await _service.CreateNoteAsync(note);

        _logger.LogInformation("User {UserId} created note {NoteId}", UserId, id);
        return Ok(new { NoteId = id });
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Update(int id, Note note)
    {
        note.NoteId = id;
        note.UserId = UserId;

        var success = await _service.UpdateNoteAsync(note);
        if (!success)
        {
            _logger.LogWarning("User {UserId} attempted to update note {NoteId} but was denied", UserId, id);
            return Forbid();
        }

        _logger.LogInformation("User {UserId} updated note {NoteId}", UserId, id);
        return NoContent();
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        var success = await _service.DeleteNoteAsync(id, UserId);
        if (!success)
        {
            _logger.LogWarning("User {UserId} attempted to delete note {NoteId} but was denied", UserId, id);
            return Forbid();
        }

        _logger.LogInformation("User {UserId} deleted note {NoteId}", UserId, id);
        return NoContent();
    }

}
