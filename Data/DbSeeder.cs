using KolokwiumGrupaA.Entities;
using Microsoft.EntityFrameworkCore;

namespace KolokwiumGrupaA.Data;

public static class DbSeeder
{
    public static async Task SeedAsync(AppDbContext context)
    {
        if (await context.Patients.AnyAsync())
            return;

        var doctors = new[]
        {
            new Doctor { FirstName = "Adam", LastName = "Mazur", Specialization = "Cardiology", Phone = "111222333" },
            new Doctor { FirstName = "Ewa", LastName = "Kaczmarek", Specialization = "Dermatology", Phone = "222333444" }
        };
        context.Doctors.AddRange(doctors);

        var services = new[]
        {
            new MedicalService { Name = "General Consultation", Description = "Basic medical consultation", Price = 150, DurationMinutes = 30 },
            new MedicalService { Name = "Blood Test", Description = "Routine blood work", Price = 80, DurationMinutes = 15 },
            new MedicalService { Name = "ECG", Description = "Electrocardiogram examination", Price = 120, DurationMinutes = 20 },
            new MedicalService { Name = "Skin Examination", Description = "Dermatological skin check", Price = 200, DurationMinutes = 40 }
        };
        context.MedicalServices.AddRange(services);
        await context.SaveChangesAsync();

        var patients = new[]
        {
            new Patient { FirstName = "Anna", LastName = "Kowalska", DateOfBirth = new DateTime(1990, 3, 15), Phone = "123456789" },
            new Patient { LastName = "Nowak", FirstName = "Jan", DateOfBirth = new DateTime(1985, 7, 22), Phone = "234567891" }
        };
        context.Patients.AddRange(patients);
        await context.SaveChangesAsync();

        var appt1 = new Appointment
        {
            PatientId = patients[0].PatientId,
            DoctorId = doctors[0].DoctorId,
            AppointmentDate = new DateOnly(2026, 5, 20),
            Status = "Completed"
        };
        var appt2 = new Appointment
        {
            PatientId = patients[1].PatientId,
            DoctorId = doctors[1].DoctorId,
            AppointmentDate = new DateOnly(2026, 5, 21),
            Status = "Completed"
        };
        context.Appointments.AddRange(appt1, appt2);
        await context.SaveChangesAsync();

        context.AppointmentServices.AddRange(
            new AppointmentService { AppointmentId = appt1.AppointmentId, ServiceId = services[0].ServiceId, Quantity = 1, PerformedAt = new DateOnly(2026, 5, 20) },
            new AppointmentService { AppointmentId = appt1.AppointmentId, ServiceId = services[2].ServiceId, Quantity = 1, PerformedAt = new DateOnly(2026, 5, 20) },
            new AppointmentService { AppointmentId = appt2.AppointmentId, ServiceId = services[3].ServiceId, Quantity = 1, PerformedAt = new DateOnly(2026, 5, 21) }
        );
        await context.SaveChangesAsync();
    }
}
