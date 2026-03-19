using System.ComponentModel.DataAnnotations;

namespace HospitalManagementSystem.DTOs;

public class CreateDoctorDto
{
    [Required(ErrorMessage = "User ID is required")]
    public int UserId { get; set; }

    [Required(ErrorMessage = "Department ID is required")]
    public int DepartmentId { get; set; }

    [Required(ErrorMessage = "License number is required")]
    [MaxLength(50)]
    public required string LicenseNumber { get; set; }

    [Required(ErrorMessage = "Specialization is required")]
    [MaxLength(100)]
    public required string Specialization { get; set; }

    [Required(ErrorMessage = "Bio is required")]
    [MaxLength(500)]
    public required string Bio { get; set; }

    [Required(ErrorMessage = "Years of experience is required")]
    [Range(0, 60, ErrorMessage = "Years of experience must be between 0 and 60")]
    public int YearsOfExperience { get; set; }
}

public class UpdateDoctorDto
{
    public int? DepartmentId { get; set; }

    [MaxLength(50)]
    public string? LicenseNumber { get; set; }

    [MaxLength(100)]
    public string? Specialization { get; set; }

    [MaxLength(500)]
    public string? Bio { get; set; }

    [Range(0, 60, ErrorMessage = "Years of experience must be between 0 and 60")]
    public int? YearsOfExperience { get; set; }
}

public class DoctorResponseDto
{
    public int Id { get; set; }
    public int UserId { get; set; }
    public int DepartmentId { get; set; }
    public string LicenseNumber { get; set; } = "";
    public string DoctorName { get; set; } = "";
    public string Specialization { get; set; } = "";
    public string Bio { get; set; } = "";
    public int YearsOfExperience { get; set; }
    public string DepartmentName { get; set; } = "";
}
