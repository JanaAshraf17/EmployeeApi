# Performance Measurement Report: Sequential vs. Parallel Execution

## 1. Objective
To evaluate and compare the execution time and performance impact between sequential and parallel asynchronous database/I/O operations using ASP.NET Core, Entity Framework Core, and `Stopwatch`.

## 2. Methodology
- **Sequential Execution:** Two independent tasks (fetching Employees and Departments) were executed one after another, each with a simulated 1-second delay.
- **Parallel Execution:** Both tasks were initiated concurrently using `Task.WhenAll` with independent `DbContext` instances (via `IDbContextFactory`) to avoid threading and concurrency violations.
- **Measurement Tool:** `System.Diagnostics.Stopwatch` was utilized inside the `performance-test` API endpoint.

## 3. Results & Observations
| Execution Type | Approximate Total Time | Description |
| :--- | :--- | :--- |
| **Sequential** | ~2000 ms | Total time equals the sum of all individual operations (Task 1 time + Task 2 time). |
| **Parallel** | ~1000 ms | Total time equals roughly the duration of the longest single operation. |

## 4. Conclusion
Parallel execution drastically improves throughput and reduces response times for independent I/O-bound operations (such as calling multiple external APIs or independent database queries). However, proper isolation (such as using separate `DbContext` instances) is crucial to prevent EF Core concurrency exceptions.