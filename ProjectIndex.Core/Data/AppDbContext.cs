using Microsoft.EntityFrameworkCore;
using ProjectIndex.Core.Models;

namespace ProjectIndex.Core.Data;

public class AppDbContext(DbContextOptions<AppDbContext> options)
    : DbContext(options)
{
    public DbSet<Project> Projects => Set<Project>();

    protected override void OnModelCreating(ModelBuilder mb)
    {
        mb.Entity<Project>()
          .HasIndex(p => p.Name);
    }
}
