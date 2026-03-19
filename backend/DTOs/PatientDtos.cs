using System.ComponentModel.DataAnnotations;

namespace HospitalManagementSystem.DTOs;

public class CreatePatientDto
{
    [Required(ErrorMessage = "User ID is required")]
    public int UserId { get; set; }

    [Required(ErrorMessage = "Date of birth is required")]
    public DateTime DateOfBirth { get; set; }

    [Required(ErrorMessage = "Blood type is required")]
    [MaxLength(5)]
    public required string BloodType { get; set; }

    [Required(ErrorMessage = "Address is required")]
    [MaxLength(200)]
    public required string Address { get; set; }
}

public class UpdatePatientDto
{
    public DateTime? DateOfBirth { get; set; }

    [MaxLength(5)]
    public string? BloodType { get; set; }

    [MaxLength(200)]
    public string? Address { get; set; }
}

public class PatientResponseDto
{
    public int Id { get; set; }
    public int UserId { get; set; }
    public string PatientName { get; set; } = "";
    public DateTime DateOfBirth { get; set; }
    public string BloodType { get; set; } = "";
    public string Address { get; set; } = "";
}
