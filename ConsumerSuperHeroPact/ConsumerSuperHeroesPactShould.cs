using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using ConsumerSuperHero;
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
                .UponReceiving("One fanOfSuperHero")
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

                    Body = new FanOfSuperHero(1, "Peter", "Parker", "Peter Parker is the secret identity of the character Spider-Man.")
                });

            var httpResponseMessage = new SuperHeroAdapter(_mockProviderServiceBaseUri)
                .GetSuperHeroById(employeeId).GetAwaiter().GetResult();

            if (httpResponseMessage.IsSuccessStatusCode) AssertFirstSuperHero(await AdaptSuperHero(httpResponseMessage));

            _mockProviderService.VerifyInteractions();
        }


        private static void AssertFirstSuperHero(FanOfSuperHero fanOfSuperHero)
        {
            Check.That(fanOfSuperHero.Id).IsEqualTo(fanOfSuperHero.Id);
            Check.That(fanOfSuperHero.FirstName).IsEqualTo("Peter");
            Check.That(fanOfSuperHero.Name).IsEqualTo("Parker");
            Check.That(fanOfSuperHero.Summary).IsEqualTo("Peter Parker is the secret identity of the character Spider-Man.");
        }

        private static async Task<FanOfSuperHero> AdaptSuperHero(HttpResponseMessage httpResponseMessage)
        {
            return JsonConvert.DeserializeObject<FanOfSuperHero>(await httpResponseMessage.Content.ReadAsStringAsync());
        }
    }
}