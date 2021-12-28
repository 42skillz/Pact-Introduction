using System;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using PactNet;
using PactNet.Mocks.MockHttpService;

namespace ConsumerCharacterPact
{
    public class ConsumerPactClassFixture : IDisposable
    {
        public ConsumerPactClassFixture()
        {
            // Using Spec version 2.0.0 more details at https://goo.gl/UrBSRc
            var pactConfig = new PactConfig
            {
                SpecificationVersion = "2.0.0",
                PactDir = @"..\..\..\..\pacts\",
                LogDir = @"..\..\..\..\pact_logs"
            };

            PactBuilder = new PactBuilder(pactConfig);

            PactBuilder.ServiceConsumer("ConsumerCharacter")
                .HasPactWith("ProviderSuperHeroes");

            MockProviderService = PactBuilder.MockService(MockServerPort, new JsonSerializerSettings
            {
                ContractResolver = new CamelCasePropertyNamesContractResolver()
            });
        }

        private IPactBuilder PactBuilder { get; }
        public IMockProviderService MockProviderService { get; }

        private int MockServerPort => 9222;
        public string MockProviderServiceBaseUri => $"http://localhost:{MockServerPort}";

        #region IDisposable Support

        private bool _disposedValue; // To detect redundant calls

        protected virtual void Dispose(bool disposing)
        {
            if (!_disposedValue)
            {
                if (disposing)
                {
                    // This will save the pact file once finished.
                    PactBuilder.Build();

                    var brokerUriOptions =
                        new PactUriOptions("JjO7m8_Dm5DFCgUWsG8GAg").SetSslCaFilePath("c:\\dev\\ca.crt");
                    var pactPublisher = new PactPublisher("https://42skillz.pactflow.io", brokerUriOptions);
                    pactPublisher.PublishToBroker(@"..\..\..\..\pacts\consumercharacter-providersuperheroes.json",
                        "1.0.1",
                        new[] { "master" });
                }

                _disposedValue = true;
            }
        }

        // This code added to correctly implement the disposable pattern.
        public void Dispose()
        {
            // Do not change this code. Put cleanup code in Dispose(bool disposing) above.
            Dispose(true);
        }

        #endregion
    }
}