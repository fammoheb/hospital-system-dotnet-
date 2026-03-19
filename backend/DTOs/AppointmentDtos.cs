using System.ComponentModel.DataAnnotations;

namespace HospitalManagementSystem.DTOs;

public class CreateAppointmentDto
{
    [Required(ErrorMessage = "Doctor ID is required")]
    public int DoctorId { get; set; }

    [Required(ErrorMessage = "Patient ID is required")]
    public int PatientId { get; set; }

    [Required(ErrorMessage = "Appointment date is required")]
    public DateTime AppointmentDate { get; set; }

    [MaxLength(500)]
    public string? Notes { get; set; }
}

public class UpdateAppointmentDto
{
    public DateTime? AppointmentDate { get; set; }

    [MaxLength(20)]
    public string? Status { get; set; }

    [MaxLength(500)]
    public string? Notes { get; set; }
}

public class AppointmentResponseDto
{
    public int Id { get; set; }
    public int DoctorId { get; set; }
    public int PatientId { get; set; }
    public string DoctorName { get; set; } = "";
    public string PatientName { get; set; } = "";
    public DateTime AppointmentDate { get; set; }
    public string Status { get; set; } = "";
    public string? Notes { get; set; }
}
