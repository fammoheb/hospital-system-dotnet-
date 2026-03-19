using HospitalManagementSystem.Data;
using HospitalManagementSystem.DTOs;
using HospitalManagementSystem.Models;
using Microsoft.EntityFrameworkCore;

namespace HospitalManagementSystem.Services;

public interface IDoctorService
{
    Task<List<DoctorResponseDto>> GetAllDoctorsAsync();
    Task<DoctorResponseDto?> GetDoctorByIdAsync(int id);
    Task<DoctorResponseDto> CreateDoctorAsync(CreateDoctorDto dto);
    Task<DoctorResponseDto?> UpdateDoctorAsync(int id, UpdateDoctorDto dto);
    Task<bool> DeleteDoctorAsync(int id);
}

public class DoctorService : IDoctorService
{
    private readonly AppDbContext _context;

    public DoctorService(AppDbContext context)
    {
        _context = context;
    }

    public async Task<List<DoctorResponseDto>> GetAllDoctorsAsync()
    {
        return await _context.Doctors
            .AsNoTracking()
            .Include(d => d.User)
            .Include(d => d.Department)
            .Where(d => d.User != null)
            .Select(d => new DoctorResponseDto
            {
                Id = d.Id,
                UserId = d.UserId,
                DepartmentId = d.DepartmentId,
                LicenseNumber = d.LicenseNumber,
                DoctorName = d.User!.FullName,
                Specialization = d.User.DoctorProfile != null ? d.User.DoctorProfile.Specialization : "",
                Bio = d.User.DoctorProfile != null ? d.User.DoctorProfile.Bio : "",
                YearsOfExperience = d.User.DoctorProfile != null ? d.User.DoctorProfile.YearsOfExperience : 0,
                DepartmentName = d.Department != null ? d.Department.Name : ""
            })
            .ToListAsync();
    }

    public async Task<DoctorResponseDto?> GetDoctorByIdAsync(int id)
    {
        return await _context.Doctors
            .AsNoTracking()
            .Include(d => d.User)
            .Include(d => d.Department)
            .Where(d => d.Id == id && d.User != null)
            .Select(d => new DoctorResponseDto
            {
                Id = d.Id,
                UserId = d.UserId,
                DepartmentId = d.DepartmentId,
                LicenseNumber = d.LicenseNumber,
                DoctorName = d.User!.FullName,
                Specialization = d.User.DoctorProfile != null ? d.User.DoctorProfile.Specialization : "",
                Bio = d.User.DoctorProfile != null ? d.User.DoctorProfile.Bio : "",
                YearsOfExperience = d.User.DoctorProfile != null ? d.User.DoctorProfile.YearsOfExperience : 0,
                DepartmentName = d.Department != null ? d.Department.Name : ""
            })
            .FirstOrDefaultAsync();
    }

    public async Task<DoctorResponseDto> CreateDoctorAsync(CreateDoctorDto dto)
    {
        var doctor = new Doctor
        {
            UserId = dto.UserId,
            DepartmentId = dto.DepartmentId,
            LicenseNumber = dto.LicenseNumber
        };

        _context.Doctors.Add(doctor);

        // Create or update DoctorProfile
        var doctorProfile = new DoctorProfile
        {
            UserId = dto.UserId,
            Specialization = dto.Specialization,
            Bio = dto.Bio,
            YearsOfExperience = dto.YearsOfExperience
        };

        _context.DoctorProfiles.Add(doctorProfile);
        await _context.SaveChangesAsync();

        return new DoctorResponseDto
        {
            Id = doctor.Id,
            UserId = doctor.UserId,
            DepartmentId = doctor.DepartmentId,
            LicenseNumber = doctor.LicenseNumber,
            DoctorName = "",
            Specialization = dto.Specialization,
            Bio = dto.Bio,
            YearsOfExperience = dto.YearsOfExperience,
            DepartmentName = ""
        };
    }

    public async Task<DoctorResponseDto?> UpdateDoctorAsync(int id, UpdateDoctorDto dto)
    {
        var doctor = _context.Doctors.Include(d => d.User).FirstOrDefault(d => d.Id == id);
        if (doctor == null)
            return null;

        if (dto.DepartmentId.HasValue)
            doctor.DepartmentId = dto.DepartmentId.Value;
        if (!string.IsNullOrEmpty(dto.LicenseNumber))
            doctor.LicenseNumber = dto.LicenseNumber;

        // Update DoctorProfile if needed
        var doctorProfile = _context.DoctorProfiles.FirstOrDefault(dp => dp.UserId == doctor.UserId);
        if (doctorProfile != null)
        {
            if (!string.IsNullOrEmpty(dto.Specialization))
                doctorProfile.Specialization = dto.Specialization;
            if (!string.IsNullOrEmpty(dto.Bio))
                doctorProfile.Bio = dto.Bio;
            if (dto.YearsOfExperience.HasValue)
                doctorProfile.YearsOfExperience = dto.YearsOfExperience.Value;
        }

        await _context.SaveChangesAsync();

        return await GetDoctorByIdAsync(id);
    }

    public async Task<bool> DeleteDoctorAsync(int id)
    {
        var doctor = _context.Doctors.FirstOrDefault(d => d.Id == id);
        if (doctor == null)
            return false;

        // Also delete associated DoctorProfile
        var doctorProfile = _context.DoctorProfiles.FirstOrDefault(dp => dp.UserId == doctor.UserId);
        if (doctorProfile != null)
            _context.DoctorProfiles.Remove(doctorProfile);

        _context.Doctors.Remove(doctor);
        await _context.SaveChangesAsync();

        return true;
    }
}
