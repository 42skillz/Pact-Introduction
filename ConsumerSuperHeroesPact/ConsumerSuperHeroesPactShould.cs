using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using ConsumerSuperHeroes;
using Newtonsoft.Json;
using NFluent;
using PactNet.Mocks.MockHttpService;
using PactNet.Mocks.MockHttpService.Models;
using Xunit;

namespace ConsumerSuperHeroesPact
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
                .UponReceiving("One superHero")
                // When
                .With(new ProviderServiceRequest
                {
                    Method = HttpVerb.Get,
                    Path = $"/api/superheroes/{employeeId}"
                })
                // Then
                .WillRespondWith(new ProviderServiceResponse
                {
                    Status = 200,

                    Headers = new Dictionary<string, object>
                    {
                        ["Content-Type"] = "application/json; charset=utf-8"
                    },

                    Body = new FanOfSuperHero(1, "Peter", "Parker",
                        "Peter Parker is the secret identity of the character Spider-Man.")
                });

            var httpResponseMessage = new ConsumerSuperHeroes.ConsumerSuperHeroes(_mockProviderServiceBaseUri)
                .GetSuperHeroById(employeeId).GetAwaiter().GetResult();

            if (httpResponseMessage.IsSuccessStatusCode) AssertFirstSuperHero(await AdaptSuperHero(httpResponseMessage));

            _mockProviderService.VerifyInteractions();
        }


        private static void AssertFirstSuperHero(FanOfSuperHero superHero)
        {
            Check.That(superHero.Id).IsEqualTo(superHero.Id);
            Check.That(superHero.FirstName).IsEqualTo("Peter");
            Check.That(superHero.Name).IsEqualTo("Parker");
            Check.That(superHero.Summary).IsEqualTo("Peter Parker is the secret identity of the character Spider-Man.");
        }

        private static async Task<FanOfSuperHero> AdaptSuperHero(HttpResponseMessage httpResponseMessage)
        {
            return JsonConvert.DeserializeObject<FanOfSuperHero>(await httpResponseMessage.Content.ReadAsStringAsync());
        }
    }
}