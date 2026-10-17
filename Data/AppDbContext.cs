using EmployeeApi.Models;
using Microsoft.EntityFrameworkCore;

namespace EmployeeApi.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        public DbSet<Employee> Employees => Set<Employee>();
        public DbSet<Department> Departments => Set<Department>();

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // 1. Fix decimal precision for Salary property
            modelBuilder.Entity<Employee>()
                .Property(e => e.Salary)
                .HasPrecision(18, 2);

            // 2. Seed Data for Departments
            modelBuilder.Entity<Department>().HasData(
                new Department { Id = 1, Name = "HR" },
                new Department { Id = 2, Name = "IT" }
            );

            // 3. Seed Data for Employees
            modelBuilder.Entity<Employee>().HasData(
                new Employee { Id = 1, Name = "Jana Ashraf", Position = "Backend Developer", Salary = 12000, DepartmentId = 2 },
                new Employee { Id = 2, Name = "Ahmed Ali", Position = "HR Specialist", Salary = 8000, DepartmentId = 1 }
            );
        }
    }
}