namespace HospitalManagementSystem.Models;

public class Department
{
    public int Id { get; set; }
    public required string Name { get; set; }
    public required string Description { get; set; }

    // Navigation properties
    public ICollection<Doctor> Doctors { get; set; } = [];
}
