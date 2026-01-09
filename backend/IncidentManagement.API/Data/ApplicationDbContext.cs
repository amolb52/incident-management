using Microsoft.EntityFrameworkCore;
using IncidentManagement.API.Models;

namespace IncidentManagement.API.Data;

public class ApplicationDbContext : DbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) { }
    public DbSet<Incident> Incidents { get; set; } = null!;
    public DbSet<Attachment> Attachments { get; set; } = null!;

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        modelBuilder.Entity<Incident>().Property(i => i.Severity).HasConversion<string>();
        modelBuilder.Entity<Incident>().Property(i => i.Status).HasConversion<string>();
    }
}
