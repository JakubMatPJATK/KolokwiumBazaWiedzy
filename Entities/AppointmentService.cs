namespace KolokwiumGrupaA.Entities;

public class AppointmentService
{
    public int AppointmentId { get; set; }
    public int ServiceId { get; set; }
    public int Quantity { get; set; }
    public DateOnly PerformedAt { get; set; }

    public Appointment Appointment { get; set; } = null!;
    public MedicalService MedicalService { get; set; } = null!;
}
