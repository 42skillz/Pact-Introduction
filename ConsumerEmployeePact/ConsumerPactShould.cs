using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using ConsumerEmployee;
using Newtonsoft.Json;
using NFluent;
using PactNet.Matchers;
using PactNet.Mocks.MockHttpService;
using PactNet.Mocks.MockHttpService.Models;
using Xunit;

namespace ConsumerEmployeePact
{
    public class ConsumerPactShould : IClassFixture<ConsumerPactClassFixture>
    {
        private readonly IMockProviderService _mockProviderService;
        private readonly string _mockProviderServiceBaseUri;

        public ConsumerPactShould(ConsumerPactClassFixture fixture)
        {
            _mockProviderService = fixture.MockProviderService;
            _mockProviderService.ClearInteractions();
            _mockProviderServiceBaseUri = fixture.MockProviderServiceBaseUri;
        }

        [Fact]
        public async Task Validate_one_employee_by_id()
        {
            const int employeeId = 1;

            _mockProviderService.Given("There are employees")
                .UponReceiving("One employee")
                // When
                .With(new ProviderServiceRequest
                {
                    Method = HttpVerb.Get,
                    Path = $"/api/superheroes/{employeeId}",
                })
                // Then
                .WillRespondWith(new ProviderServiceResponse
                {
                    Status = 200,

                    Headers = new Dictionary<string, object>
                    {
                        ["Content-Type"] = "application/json; charset=utf-8"
                    },

                    Body = new Employee
                    {
                        Id = 1, Name = "Parker", City = "NY"
                    }
                });

            var httpResponseMessage = new EmployeeAdapter(_mockProviderServiceBaseUri)
                .GetEmployeeById(employeeId).GetAwaiter().GetResult();
            
            if (httpResponseMessage.IsSuccessStatusCode)
            {
                AssertFirstEmployee(await AdaptEmployee(httpResponseMessage));
            }
            
            _mockProviderService.VerifyInteractions();
        }

        [Fact]
        public void Validate_all_employees()
        {
            _mockProviderService.Given("There are employees")
                .UponReceiving("All employees")
                .With(new ProviderServiceRequest
                {
                    Method = HttpVerb.Get,
                    Path = $"/api/superheroes",
                })
                .WillRespondWith(new ProviderServiceResponse
                {
                    Status = 200,

                    Headers = new Dictionary<string, object>
                    {
                        ["Content-Type"] = "application/json; charset=utf-8"
                    },

                    Body = Match.MinType(new 
                    {
                        Id = 1, 
                        Name = Match.Type("string"), 
                        City = Match.Type("string")
                    },1
                    )
                });

            var httpResponseMessage = new EmployeeAdapter(_mockProviderServiceBaseUri)
                .GetEmployees().GetAwaiter().GetResult();

            Check.That((int)httpResponseMessage.StatusCode).IsEqualTo(200);
            
            // If the provider works => generate pact file
            _mockProviderService.VerifyInteractions();
        }

        private static void AssertFirstEmployee(Employee employee)
        {
            Check.That(employee.Id).IsEqualTo(employee.Id);
            Check.That(employee.Name).IsEqualTo("Parker");
            Check.That(employee.City).IsEqualTo("NY");
        }

        private static async Task<Employee> AdaptEmployee(HttpResponseMessage httpResponseMessage)
        {
            return JsonConvert.DeserializeObject<Employee>(await httpResponseMessage.Content.ReadAsStringAsync());
        }
    }
}
