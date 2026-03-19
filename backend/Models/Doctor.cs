namespace HospitalManagementSystem.Models;

public class Doctor
{
    public int Id { get; set; }
    public int UserId { get; set; }
    public int DepartmentId { get; set; }
    public required string LicenseNumber { get; set; }

    // Navigation properties
    public User? User { get; set; }
    public Department? Department { get; set; }
    public ICollection<Appointment> Appointments { get; set; } = [];
}
