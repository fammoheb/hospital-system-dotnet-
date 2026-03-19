using HospitalManagementSystem.DTOs;
using HospitalManagementSystem.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HospitalManagementSystem.Controllers;

[Route("api/[controller]")]
[ApiController]
[Authorize]
public class AppointmentsController : ControllerBase
{
    private readonly IAppointmentService _appointmentService;

    public AppointmentsController(IAppointmentService appointmentService)
    {
        _appointmentService = appointmentService;
    }

    /// <summary>
    /// Get all appointments (Admin, Doctor, Patient)
    /// </summary>
    [HttpGet]
    [Authorize(Roles = "Admin,Doctor,Patient")]
    public async Task<IActionResult> GetAllAppointments()
    {
        var appointments = await _appointmentService.GetAllAppointmentsAsync();
        return Ok(appointments);
    }

    /// <summary>
    /// Get appointments by doctor ID (Admin, Doctor, Patient)
    /// </summary>
    [HttpGet("doctor/{doctorId}")]
    [Authorize(Roles = "Admin,Doctor,Patient")]
    public async Task<IActionResult> GetAppointmentsByDoctor(int doctorId)
    {
        var appointments = await _appointmentService.GetAppointmentsByDoctorAsync(doctorId);
        return Ok(appointments);
    }

    /// <summary>
    /// Get appointments by patient ID (Admin, Doctor, Patient)
    /// </summary>
    [HttpGet("patient/{patientId}")]
    [Authorize(Roles = "Admin,Doctor,Patient")]
    public async Task<IActionResult> GetAppointmentsByPatient(int patientId)
    {
        var appointments = await _appointmentService.GetAppointmentsByPatientAsync(patientId);
        return Ok(appointments);
    }

    /// <summary>
    /// Get appointment by ID (Admin, Doctor, Patient)
    /// </summary>
    [HttpGet("{id}")]
    [Authorize(Roles = "Admin,Doctor,Patient")]
    public async Task<IActionResult> GetAppointmentById(int id)
    {
        var appointment = await _appointmentService.GetAppointmentByIdAsync(id);
        if (appointment == null)
            return NotFound();

        return Ok(appointment);
    }

    /// <summary>
    /// Create a new appointment (Admin, Patient)
    /// </summary>
    [HttpPost]
    [Authorize(Roles = "Admin,Patient")]
    public async Task<IActionResult> CreateAppointment([FromBody] CreateAppointmentDto dto)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        var appointment = await _appointmentService.CreateAppointmentAsync(dto);
        return CreatedAtAction(nameof(GetAppointmentById), new { id = appointment.Id }, appointment);
    }

    /// <summary>
    /// Update appointment (Admin, Doctor)
    /// </summary>
    [HttpPut("{id}")]
    [Authorize(Roles = "Admin,Doctor")]
    public async Task<IActionResult> UpdateAppointment(int id, [FromBody] UpdateAppointmentDto dto)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        var appointment = await _appointmentService.UpdateAppointmentAsync(id, dto);
        if (appointment == null)
            return NotFound();

        return Ok(appointment);
    }

    /// <summary>
    /// Delete appointment (Admin only)
    /// </summary>
    [HttpDelete("{id}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> DeleteAppointment(int id)
    {
        var success = await _appointmentService.DeleteAppointmentAsync(id);
        if (!success)
            return NotFound();

        return NoContent();
    }
}
