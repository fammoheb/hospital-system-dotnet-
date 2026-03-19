namespace HospitalManagementSystem.Models;

public class DoctorProfile
{
    public int Id { get; set; }
    public int UserId { get; set; }
    public required string Specialization { get; set; }
    public required string Bio { get; set; }
    public int YearsOfExperience { get; set; }

    // Navigation properties
    public User? User { get; set; }
}
