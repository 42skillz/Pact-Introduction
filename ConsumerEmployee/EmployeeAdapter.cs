using System;
using System.Net.Http;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace ConsumerEmployee
{
    public class EmployeeAdapter
    {
        private readonly string _uriEmployeeService;

        public EmployeeAdapter(string uriEmployeeService)
        {
            _uriEmployeeService = uriEmployeeService;
        }
        public async Task<Employee> LookForEmployee(int id)
        {
            using var client = new HttpClient();

            client.BaseAddress = new Uri(_uriEmployeeService);

            var response = await client.GetAsync($"/employees/{id}");

            return JsonConvert.DeserializeObject<Employee>(await response.Content.ReadAsStringAsync());
        }
    }
}