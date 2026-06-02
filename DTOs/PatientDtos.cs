using System.ComponentModel.DataAnnotations;

namespace KolokwiumGrupaA.DTOs;

public record PatientListDto(
    string FirstName,
    string LastName,
    DateTime DateOfBirth,
    string Phone,
    IReadOnlyList<AppointmentDto> Appointments);

public record AppointmentDto(
    int AppointmentId,
    DoctorDto Doctor,
    string AppointmentDate,
    string Status,
    IReadOnlyList<AppointmentServiceDto> AppointmentServices);

public record DoctorDto(
    string FirstName,
    string LastName,
    string Specialization,
    string Phone);

public record AppointmentServiceDto(
    int Quantity,
    string PerformedAt,
    MedicalServiceDto MedicalService);

public record MedicalServiceDto(
    int ServiceId,
    string Name,
    string Description,
    decimal Price,
    int DurationMinutes);

public class CreatePatientDto
{
    [Required, MaxLength(50)]
    public string FirstName { get; set; } = null!;

    [Required, MaxLength(100)]
    public string LastName { get; set; } = null!;

    [Required]
    public DateOnly DateOfBirth { get; set; }

    [Required, MaxLength(9)]
    public string Phone { get; set; } = null!;

    [Required]
    public CreateAppointmentDto Appointment { get; set; } = null!;
}

public class CreateAppointmentDto
{
    [Required]
    public int DoctorId { get; set; }

    [Required]
    public DateOnly AppointmentDate { get; set; }
}
