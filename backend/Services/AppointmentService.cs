using HospitalManagementSystem.Data;
using HospitalManagementSystem.DTOs;
using HospitalManagementSystem.Models;
using Microsoft.EntityFrameworkCore;

namespace HospitalManagementSystem.Services;

public interface IAppointmentService
{
    Task<List<AppointmentResponseDto>> GetAllAppointmentsAsync();
    Task<List<AppointmentResponseDto>> GetAppointmentsByDoctorAsync(int doctorId);
    Task<List<AppointmentResponseDto>> GetAppointmentsByPatientAsync(int patientId);
    Task<AppointmentResponseDto?> GetAppointmentByIdAsync(int id);
    Task<AppointmentResponseDto> CreateAppointmentAsync(CreateAppointmentDto dto);
    Task<AppointmentResponseDto?> UpdateAppointmentAsync(int id, UpdateAppointmentDto dto);
    Task<bool> DeleteAppointmentAsync(int id);
}

public class AppointmentService : IAppointmentService
{
    private readonly AppDbContext _context;

    public AppointmentService(AppDbContext context)
    {
        _context = context;
    }

    public async Task<List<AppointmentResponseDto>> GetAllAppointmentsAsync()
    {
        return await _context.Appointments
            .AsNoTracking()
            .Include(a => a.Doctor)
            .ThenInclude(d => d!.User)
            .Include(a => a.Patient)
            .ThenInclude(p => p!.User)
            .Where(a => a.Doctor != null && a.Doctor.User != null && a.Patient != null && a.Patient.User != null)
            .Select(a => new AppointmentResponseDto
            {
                Id = a.Id,
                DoctorId = a.DoctorId,
                PatientId = a.PatientId,
                DoctorName = a.Doctor!.User!.FullName,
                PatientName = a.Patient!.User!.FullName,
                AppointmentDate = a.AppointmentDate,
                Status = a.Status,
                Notes = a.Notes
            })
            .ToListAsync();
    }

    public async Task<List<AppointmentResponseDto>> GetAppointmentsByDoctorAsync(int doctorId)
    {
        return await _context.Appointments
            .AsNoTracking()
            .Include(a => a.Doctor)
            .ThenInclude(d => d!.User)
            .Include(a => a.Patient)
            .ThenInclude(p => p!.User)
            .Where(a => a.DoctorId == doctorId && a.Doctor != null && a.Doctor.User != null && a.Patient != null && a.Patient.User != null)
            .Select(a => new AppointmentResponseDto
            {
                Id = a.Id,
                DoctorId = a.DoctorId,
                PatientId = a.PatientId,
                DoctorName = a.Doctor!.User!.FullName,
                PatientName = a.Patient!.User!.FullName,
                AppointmentDate = a.AppointmentDate,
                Status = a.Status,
                Notes = a.Notes
            })
            .ToListAsync();
    }

    public async Task<List<AppointmentResponseDto>> GetAppointmentsByPatientAsync(int patientId)
    {
        return await _context.Appointments
            .AsNoTracking()
            .Include(a => a.Doctor)
            .ThenInclude(d => d!.User)
            .Include(a => a.Patient)
            .ThenInclude(p => p!.User)
            .Where(a => a.PatientId == patientId && a.Doctor != null && a.Doctor.User != null && a.Patient != null && a.Patient.User != null)
            .Select(a => new AppointmentResponseDto
            {
                Id = a.Id,
                DoctorId = a.DoctorId,
                PatientId = a.PatientId,
                DoctorName = a.Doctor!.User!.FullName,
                PatientName = a.Patient!.User!.FullName,
                AppointmentDate = a.AppointmentDate,
                Status = a.Status,
                Notes = a.Notes
            })
            .ToListAsync();
    }

    public async Task<AppointmentResponseDto?> GetAppointmentByIdAsync(int id)
    {
        return await _context.Appointments
            .AsNoTracking()
            .Include(a => a.Doctor)
            .ThenInclude(d => d!.User)
            .Include(a => a.Patient)
            .ThenInclude(p => p!.User)
            .Where(a => a.Id == id && a.Doctor != null && a.Doctor.User != null && a.Patient != null && a.Patient.User != null)
            .Select(a => new AppointmentResponseDto
            {
                Id = a.Id,
                DoctorId = a.DoctorId,
                PatientId = a.PatientId,
                DoctorName = a.Doctor!.User!.FullName,
                PatientName = a.Patient!.User!.FullName,
                AppointmentDate = a.AppointmentDate,
                Status = a.Status,
                Notes = a.Notes
            })
            .FirstOrDefaultAsync();
    }

    public async Task<AppointmentResponseDto> CreateAppointmentAsync(CreateAppointmentDto dto)
    {
        var appointment = new Appointment
        {
            DoctorId = dto.DoctorId,
            PatientId = dto.PatientId,
            AppointmentDate = dto.AppointmentDate,
            Status = "Pending",
            Notes = dto.Notes
        };

        _context.Appointments.Add(appointment);
        await _context.SaveChangesAsync();

        return new AppointmentResponseDto
        {
            Id = appointment.Id,
            DoctorId = appointment.DoctorId,
            PatientId = appointment.PatientId,
            DoctorName = "",
            PatientName = "",
            AppointmentDate = appointment.AppointmentDate,
            Status = appointment.Status,
            Notes = appointment.Notes
        };
    }

    public async Task<AppointmentResponseDto?> UpdateAppointmentAsync(int id, UpdateAppointmentDto dto)
    {
        var appointment = _context.Appointments.FirstOrDefault(a => a.Id == id);
        if (appointment == null)
            return null;

        if (dto.AppointmentDate.HasValue)
            appointment.AppointmentDate = dto.AppointmentDate.Value;
        if (!string.IsNullOrEmpty(dto.Status))
            appointment.Status = dto.Status;
        if (!string.IsNullOrEmpty(dto.Notes))
            appointment.Notes = dto.Notes;

        await _context.SaveChangesAsync();

        return await GetAppointmentByIdAsync(id);
    }

    public async Task<bool> DeleteAppointmentAsync(int id)
    {
        var appointment = _context.Appointments.FirstOrDefault(a => a.Id == id);
        if (appointment == null)
            return false;

        _context.Appointments.Remove(appointment);
        await _context.SaveChangesAsync();

        return true;
    }
}
