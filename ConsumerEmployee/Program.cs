using System;
using System.Threading.Tasks;

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
            
            var employee = await employeeRepository.LookForEmployee(employeeId);
            
            Console.WriteLine(employee.Summary);
        }
    }
}
