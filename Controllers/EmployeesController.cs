using System.Diagnostics;
using EmployeeApi.Data;
using EmployeeApi.Models;
using EmployeeApi.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace EmployeeApi.Controllers
{
    [ApiController]
    [Route("api/employees")]
    public class EmployeesController : ControllerBase
    {
        private readonly IEmployeeService _service;
        private readonly IDbContextFactory<AppDbContext> _contextFactory;

        public EmployeesController(IEmployeeService service, IDbContextFactory<AppDbContext> contextFactory)
        {
            _service = service;
            _contextFactory = contextFactory;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll(CancellationToken ct)
        {
            var employees = await _service.GetAllAsync(ct);
            return Ok(employees);
        }

        [HttpGet("{id:int}")]
        public async Task<IActionResult> GetById(int id, CancellationToken ct)
        {
            var employee = await _service.GetByIdAsync(id, ct);
            if (employee is null) return NotFound();
            return Ok(employee);
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] Employee employee, CancellationToken ct)
        {
            var created = await _service.CreateAsync(employee, ct);
            return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
        }

        [HttpPut("{id:int}")]
        public async Task<IActionResult> Update(int id, [FromBody] Employee employee, CancellationToken ct)
        {
            var success = await _service.UpdateAsync(id, employee, ct);
            if (!success) return NotFound();
            return NoContent();
        }

        [HttpDelete("{id:int}")]
        public async Task<IActionResult> Delete(int id, CancellationToken ct)
        {
            var success = await _service.DeleteAsync(id, ct);
            if (!success) return NotFound();
            return NoContent();
        }

         
        // Performance Measurement
         
        [HttpGet("performance-test")]
        public async Task<IActionResult> PerformanceTest(CancellationToken ct)
        {
            var sw = new Stopwatch();

            //  Sequential Execution
            sw.Start();
            using (var ctx1 = _contextFactory.CreateDbContext())
            {
                await Task.Delay(1000, ct); // Simulate external/DB delay of 1 second
                await ctx1.Employees.ToListAsync(ct);
            }
            using (var ctx2 = _contextFactory.CreateDbContext())
            {
                await Task.Delay(1000, ct); // Simulate another 1 second delay
                await ctx2.Departments.ToListAsync(ct);
            }
            sw.Stop();
            var sequentialTimeMs = sw.ElapsedMilliseconds;

            //  Parallel Execution (Using Task.WhenAll)
            sw.Restart();
            var task1 = Task.Run(async () =>
            {
                using var ctx = _contextFactory.CreateDbContext();
                await Task.Delay(1000, ct);
                return await ctx.Employees.ToListAsync(ct);
            }, ct);

            var task2 = Task.Run(async () =>
            {
                using var ctx = _contextFactory.CreateDbContext();
                await Task.Delay(1000, ct);
                return await ctx.Departments.ToListAsync(ct);
            }, ct);

            await Task.WhenAll(task1, task2);
            sw.Stop();
            var parallelTimeMs = sw.ElapsedMilliseconds;

            return Ok(new
            {
                SequentialTimeMs = sequentialTimeMs,
                ParallelTimeMs = parallelTimeMs,
                TimeDifferenceMs = sequentialTimeMs - parallelTimeMs,
                Message = "Parallel execution significantly improved performance by reducing total execution time for independent I/O operations!"
            });
        }
    }
}