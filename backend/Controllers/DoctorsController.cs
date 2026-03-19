using HospitalManagementSystem.DTOs;
using HospitalManagementSystem.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HospitalManagementSystem.Controllers;

[Route("api/[controller]")]
[ApiController]
[Authorize]
public class DoctorsController : ControllerBase
{
    private readonly IDoctorService _doctorService;

    public DoctorsController(IDoctorService doctorService)
    {
        _doctorService = doctorService;
    }

    /// <summary>
    /// Get all doctors (Admin, Patient)
    /// </summary>
    [HttpGet]
    [Authorize(Roles = "Admin,Patient")]
    public async Task<IActionResult> GetAllDoctors()
    {
        var doctors = await _doctorService.GetAllDoctorsAsync();
        return Ok(doctors);
    }

    /// <summary>
    /// Get doctor by ID (Admin, Patient)
    /// </summary>
    [HttpGet("{id}")]
    [Authorize(Roles = "Admin,Patient")]
    public async Task<IActionResult> GetDoctorById(int id)
    {
        var doctor = await _doctorService.GetDoctorByIdAsync(id);
        if (doctor == null)
            return NotFound();

        return Ok(doctor);
    }

    /// <summary>
    /// Create a new doctor (Admin only)
    /// </summary>
    [HttpPost]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> CreateDoctor([FromBody] CreateDoctorDto dto)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        var doctor = await _doctorService.CreateDoctorAsync(dto);
        return CreatedAtAction(nameof(GetDoctorById), new { id = doctor.Id }, doctor);
    }

    /// <summary>
    /// Update doctor (Admin, Doctor)
    /// </summary>
    [HttpPut("{id}")]
    [Authorize(Roles = "Admin,Doctor")]
    public async Task<IActionResult> UpdateDoctor(int id, [FromBody] UpdateDoctorDto dto)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        var doctor = await _doctorService.UpdateDoctorAsync(id, dto);
        if (doctor == null)
            return NotFound();

        return Ok(doctor);
    }

    /// <summary>
    /// Delete doctor (Admin only)
    /// </summary>
    [HttpDelete("{id}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> DeleteDoctor(int id)
    {
        var success = await _doctorService.DeleteDoctorAsync(id);
        if (!success)
            return NotFound();

        return NoContent();
    }
}
