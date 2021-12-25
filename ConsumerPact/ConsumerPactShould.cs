using System.Collections.Generic;
using Consumer;
using NFluent;
using PactNet.Mocks.MockHttpService;
using PactNet.Mocks.MockHttpService.Models;
using Xunit;

namespace ConsumerPact
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
        public void Should_validate_one_employee()
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

                                    Body = new Employee { Id = 1, Name = "Parker", City = "NY", Summary = "Peter Parker is the secret identity of the character Spider-Man." }
                                });

            var employee = new EmployeeAdapter(_mockProviderServiceBaseUri)
                .LookForEmployeeById(employeeId).GetAwaiter().GetResult();

            Check.That(employee.Id).IsEqualTo(employee.Id);
            Check.That(employee.Name).IsEqualTo("Parker");
            Check.That(employee.City).IsEqualTo("NY");
            Check.That(employee.Summary).IsEqualTo("Peter Parker is the secret identity of the character Spider-Man.");

            _mockProviderService.VerifyInteractions();
        }
    }
}
