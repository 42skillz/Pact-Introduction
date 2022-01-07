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
        private const string Token = "JjO7m8_Dm5DFCgUWsG8GAg";
        private const string PathToSslCaFile = @"..\..\..\..\ca.crt";
        private const string BrokerBaseUri = "https://42skillz.pactflow.io";
        private const string PactFile = "consumercharacters-providersuperheroes.json";
        private const string ConsumerVersion = "2.0.3";
        private const string ConsumerVersionTag = "master";

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

                    PublishToBroker(Token, PathToSslCaFile, BrokerBaseUri, PactFile, ConsumerVersion,
                        ConsumerVersionTag);
                }

                _disposedValue = true;
            }
        }

        private static void PublishToBroker(string token, string pathToSslCaFile, string brokerBaseUri, string pactFile,
            string consumerVersion, string consumerVersionTag)
        {
            var brokerUriOptions =
                new PactUriOptions(token).SetSslCaFilePath(pathToSslCaFile);
            var pactPublisher = new PactPublisher(brokerBaseUri, brokerUriOptions);
            pactPublisher.PublishToBroker($"{PactDir}{pactFile}",
                consumerVersion,
                new[] { consumerVersionTag, "uat" });
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