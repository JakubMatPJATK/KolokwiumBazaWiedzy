using KolokwiumGrupaA.Data;
using KolokwiumGrupaA.DTOs;
using KolokwiumGrupaA.Entities;
using Microsoft.EntityFrameworkCore;

namespace KolokwiumGrupaA.Repositories;

public class PatientRepository(AppDbContext context) : IPatientRepository
{
    public async Task<IReadOnlyList<PatientListDto>> GetPatientsAsync(string? lastName, CancellationToken cancellationToken = default)
    {
        var query = context.Patients
            .AsNoTracking()
            .Include(p => p.Appointments)
                .ThenInclude(a => a.Doctor)
            .Include(p => p.Appointments)
                .ThenInclude(a => a.AppointmentServices)
                .ThenInclude(asv => asv.MedicalService)
            .AsQueryable();

        if (!string.IsNullOrWhiteSpace(lastName))
            query = query.Where(p => p.LastName.Contains(lastName));

        var patients = await query.ToListAsync(cancellationToken);

        return patients.Select(MapToListDto).ToList();
    }

    public async Task<(Patient? Patient, string? Error)> AddPatientWithAppointmentAsync(
        CreatePatientDto dto,
        CancellationToken cancellationToken = default)
    {
        var doctorExists = await context.Doctors.AnyAsync(d => d.DoctorId == dto.Appointment.DoctorId, cancellationToken);
        if (!doctorExists)
            return (null, $"Doctor with id {dto.Appointment.DoctorId} does not exist.");

        if (dto.Appointment.AppointmentDate < DateOnly.FromDateTime(DateTime.Today))
            return (null, "Appointment date cannot be earlier than today.");

        await using var transaction = await context.Database.BeginTransactionAsync(cancellationToken);
        try
        {
            var patient = new Patient
            {
                FirstName = dto.FirstName,
                LastName = dto.LastName,
                DateOfBirth = dto.DateOfBirth.ToDateTime(TimeOnly.MinValue),
                Phone = dto.Phone
            };
            context.Patients.Add(patient);
            await context.SaveChangesAsync(cancellationToken);

            context.Appointments.Add(new Appointment
            {
                PatientId = patient.PatientId,
                DoctorId = dto.Appointment.DoctorId,
                AppointmentDate = dto.Appointment.AppointmentDate,
                Status = "Scheduled"
            });
            await context.SaveChangesAsync(cancellationToken);
            await transaction.CommitAsync(cancellationToken);

            return (patient, null);
        }
        catch
        {
            await transaction.RollbackAsync(cancellationToken);
            throw;
        }
    }

    private static PatientListDto MapToListDto(Patient patient) =>
        new(
            patient.FirstName,
            patient.LastName,
            patient.DateOfBirth,
            patient.Phone,
            patient.Appointments.Select(a => new AppointmentDto(
                a.AppointmentId,
                new DoctorDto(a.Doctor.FirstName, a.Doctor.LastName, a.Doctor.Specialization, a.Doctor.Phone),
                a.AppointmentDate.ToString("yyyy-MM-dd"),
                a.Status,
                a.AppointmentServices.Select(s => new AppointmentServiceDto(
                    s.Quantity,
                    s.PerformedAt.ToString("yyyy-MM-dd"),
                    new MedicalServiceDto(
                        s.MedicalService.ServiceId,
                        s.MedicalService.Name,
                        s.MedicalService.Description,
                        s.MedicalService.Price,
                        s.MedicalService.DurationMinutes))).ToList())).ToList());
}
