using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using ConsumerSuperHeroes;
using Newtonsoft.Json;
using NFluent;
using PactNet;
using PactNet.Matchers;
using PactNet.Mocks.MockHttpService;
using PactNet.Mocks.MockHttpService.Models;
using Xunit;

namespace ConsumerSuperHeroesPact
{
    // Pact Terminology https://docs.pact.io/getting_started/terminology
    public class ConsumerPactShould : IClassFixture<ConsumerSuperHeroesPactClassFixture>
    {
        private readonly IMockProviderService _mockProviderService;
        private readonly string _mockProviderServiceBaseUri;

        public ConsumerPactShould(ConsumerSuperHeroesPactClassFixture fixture)
        {
            _mockProviderService = fixture.MockProviderService;
            _mockProviderService.ClearInteractions();
            _mockProviderServiceBaseUri = ConsumerSuperHeroesPactClassFixture.MockProviderServiceBaseUri;
        }

        [Fact]
        public async Task Validate_one_super_hero_by_id()
        {
            const int superHeroId = 1;

            _mockProviderService.Given("There are superheroes")
                .UponReceiving("One fanOfSuperHero")
                // When
                .With(new ProviderServiceRequest
                {
                    Method = HttpVerb.Get,
                    Path = $"/api/superheroes/{superHeroId}"
                })
                // Then
                .WillRespondWith(new ProviderServiceResponse
                {
                    Status = 200,

                    Headers = new Dictionary<string, object>
                    {
                        ["Content-Type"] = "application/json; charset=utf-8"
                    },

                    Body = Match.Type(new {
                        Id = 1,
                        FirstName = "Peter",
                        Name = "Parker",
                        Summary = "Peter Parker is the secret identity of the character Spider-Man."
                    })
                });

            var message = await new ConsumerSuperHeroes.ConsumerSuperHeroes(_mockProviderServiceBaseUri)
                .GetSuperHeroById(superHeroId);

            if (message.IsSuccessStatusCode)
                AssertFirstSuperHero(await AdaptSuperHero(message));

            _mockProviderService.VerifyInteractions();
        }


        private static void AssertFirstSuperHero(FanOfSuperHero fanOfSuperHero)
        {
            Check.That(fanOfSuperHero.Id).IsInstanceOfType(typeof(int));
            Check.That(fanOfSuperHero.FirstName).IsInstanceOfType(typeof(string));
            Check.That(fanOfSuperHero.Name).IsInstanceOfType(typeof(string));
            Check.That(fanOfSuperHero.Summary).IsInstanceOfType(typeof(string)); ;
        }

        private static async Task<FanOfSuperHero> AdaptSuperHero(HttpResponseMessage httpResponseMessage)
        {
            return JsonConvert.DeserializeObject<FanOfSuperHero>(await httpResponseMessage.Content.ReadAsStringAsync());
        }
    }
}