namespace HospitalManagementSystem.Models;

public class Patient
{
    public int Id { get; set; }
    public int UserId { get; set; }
    public DateTime DateOfBirth { get; set; }
    public required string BloodType { get; set; }
    public required string Address { get; set; }

    // Navigation properties
    public User? User { get; set; }
    public ICollection<Appointment> Appointments { get; set; } = [];
}
