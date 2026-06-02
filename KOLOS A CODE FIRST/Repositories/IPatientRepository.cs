using KolokwiumGrupaA.DTOs;
using KolokwiumGrupaA.Entities;

namespace KolokwiumGrupaA.Repositories;

public interface IPatientRepository
{
    Task<IReadOnlyList<PatientListDto>> GetPatientsAsync(string? lastName, CancellationToken cancellationToken = default);
    Task<(Patient? Patient, string? Error)> AddPatientWithAppointmentAsync(CreatePatientDto dto, CancellationToken cancellationToken = default);
}
