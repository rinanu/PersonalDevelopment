using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using PersonalDevelopment.Api.Controllers;
using PersonalDevelopment.Application.Interfaces;
using PersonalDevelopment.Domain.Entities;
using System.Security.Claims;
public class NotesControllerTests
{
    private readonly Mock<INoteService> _mockService;
    private readonly Mock<ILogger<NotesController>> _mockLogger;
    private readonly NotesController _controller;

    public NotesControllerTests()
    {
        _mockService = new Mock<INoteService>();
        _mockLogger = new Mock<ILogger<NotesController>>();
        _controller = new NotesController(_mockService.Object, _mockLogger.Object);

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
    public async Task GetNotes_ReturnsOk_WithNotes()
    {
        var notes = new List<Note>
        {
            new Note { NoteId = 1, UserId = 1, Title = "Test", Content = "Hello" }
        };

        _mockService.Setup(s => s.GetNotesAsync(1)).ReturnsAsync(notes);

        var result = await _controller.GetNotes();

        var ok = Assert.IsType<OkObjectResult>(result);
        var returned = Assert.IsAssignableFrom<IEnumerable<Note>>(ok.Value);
        Assert.Single(returned);
    }

    [Fact]
    public async Task Create_ReturnsOk_WithNoteId()
    {
        var note = new Note { Title = "New", Content = "Content" };
        _mockService.Setup(s => s.CreateNoteAsync(It.IsAny<Note>())).ReturnsAsync(42);

        var result = await _controller.Create(note);

        var ok = Assert.IsType<OkObjectResult>(result);
        var noteIdProp = ok.Value!.GetType().GetProperty("NoteId");
        var noteId = (int)noteIdProp!.GetValue(ok.Value)!;
        Assert.Equal(42, noteId);
    }

    [Fact]
    public async Task Update_ReturnsNoContent_WhenSuccessful()
    {
        var note = new Note { Title = "Update", Content = "Updated" };
        _mockService.Setup(s => s.UpdateNoteAsync(It.IsAny<Note>())).ReturnsAsync(true);

        var result = await _controller.Update(5, note);

        Assert.IsType<NoContentResult>(result);
    }

    [Fact]
    public async Task Update_ReturnsForbid_WhenFailed()
    {
        var note = new Note { Title = "Update", Content = "Updated" };
        _mockService.Setup(s => s.UpdateNoteAsync(It.IsAny<Note>())).ReturnsAsync(false);

        var result = await _controller.Update(5, note);

        Assert.IsType<ForbidResult>(result);
    }

    [Fact]
    public async Task Delete_ReturnsNoContent_WhenSuccessful()
    {
        _mockService.Setup(s => s.DeleteNoteAsync(5, 1)).ReturnsAsync(true);

        var result = await _controller.Delete(5);

        Assert.IsType<NoContentResult>(result);
    }

    [Fact]
    public async Task Delete_ReturnsForbid_WhenFailed()
    {
        _mockService.Setup(s => s.DeleteNoteAsync(5, 1)).ReturnsAsync(false);

        var result = await _controller.Delete(5);

        Assert.IsType<ForbidResult>(result);
    }
}
