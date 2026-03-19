using HospitalManagementSystem.DTOs;
using HospitalManagementSystem.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HospitalManagementSystem.Controllers;

[Route("api/[controller]")]
[ApiController]
[Authorize]
public class PatientsController : ControllerBase
{
    private readonly IPatientService _patientService;

    public PatientsController(IPatientService patientService)
    {
        _patientService = patientService;
    }

    /// <summary>
    /// Get all patients (Admin, Doctor)
    /// </summary>
    [HttpGet]
    [Authorize(Roles = "Admin,Doctor")]
    public async Task<IActionResult> GetAllPatients()
    {
        var patients = await _patientService.GetAllPatientsAsync();
        return Ok(patients);
    }

    /// <summary>
    /// Get patient by ID (Admin, Doctor)
    /// </summary>
    [HttpGet("{id}")]
    [Authorize(Roles = "Admin,Doctor")]
    public async Task<IActionResult> GetPatientById(int id)
    {
        var patient = await _patientService.GetPatientByIdAsync(id);
        if (patient == null)
            return NotFound();

        return Ok(patient);
    }

    /// <summary>
    /// Create a new patient (Admin only)
    /// </summary>
    [HttpPost]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> CreatePatient([FromBody] CreatePatientDto dto)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        var patient = await _patientService.CreatePatientAsync(dto);
        return CreatedAtAction(nameof(GetPatientById), new { id = patient.Id }, patient);
    }

    /// <summary>
    /// Update patient (Admin, Patient)
    /// </summary>
    [HttpPut("{id}")]
    [Authorize(Roles = "Admin,Patient")]
    public async Task<IActionResult> UpdatePatient(int id, [FromBody] UpdatePatientDto dto)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        var patient = await _patientService.UpdatePatientAsync(id, dto);
        if (patient == null)
            return NotFound();

        return Ok(patient);
    }

    /// <summary>
    /// Delete patient (Admin only)
    /// </summary>
    [HttpDelete("{id}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> DeletePatient(int id)
    {
        var success = await _patientService.DeletePatientAsync(id);
        if (!success)
            return NotFound();

        return NoContent();
    }
}
