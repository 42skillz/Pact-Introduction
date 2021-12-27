using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using Consumer;
using Newtonsoft.Json;
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
        public async Task Validate_one_employee_by_id()
        {
            const int employeeId = 1;

            _mockProviderService.Given("There is data")
                .UponReceiving("A valid GET employee")
                // When
                .With(new ProviderServiceRequest
                {
                    Method = HttpVerb.Get,
                    Path = $"/api/employees/{employeeId}",
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
                        Id = 1, Name = "Parker", City = "NY",
                        Summary = "Peter Parker is the secret identity of the character Spider-Man."
                    }
                });

            var httpResponseMessage = new EmployeeAdapter(_mockProviderServiceBaseUri)
                .GetEmployeeById(employeeId).GetAwaiter().GetResult();
            
            if (httpResponseMessage.IsSuccessStatusCode)
            {
                AssertEmployee(await AdaptEmployee(httpResponseMessage));
            }
            
            _mockProviderService.VerifyInteractions();
        }

        [Fact]
        public void Retrieve_all_employees()
        {
            _mockProviderService.Given("There is data")
                .UponReceiving("Retrieve all employees")
                .With(new ProviderServiceRequest
                {
                    Method = HttpVerb.Get,
                    Path = $"/api/employees",
                })
                .WillRespondWith(new ProviderServiceResponse
                {
                    Status = 200,

                    Headers = new Dictionary<string, object>
                    {
                        ["Content-Type"] = "application/json; charset=utf-8"
                    },

                    Body = GetAllEmployees()
                });

            var httpResponseMessage = new EmployeeAdapter(_mockProviderServiceBaseUri)
                .GetEmployees().GetAwaiter().GetResult();

            Check.That((int)httpResponseMessage.StatusCode).IsEqualTo(200);

            // If the provider works => generate pact file
            _mockProviderService.VerifyInteractions();
        }

        private static Employee[] GetAllEmployees()
        {
            return new[]
            {
                new Employee { Id = 1, Name = "Parker", City = "NY", Summary = "Peter Parker is the secret identity of the character Spider-Man."},
                new Employee { Id = 2, Name = "Stark", City = "NY", Summary = "A wealthy American business magnate, playboy, philanthropist, inventor and ingenious scientist."},
                new Employee { Id = 3, Name = "Clark", City = "NY", Summary = "He was found and adopted by farmers Jonathan and Martha Kent, who named him Clark Kent."},
                new Employee { Id = 4, Name = "Grimm", City = "NY", Summary = "Benjamin Jacob Grimm, also known as The Thing, is a fictional superhero appearing in American comic books published by Marvel Comics."},
                new Employee { Id = 5, Name = "Wayne", City = "Gotham City", Summary = "Bruce Wayne, a wealthy American playboy, philanthropist, and industrialist who resides in Gotham City."},
                new Employee { Id = 6, Name = "Murdock", City = "NY", Summary = "Daredevil (Matt Murdock) is a fictional superhero appearing in American comic books published by Marvel Comics. Daredevil was created by writer-editor Stan Lee and artist Bill Everett, with an unspecified amount of input from Jack Kirby."},
                new Employee { Id = 7, Name = "Rogers", City = "The world", Summary = "The character wears a costume bearing an American flag motif, and he utilizes a nearly-indestructible shield that he throws as a projectile. Captain America is the alter ego of Steve Rogers, a frail young artist enhanced to the peak of human perfection by an experimental \"super-soldier serum\" after joining the military to aid the United States government's efforts in World War II. Near the end of the war, he was trapped in ice and survived in suspended animation until he was revived in modern times.."},
                new Employee { Id = 8, Name = "Romanova", City = "NY", Summary = "Black Widow (Natalia Alianovna \"Natasha\" Romanova; is a fictional character appearing in American comic books published by Marvel Comics."},
                new Employee { Id = 9, Name = "Strange", City = "NY", Summary = "Bruce Wayne, a wealthy American playboy, philanthropist, and industrialist who resides in Gotham City."},
                new Employee { Id = 10, Name = "Wayne", City = "Gotham City", Summary = "The character begins as an extremely talented but egotistical surgeon who loses the ability to operate after a car crash severely damages his hands beyond repair. Searching the globe for healing, he encounters the Ancient One, the Sorcerer Supreme.."}
            };
        }

        private static void AssertEmployee(Employee employee)
        {
            Check.That(employee.Id).IsEqualTo(employee.Id);
            Check.That(employee.Name).IsEqualTo("Parker");
            Check.That(employee.City).IsEqualTo("NY");
            Check.That(employee.Summary)
                .IsEqualTo("Peter Parker is the secret identity of the character Spider-Man.");
        }

        private static async Task<Employee> AdaptEmployee(HttpResponseMessage httpResponseMessage)
        {
            return JsonConvert.DeserializeObject<Employee>(await httpResponseMessage.Content.ReadAsStringAsync());
        }
    }
}
