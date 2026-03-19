using HospitalManagementSystem.Data;
using HospitalManagementSystem.DTOs;
using HospitalManagementSystem.Models;
using Microsoft.EntityFrameworkCore;

namespace HospitalManagementSystem.Services;

public interface IDepartmentService
{
    Task<List<DepartmentResponseDto>> GetAllDepartmentsAsync();
    Task<DepartmentResponseDto?> GetDepartmentByIdAsync(int id);
    Task<DepartmentResponseDto> CreateDepartmentAsync(CreateDepartmentDto dto);
    Task<DepartmentResponseDto?> UpdateDepartmentAsync(int id, UpdateDepartmentDto dto);
    Task<bool> DeleteDepartmentAsync(int id);
}

public class DepartmentService : IDepartmentService
{
    private readonly AppDbContext _context;

    public DepartmentService(AppDbContext context)
    {
        _context = context;
    }

    public async Task<List<DepartmentResponseDto>> GetAllDepartmentsAsync()
    {
        return await _context.Departments
            .AsNoTracking()
            .Select(d => new DepartmentResponseDto
            {
                Id = d.Id,
                Name = d.Name,
                Description = d.Description
            })
            .ToListAsync();
    }

    public async Task<DepartmentResponseDto?> GetDepartmentByIdAsync(int id)
    {
        return await _context.Departments
            .AsNoTracking()
            .Where(d => d.Id == id)
            .Select(d => new DepartmentResponseDto
            {
                Id = d.Id,
                Name = d.Name,
                Description = d.Description
            })
            .FirstOrDefaultAsync();
    }

    public async Task<DepartmentResponseDto> CreateDepartmentAsync(CreateDepartmentDto dto)
    {
        var department = new Department
        {
            Name = dto.Name,
            Description = dto.Description
        };

        _context.Departments.Add(department);
        await _context.SaveChangesAsync();

        return new DepartmentResponseDto
        {
            Id = department.Id,
            Name = department.Name,
            Description = department.Description
        };
    }

    public async Task<DepartmentResponseDto?> UpdateDepartmentAsync(int id, UpdateDepartmentDto dto)
    {
        var department = _context.Departments.FirstOrDefault(d => d.Id == id);
        if (department == null)
            return null;

        if (!string.IsNullOrEmpty(dto.Name))
            department.Name = dto.Name;
        if (!string.IsNullOrEmpty(dto.Description))
            department.Description = dto.Description;

        await _context.SaveChangesAsync();

        return await GetDepartmentByIdAsync(id);
    }

    public async Task<bool> DeleteDepartmentAsync(int id)
    {
        var department = _context.Departments.FirstOrDefault(d => d.Id == id);
        if (department == null)
            return false;

        _context.Departments.Remove(department);
        await _context.SaveChangesAsync();

        return true;
    }
}
