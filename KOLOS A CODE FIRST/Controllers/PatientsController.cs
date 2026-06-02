using KolokwiumGrupaA.DTOs;
using KolokwiumGrupaA.Repositories;
using Microsoft.AspNetCore.Mvc;

namespace KolokwiumGrupaA.Controllers;

[ApiController]
[Route("api/[controller]")]
public class PatientsController(IPatientRepository patientRepository) : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<IReadOnlyList<PatientListDto>>> GetPatients(
        [FromQuery] string? lastName,
        CancellationToken cancellationToken)
    {
        var result = await patientRepository.GetPatientsAsync(lastName, cancellationToken);
        return Ok(result);
    }

    [HttpPost]
    public async Task<IActionResult> CreatePatient(
        [FromBody] CreatePatientDto dto,
        CancellationToken cancellationToken)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        var (patient, error) = await patientRepository.AddPatientWithAppointmentAsync(dto, cancellationToken);
        if (error is not null)
            return BadRequest(new { message = error });

        return CreatedAtAction(nameof(GetPatients), new { id = patient!.PatientId }, new { patient.PatientId });
    }
}
