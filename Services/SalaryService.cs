using Backend.Data;
using Backend.Models;
using Microsoft.EntityFrameworkCore;

namespace Backend.Services;

public class SalaryService : ISalaryService
{
    private readonly ApplicationDbContext _context;

    public SalaryService(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<SalaryModel?> GetLatestSalaryForEmployeeAsync(int employeeId)
    {
        return await _context.Salaries
            .Where(s => s.EmployeeID == employeeId)
            .OrderByDescending(s => s.Timestamp)
            .FirstOrDefaultAsync();
    }

    public async Task<List<SalaryModel>> GetSalaryHistoryAsync(int employeeId)
    {
        return await _context.Salaries
            .Where(s => s.EmployeeID == employeeId)
            .OrderByDescending(s => s.Timestamp)
            .ToListAsync();
    }

    public async Task<SalaryModel> AddSalaryAsync(SalaryModel salary)
    {
        salary.Timestamp = DateTime.UtcNow;
        _context.Salaries.Add(salary);
        await _context.SaveChangesAsync();
        return salary;
    }
}
