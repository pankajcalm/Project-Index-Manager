using System.ComponentModel.DataAnnotations.Schema;

namespace ProjectIndex.Core.Models;

public class Project
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public string Status { get; set; } = "Active"; // Active, Paused, Idea, Archived
    public string? Category { get; set; }
    public string? Path { get; set; }
    public string? RepoUrl { get; set; }
    public string? Tags { get; set; } // comma-separated
    public string? Notes { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

    [NotMapped]
    public List<string> TagList =>
        Tags?.Split(',', StringSplitOptions.RemoveEmptyEntries)
             .Select(t => t.Trim()).ToList() ?? [];
}
