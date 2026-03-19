using HospitalManagementSystem.DTOs;
using HospitalManagementSystem.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HospitalManagementSystem.Controllers;

[Route("api/[controller]")]
[ApiController]
[Authorize]
public class DepartmentsController : ControllerBase
{
    private readonly IDepartmentService _departmentService;

    public DepartmentsController(IDepartmentService departmentService)
    {
        _departmentService = departmentService;
    }

    /// <summary>
    /// Get all departments (All authenticated users)
    /// </summary>
    [HttpGet]
    [Authorize]
    public async Task<IActionResult> GetAllDepartments()
    {
        var departments = await _departmentService.GetAllDepartmentsAsync();
        return Ok(departments);
    }

    /// <summary>
    /// Get department by ID (All authenticated users)
    /// </summary>
    [HttpGet("{id}")]
    [Authorize]
    public async Task<IActionResult> GetDepartmentById(int id)
    {
        var department = await _departmentService.GetDepartmentByIdAsync(id);
        if (department == null)
            return NotFound();

        return Ok(department);
    }

    /// <summary>
    /// Create a new department (Admin only)
    /// </summary>
    [HttpPost]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> CreateDepartment([FromBody] CreateDepartmentDto dto)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        var department = await _departmentService.CreateDepartmentAsync(dto);
        return CreatedAtAction(nameof(GetDepartmentById), new { id = department.Id }, department);
    }

    /// <summary>
    /// Update department (Admin only)
    /// </summary>
    [HttpPut("{id}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> UpdateDepartment(int id, [FromBody] UpdateDepartmentDto dto)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        var department = await _departmentService.UpdateDepartmentAsync(id, dto);
        if (department == null)
            return NotFound();

        return Ok(department);
    }

    /// <summary>
    /// Delete department (Admin only)
    /// </summary>
    [HttpDelete("{id}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> DeleteDepartment(int id)
    {
        var success = await _departmentService.DeleteDepartmentAsync(id);
        if (!success)
            return NotFound();

        return NoContent();
    }
}
