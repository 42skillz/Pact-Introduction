using System;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using PactNet;
using PactNet.Mocks.MockHttpService;

namespace ConsumerCharactersPact
{
    public class ConsumerPactClassFixture : IDisposable
    {
        private const string ConsumerName = "ConsumerCharacters";
        private const string ProviderName = "ProviderSuperHeroes";
        private const string SpecificationVersion = "2.0.0";
        private const string PactDir = @"..\..\..\..\pacts\";
        private const string PactLogs = @"..\..\..\..\pact_logs";

        public ConsumerPactClassFixture()
        {
            // Using Spec version 2.0.0 more details at https://goo.gl/UrBSRc
            var pactConfig = new PactConfig
            {
                SpecificationVersion = SpecificationVersion,
                PactDir = PactDir,
                LogDir = PactLogs
            };

            PactBuilder = new PactBuilder(pactConfig);

            PactBuilder.ServiceConsumer(ConsumerName)
                .HasPactWith(ProviderName);

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

                    PublishToBroker();
                }

                _disposedValue = true;
            }
        }

        private static void PublishToBroker()
        {
            var brokerUriOptions =
                new PactUriOptions("JjO7m8_Dm5DFCgUWsG8GAg").SetSslCaFilePath("c:\\dev\\ca.crt");
            var pactPublisher = new PactPublisher("https://42skillz.pactflow.io", brokerUriOptions);
            pactPublisher.PublishToBroker(@"..\..\..\..\pacts\consumercharacters-providersuperheroes.json",
                "1.0.1",
                new[] { "master" });
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