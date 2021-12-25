using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace Consumer
{
    public class EmployeeAdapter
    {
        private readonly string _uriEmployeeService;

        public EmployeeAdapter(string uriEmployeeService)
        {
            _uriEmployeeService = uriEmployeeService;
        }
        public async Task<Employee> LookForEmployeeById(int id)
        {
            using var client = new HttpClient();

            client.BaseAddress = new Uri(_uriEmployeeService);

            var response = await client.GetAsync($"api/employees/{id}");

            return JsonConvert.DeserializeObject<Employee>(await response.Content.ReadAsStringAsync());
        }

        public async Task<IEnumerable<Employee>> GetAllEmployees()
        {
            using var client = new HttpClient();

            client.BaseAddress = new Uri(_uriEmployeeService);

            var response = await client.GetAsync($"api/employees");

            return JsonConvert.DeserializeObject<List<Employee>>(await response.Content.ReadAsStringAsync());
        }
    }
}