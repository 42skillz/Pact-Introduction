using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using ConsumerCharacters;
using Newtonsoft.Json;
using NFluent;
using PactNet.Matchers;
using PactNet.Mocks.MockHttpService;
using PactNet.Mocks.MockHttpService.Models;
using Xunit;

namespace ConsumerCharactersPact
{
    // Pact Terminology https://docs.pact.io/getting_started/terminology
    public class ConsumerPactShould : IClassFixture<ConsumerPactClassFixture>
    {
        private readonly IMockProviderService _mockProviderService;
        private readonly string _mockProviderServiceBaseUri;

        public ConsumerPactShould(ConsumerPactClassFixture fixture)
        {
            _mockProviderService = fixture.MockProviderService;
            _mockProviderService.ClearInteractions();
            _mockProviderServiceBaseUri = ConsumerPactClassFixture.MockProviderServiceBaseUri;
        }

        [Fact]
        public async Task Validate_one_character_by_id()
        {
            const int characterId = 1;

            _mockProviderService.Given("There are characters")
                .UponReceiving("One character")
                // When
                .With(new ProviderServiceRequest
                {
                    Method = HttpVerb.Get,
                    Path = $"/api/superheroes/{characterId}"
                })
                // Then
                .WillRespondWith(new ProviderServiceResponse
                {
                    Status = 200,

                    Headers = new Dictionary<string, object>
                    {
                        ["Content-Type"] = "application/json; charset=utf-8"
                    },

                    Body = Match.Type( new
                    {
                        Id = 1,
                        Name = Match.Type("string"),
                        City = Match.Type("string")
                    })
                });

            var consumerCharacters = new ConsumerCharacters.ConsumerCharacters(_mockProviderServiceBaseUri);

            var message = await consumerCharacters.GetCharacterById(characterId);

            if (message.IsSuccessStatusCode) AssertFirstCharacter(await AdaptCharacter(message));

            _mockProviderService.VerifyInteractions();
        }

        [Fact]
        public void Validate_all_characters()
        {
            _mockProviderService.Given("There are characters")
                .UponReceiving("All characters")
                .With(new ProviderServiceRequest
                {
                    Method = HttpVerb.Get,
                    Path = "/api/superheroes"
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
                    }, 
                        1
                    )
                });

            var result = new ConsumerCharacters.ConsumerCharacters(_mockProviderServiceBaseUri)
                .GetCharacters().GetAwaiter().GetResult();

            Check.That((int)result.StatusCode).IsEqualTo(200);

            // If the provider works => generate pact file
            _mockProviderService.VerifyInteractions();
        }

        private static void AssertFirstCharacter(Character character)
        {
            Check.That(character.Id).IsInstanceOfType(typeof(int));
            Check.That(character.Name).IsInstanceOfType(typeof(string));
            Check.That(character.City).IsInstanceOfType(typeof(string));
        }

        private static async Task<Character> AdaptCharacter(HttpResponseMessage httpResponseMessage)
        {
            return JsonConvert.DeserializeObject<Character>(await httpResponseMessage.Content.ReadAsStringAsync());
        }
    }
}