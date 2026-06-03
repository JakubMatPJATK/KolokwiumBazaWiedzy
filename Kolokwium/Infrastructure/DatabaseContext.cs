using Kolokwium.Entities;
using Microsoft.EntityFrameworkCore;

namespace Kolokwium.Infrastructure;

public class DatabaseContext(DbContextOptions<DatabaseContext> options, IConfiguration configuration) : DbContext(options)
{
    // Dodaj DbSet<> dla encji z ERD:
    // public virtual DbSet<Patient> Patients { get; set; }

    public virtual DbSet<ExampleEntity> Examples { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        var schema = configuration["DB:DefaultSchema"];
        if (!string.IsNullOrWhiteSpace(schema))
            modelBuilder.HasDefaultSchema(schema);

        // Fluent API – klucze złożone, nazwy tabel, FK z ERD:
        // modelBuilder.Entity<Enrollment>().HasKey(e => new { e.CourseId, e.StudentId });
        // modelBuilder.Entity<Order>().Property(o => o.UserId).HasColumnName("Users_UserId");

        modelBuilder.Entity<ExampleEntity>(e =>
        {
            e.HasKey(x => x.Id);
            e.Property(x => x.Name).HasMaxLength(100).IsRequired();
        });
    }
}
