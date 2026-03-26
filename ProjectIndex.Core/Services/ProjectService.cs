using Microsoft.EntityFrameworkCore;
using ProjectIndex.Core.Data;
using ProjectIndex.Core.Models;

namespace ProjectIndex.Core.Services;

public class ProjectService(AppDbContext db)
{
    public async Task<List<Project>> SearchAsync(
        string? query = null, string? status = null, string? category = null)
    {
        var q = db.Projects.AsQueryable();

        if (!string.IsNullOrWhiteSpace(query))
            q = q.Where(p => p.Name.Contains(query) ||
                              (p.Description != null && p.Description.Contains(query)) ||
                              (p.Tags != null && p.Tags.Contains(query)) ||
                              (p.Path != null && p.Path.Contains(query)));

        if (!string.IsNullOrWhiteSpace(status))
            q = q.Where(p => p.Status == status);

        if (!string.IsNullOrWhiteSpace(category))
            q = q.Where(p => p.Category == category);

        return await q.OrderByDescending(p => p.UpdatedAt).ToListAsync();
    }

    public async Task<Project?> GetByIdAsync(int id) =>
        await db.Projects.FindAsync(id);

    public async Task<Project> SaveAsync(Project project)
    {
        project.UpdatedAt = DateTime.UtcNow;
        if (project.Id == 0)
            db.Projects.Add(project);
        else
            db.Projects.Update(project);

        await db.SaveChangesAsync();
        return project;
    }

    public async Task DeleteAsync(int id)
    {
        await db.Projects.Where(p => p.Id == id).ExecuteDeleteAsync();
    }

    public async Task<List<string>> GetCategoriesAsync() =>
        await db.Projects
                .Where(p => p.Category != null)
                .Select(p => p.Category!)
                .Distinct()
                .OrderBy(c => c)
                .ToListAsync();
}
