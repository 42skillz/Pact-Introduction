using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using ConsumerSuperHeroes;
using Newtonsoft.Json;
using NFluent;
using PactNet.Mocks.MockHttpService;
using PactNet.Mocks.MockHttpService.Models;
using Xunit;

namespace ConsumerSuperHeroPact
{
    public class ConsumerPactShould : IClassFixture<ConsumerSuperHeroesPactClassFixture>
    {
        private readonly IMockProviderService _mockProviderService;
        private readonly string _mockProviderServiceBaseUri;

        public ConsumerPactShould(ConsumerSuperHeroesPactClassFixture fixture)
        {
            _mockProviderService = fixture.MockProviderService;
            _mockProviderService.ClearInteractions();
            _mockProviderServiceBaseUri = fixture.MockProviderServiceBaseUri;
        }

        [Fact]
        public async Task Validate_one_super_hero_by_id()
        {
            const int employeeId = 1;

            _mockProviderService.Given("There are superheroes")
                .UponReceiving("GET one superHero")
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

                    Body = new SuperHeroFan()
                    {
                        Id = 1, Name = "Parker", FirstName = "Peter", Summary = "Peter Parker is the secret identity of the character Spider-Man."
                    }
                });

            var httpResponseMessage = new SuperHeroAdapter(_mockProviderServiceBaseUri)
                .GetSuperHeroById(employeeId).GetAwaiter().GetResult();
            
            if (httpResponseMessage.IsSuccessStatusCode)
            {
                AssertFirstSuperHero(await AdaptEmployee(httpResponseMessage));
            }
            
            _mockProviderService.VerifyInteractions();
        }

        

        private static void AssertFirstSuperHero(SuperHeroFan superHero)
        {
            Check.That(superHero.Id).IsEqualTo(superHero.Id);
            Check.That(superHero.FirstName).IsEqualTo("Peter");
            Check.That(superHero.Name).IsEqualTo("Parker");
            Check.That(superHero.Summary).IsEqualTo("Peter Parker is the secret identity of the character Spider-Man.");
        }

        private static async Task<SuperHeroFan> AdaptEmployee(HttpResponseMessage httpResponseMessage)
        {
            return JsonConvert.DeserializeObject<SuperHeroFan>(await httpResponseMessage.Content.ReadAsStringAsync());
        }
    }
}
