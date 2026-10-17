using EmployeeApi.Data;
using EmployeeApi.Models;
using Microsoft.EntityFrameworkCore;

namespace EmployeeApi.Services
{
    public class EmployeeService : IEmployeeService
    {
        private readonly AppDbContext _context;

        public EmployeeService(AppDbContext context)
        {
            _context = context;
        }

        public async Task<List<Employee>> GetAllAsync(CancellationToken ct)
        {
            return await _context.Employees.ToListAsync(ct);
        }

        public async Task<Employee?> GetByIdAsync(int id, CancellationToken ct)
        {
            return await _context.Employees.FirstOrDefaultAsync(e => e.Id == id, ct);
        }

        public async Task<Employee> CreateAsync(Employee employee, CancellationToken ct)
        {
            await _context.Employees.AddAsync(employee, ct);
            await _context.SaveChangesAsync(ct);
            return employee;
        }

        public async Task<bool> UpdateAsync(int id, Employee employee, CancellationToken ct)
        {
            var existing = await _context.Employees.FirstOrDefaultAsync(e => e.Id == id, ct);
            if (existing is null) return false;

            existing.Name = employee.Name;
            existing.Position = employee.Position;
            existing.Salary = employee.Salary;
            existing.DepartmentId = employee.DepartmentId;

            await _context.SaveChangesAsync(ct);
            return true;
        }

        public async Task<bool> DeleteAsync(int id, CancellationToken ct)
        {
            var existing = await _context.Employees.FirstOrDefaultAsync(e => e.Id == id, ct);
            if (existing is null) return false;

            _context.Employees.Remove(existing);
            await _context.SaveChangesAsync(ct);
            return true;
        }
    }
}