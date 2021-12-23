using System.Collections.Generic;
using ConsumerEmployee;
using NFluent;
using PactNet.Mocks.MockHttpService;
using PactNet.Mocks.MockHttpService.Models;
using Xunit;

namespace ConsumerTest
{
    public class ConsumerPactTests : IClassFixture<ConsumerPactClassFixture>
    {
        private readonly IMockProviderService _mockProviderService;
        private readonly string _mockProviderServiceBaseUri;

        public ConsumerPactTests(ConsumerPactClassFixture fixture)
        {
            _mockProviderService = fixture.MockProviderService;
            _mockProviderService.ClearInteractions(); //NOTE: Clears any previously registered interactions before the test is run
            _mockProviderServiceBaseUri = fixture.MockProviderServiceBaseUri;
        }


        [Fact]
        public void Should_retrieve_employee_with_status_ok()
        {
            var employeeId = 1;

            _mockProviderService.Given("There is data")
                                .UponReceiving("A valid GET employee")
                                .With(new ProviderServiceRequest
                                {
                                    Method = HttpVerb.Get,
                                    Path = $"/api/employees/{employeeId}",
                                })
                                .WillRespondWith(new ProviderServiceResponse
                                {
                                    Status = 200,
                                    
                                    Headers = new Dictionary<string, object>
                                    {
                                        ["Content-Type"] = "application/json; charset=utf-8"
                                    },

                                    Body = new Employee { Id = 1, Name = "Stacy", City = "NY", Summary = "Captain Stacy"}
                                });

            var employee = new EmployeeAdapter(_mockProviderServiceBaseUri)
                .LookForEmployee(employeeId).GetAwaiter().GetResult();

            Check.That(employee.Id).IsEqualTo(employeeId);
            Check.That(employee.Name).IsEqualTo("Stacy");
            Check.That(employee.City).IsEqualTo("NY");
            Check.That(employee.Summary).IsEqualTo("Captain Stacy");

            _mockProviderService.VerifyInteractions();
        }
    }
}
