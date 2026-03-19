using System.ComponentModel.DataAnnotations;

namespace HospitalManagementSystem.DTOs;

public class CreateDepartmentDto
{
    [Required(ErrorMessage = "Name is required")]
    [MaxLength(100)]
    public required string Name { get; set; }

    [Required(ErrorMessage = "Description is required")]
    [MaxLength(500)]
    public required string Description { get; set; }
}

public class UpdateDepartmentDto
{
    [MaxLength(100)]
    public string? Name { get; set; }

    [MaxLength(500)]
    public string? Description { get; set; }
}

public class DepartmentResponseDto
{
    public int Id { get; set; }
    public string Name { get; set; } = "";
    public string Description { get; set; } = "";
}
