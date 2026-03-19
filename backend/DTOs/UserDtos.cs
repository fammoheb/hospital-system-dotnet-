using System.ComponentModel.DataAnnotations;

namespace HospitalManagementSystem.DTOs;

public class CreateUserDto
{
    [Required(ErrorMessage = "Full name is required")]
    [MaxLength(100)]
    public required string FullName { get; set; }

    [Required(ErrorMessage = "Email is required")]
    [EmailAddress(ErrorMessage = "Invalid email format")]
    public required string Email { get; set; }

    [Required(ErrorMessage = "Password is required")]
    [MinLength(8, ErrorMessage = "Password must be at least 8 characters")]
    public required string Password { get; set; }

    [Required(ErrorMessage = "Role is required")]
    public required string Role { get; set; } // "Admin", "Doctor", "Patient"
}

public class UpdateUserDto
{
    [MaxLength(100)]
    public string? FullName { get; set; }

    [EmailAddress(ErrorMessage = "Invalid email format")]
    public string? Email { get; set; }

    [MinLength(8, ErrorMessage = "Password must be at least 8 characters")]
    public string? Password { get; set; }
}

public class UserResponseDto
{
    public int Id { get; set; }
    public string FullName { get; set; } = "";
    public string Email { get; set; } = "";
    public string Role { get; set; } = "";
}
