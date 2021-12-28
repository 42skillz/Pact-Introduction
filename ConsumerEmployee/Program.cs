using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace ConsumerEmployee
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
            var response = await employeeRepository.GetEmployeeById(employeeId);
            Employee employee = JsonConvert.DeserializeObject<Employee>(await response.Content.ReadAsStringAsync());
            Console.WriteLine($"Retrieve employee: ID: {employee.Id} Name: {employee.Name} City: {employee.City}.");

            // Retrieve all employees
            response = await employeeRepository.GetEmployees();
            IEnumerable<Employee> employees = JsonConvert.DeserializeObject<List<Employee>>(await response.Content.ReadAsStringAsync());
            Console.WriteLine($"Retrieve all employees: {string.Join(", ", employees.Select( e => e.Name))}.");
        }
    }
}
