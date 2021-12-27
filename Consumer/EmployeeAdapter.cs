using System;
using System.Net.Http;
using System.Threading.Tasks;

namespace Consumer
{
    public class EmployeeAdapter
    {
        private readonly string _uriEmployeeService;

        public EmployeeAdapter(string uriEmployeeService)
        {
            _uriEmployeeService = uriEmployeeService;
        }

        public async Task<HttpResponseMessage> GetEmployeeById(int id)
        {
            using var client = new HttpClient();

            client.BaseAddress = new Uri(_uriEmployeeService);
            try
            {
                return await client.GetAsync($"api/employees/{id}");
            }
            catch (Exception e)
            {
                throw new Exception("There was a problem connecting to Provider API.", e);
            }
            
        }

        public async Task<HttpResponseMessage> GetEmployees()
        {
            using var client = new HttpClient();

            client.BaseAddress = new Uri(_uriEmployeeService);

            try
            {
                return await client.GetAsync($"api/employees");
            }
            catch (Exception e)
            {
                throw new Exception("There was a problem connecting to Provider API.", e);
            }
            
        }
    }
}