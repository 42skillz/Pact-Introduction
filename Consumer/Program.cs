using System;
using System.Linq;
using System.Threading.Tasks;

namespace Consumer
{
    internal static class Program
    {
        static async Task Main(string[] args)
        {
            var employeeId = 1;
            var baseUri = "http://localhost:5000";

            if (args.Length > 1)
            {
                employeeId = int.Parse(args[0]);
                baseUri = args[1];
            }

            var employeeRepository = new EmployeeAdapter(baseUri);
            
            // Retrieve one employee
            var employee = await employeeRepository.LookForEmployeeById(employeeId);
            Console.WriteLine($"Retrieve employee: ID: {employee.Id} Name: {employee.Name} City: {employee.City}.");

            // Retrieve all employees
            var employees = await employeeRepository.GetAllEmployees();
            Console.WriteLine($"Retrieve all employees: {string.Join(", ", employees.Select( e => e.Name))}.");
        }
    }
}
