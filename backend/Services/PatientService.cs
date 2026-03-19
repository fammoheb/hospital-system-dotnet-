using HospitalManagementSystem.Data;
using HospitalManagementSystem.DTOs;
using HospitalManagementSystem.Models;
using Microsoft.EntityFrameworkCore;

namespace HospitalManagementSystem.Services;

public interface IPatientService
{
    Task<List<PatientResponseDto>> GetAllPatientsAsync();
    Task<PatientResponseDto?> GetPatientByIdAsync(int id);
    Task<PatientResponseDto> CreatePatientAsync(CreatePatientDto dto);
    Task<PatientResponseDto?> UpdatePatientAsync(int id, UpdatePatientDto dto);
    Task<bool> DeletePatientAsync(int id);
}

public class PatientService : IPatientService
{
    private readonly AppDbContext _context;

    public PatientService(AppDbContext context)
    {
        _context = context;
    }

    public async Task<List<PatientResponseDto>> GetAllPatientsAsync()
    {
        return await _context.Patients
            .AsNoTracking()
            .Include(p => p.User)
            .Where(p => p.User != null)
            .Select(p => new PatientResponseDto
            {
                Id = p.Id,
                UserId = p.UserId,
                PatientName = p.User!.FullName,
                DateOfBirth = p.DateOfBirth,
                BloodType = p.BloodType,
                Address = p.Address
            })
            .ToListAsync();
    }

    public async Task<PatientResponseDto?> GetPatientByIdAsync(int id)
    {
        return await _context.Patients
            .AsNoTracking()
            .Include(p => p.User)
            .Where(p => p.Id == id && p.User != null)
            .Select(p => new PatientResponseDto
            {
                Id = p.Id,
                UserId = p.UserId,
                PatientName = p.User!.FullName,
                DateOfBirth = p.DateOfBirth,
                BloodType = p.BloodType,
                Address = p.Address
            })
            .FirstOrDefaultAsync();
    }

    public async Task<PatientResponseDto> CreatePatientAsync(CreatePatientDto dto)
    {
        var patient = new Patient
        {
            UserId = dto.UserId,
            DateOfBirth = dto.DateOfBirth,
            BloodType = dto.BloodType,
            Address = dto.Address
        };

        _context.Patients.Add(patient);
        await _context.SaveChangesAsync();

        return new PatientResponseDto
        {
            Id = patient.Id,
            UserId = patient.UserId,
            PatientName = "",
            DateOfBirth = patient.DateOfBirth,
            BloodType = patient.BloodType,
            Address = patient.Address
        };
    }

    public async Task<PatientResponseDto?> UpdatePatientAsync(int id, UpdatePatientDto dto)
    {
        var patient = _context.Patients.FirstOrDefault(p => p.Id == id);
        if (patient == null)
            return null;

        if (dto.DateOfBirth.HasValue)
            patient.DateOfBirth = dto.DateOfBirth.Value;
        if (!string.IsNullOrEmpty(dto.BloodType))
            patient.BloodType = dto.BloodType;
        if (!string.IsNullOrEmpty(dto.Address))
            patient.Address = dto.Address;

        await _context.SaveChangesAsync();

        return await GetPatientByIdAsync(id);
    }

    public async Task<bool> DeletePatientAsync(int id)
    {
        var patient = _context.Patients.FirstOrDefault(p => p.Id == id);
        if (patient == null)
            return false;

        _context.Patients.Remove(patient);
        await _context.SaveChangesAsync();

        return true;
    }
}
