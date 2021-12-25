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
            
            var employee = await employeeRepository.LookForEmployeeById(employeeId);
            var employees = await employeeRepository.GetAllEmployees();
            
            Console.WriteLine($"Retrieve employee: {employee.Id}");
            Console.WriteLine($"Retrieve all employees: {employees.Count()}");
        }
    }
}
