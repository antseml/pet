using System.ComponentModel.DataAnnotations;

namespace NotesApi.Models;

public class NoteInput
{
    [Required]
    [StringLength(100, MinimumLength = 1)]
    public string Title { get; set; } = string.Empty;

    [StringLength(2000)]
    public string? Content { get; set; }
}
