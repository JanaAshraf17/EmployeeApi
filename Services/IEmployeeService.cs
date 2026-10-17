using EmployeeApi.Models;

namespace EmployeeApi.Services
{
    public interface IEmployeeService
    {
        Task<List<Employee>> GetAllAsync(CancellationToken ct);
        Task<Employee?> GetByIdAsync(int id, CancellationToken ct);
        Task<Employee> CreateAsync(Employee employee, CancellationToken ct);
        Task<bool> UpdateAsync(int id, Employee employee, CancellationToken ct);
        Task<bool> DeleteAsync(int id, CancellationToken ct);
    }
}