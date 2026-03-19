namespace HospitalManagementSystem.Models;

public class Appointment
{
    public int Id { get; set; }
    public int DoctorId { get; set; }
    public int PatientId { get; set; }
    public DateTime AppointmentDate { get; set; }
    public required string Status { get; set; } // "Pending", "Confirmed", "Completed", "Cancelled"
    public string? Notes { get; set; }

    // Navigation properties
    public Doctor? Doctor { get; set; }
    public Patient? Patient { get; set; }
}
