using KolokwiumGrupaA.Entities;
using Microsoft.EntityFrameworkCore;

namespace KolokwiumGrupaA.Data;

public class AppDbContext(DbContextOptions<AppDbContext> options) : DbContext(options)
{
    public DbSet<Patient> Patients => Set<Patient>();
    public DbSet<Doctor> Doctors => Set<Doctor>();
    public DbSet<Appointment> Appointments => Set<Appointment>();
    public DbSet<MedicalService> MedicalServices => Set<MedicalService>();
    public DbSet<AppointmentService> AppointmentServices => Set<AppointmentService>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Patient>(e =>
        {
            e.ToTable("Patients");
            e.HasKey(p => p.PatientId);
            e.Property(p => p.FirstName).HasMaxLength(50);
            e.Property(p => p.LastName).HasMaxLength(100);
            e.Property(p => p.Phone).HasMaxLength(9);
        });

        modelBuilder.Entity<Doctor>(e =>
        {
            e.ToTable("Doctors");
            e.HasKey(d => d.DoctorId);
            e.Property(d => d.FirstName).HasMaxLength(50);
            e.Property(d => d.LastName).HasMaxLength(100);
            e.Property(d => d.Specialization).HasMaxLength(100);
            e.Property(d => d.Phone).HasMaxLength(9);
        });

        modelBuilder.Entity<Appointment>(e =>
        {
            e.ToTable("Appointments");
            e.HasKey(a => a.AppointmentId);
            e.Property(a => a.Status).HasMaxLength(50);
            e.HasOne(a => a.Patient).WithMany(p => p.Appointments).HasForeignKey(a => a.PatientId);
            e.HasOne(a => a.Doctor).WithMany(d => d.Appointments).HasForeignKey(a => a.DoctorId);
        });

        modelBuilder.Entity<MedicalService>(e =>
        {
            e.ToTable("Medical_Services");
            e.HasKey(s => s.ServiceId);
            e.Property(s => s.Name).HasMaxLength(100);
            e.Property(s => s.Description).HasMaxLength(100);
            e.Property(s => s.Price).HasColumnType("decimal(10,2)");
        });

        modelBuilder.Entity<AppointmentService>(e =>
        {
            e.ToTable("Appointment_Services");
            e.HasKey(x => new { x.AppointmentId, x.ServiceId });
            e.HasOne(x => x.Appointment).WithMany(a => a.AppointmentServices).HasForeignKey(x => x.AppointmentId);
            e.HasOne(x => x.MedicalService).WithMany(s => s.AppointmentServices).HasForeignKey(x => x.ServiceId);
        });
    }
}
