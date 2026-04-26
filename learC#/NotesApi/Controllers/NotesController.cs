using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NotesApi.Data;
using NotesApi.Models;

namespace NotesApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class NotesController : ControllerBase
{
    private readonly AppDbContext _db;

    public NotesController(AppDbContext db) => _db = db;

    [HttpGet]
    public async Task<ActionResult<IReadOnlyList<Note>>> GetAll(CancellationToken ct) =>
        await _db.Notes.AsNoTracking().OrderByDescending(n => n.Id).ToListAsync(ct);

    [HttpGet("count")]
    public async Task<ActionResult<int>> GetCount(CancellationToken ct) =>
        await _db.Notes.CountAsync(ct);

    [HttpGet("{id:int}")]
    public async Task<ActionResult<Note>> GetById(int id, CancellationToken ct)
    {
        var note = await _db.Notes.AsNoTracking().FirstOrDefaultAsync(n => n.Id == id, ct);
        return note is null ? NotFound() : note;
    }

    [HttpPost]
    public async Task<ActionResult<Note>> Create([FromBody] NoteInput input, CancellationToken ct)
    {
        var note = new Note
        {
            Title = input.Title.Trim(),
            Content = string.IsNullOrWhiteSpace(input.Content) ? null : input.Content.Trim(),
            CreatedAt = DateTime.UtcNow,
        };
        _db.Notes.Add(note);
        await _db.SaveChangesAsync(ct);
        return CreatedAtAction(nameof(GetById), new { id = note.Id }, note);
    }

    [HttpPut("{id:int}")]
    public async Task<ActionResult<Note>> Update(int id, [FromBody] NoteInput input, CancellationToken ct)
    {
        var note = await _db.Notes.FirstOrDefaultAsync(n => n.Id == id, ct);
        if (note is null) return NotFound();

        note.Title = input.Title.Trim();
        note.Content = string.IsNullOrWhiteSpace(input.Content) ? null : input.Content.Trim();
        await _db.SaveChangesAsync(ct);
        return note;
    }

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id, CancellationToken ct)
    {
        var note = await _db.Notes.FirstOrDefaultAsync(n => n.Id == id, ct);
        if (note is null) return NotFound();

        _db.Notes.Remove(note);
        await _db.SaveChangesAsync(ct);
        return NoContent();
    }
}
