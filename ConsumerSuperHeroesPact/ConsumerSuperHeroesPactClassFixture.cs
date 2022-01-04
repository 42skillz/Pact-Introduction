using System;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using PactNet;
using PactNet.Mocks.MockHttpService;

namespace ConsumerSuperHeroesPact
{
    public class ConsumerSuperHeroesPactClassFixture : IDisposable
    {
        private const string ConsumerName = "ConsumerSuperHeroes";
        private const string ProviderName = "ProviderSuperHeroes";
        private const string SpecificationVersion = "2.0.0";
        private const string PactDir = @"..\..\..\..\pacts\";
        private const string PactLogs = @"..\..\..\..\pact_logs";
        private const string Token = "JjO7m8_Dm5DFCgUWsG8GAg";
        private const string PathToSslCaFile = @"..\..\..\..\ca.crt";
        private const string BrokerBaseUri = "https://42skillz.pactflow.io";
        private const string PactFile = "consumersuperheroes-providersuperheroes.json";
        private const string ConsumerVersion = "1.0.2";
        private const string ConsumerVersionTag = "master";

        public ConsumerSuperHeroesPactClassFixture()
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

                    PublishToBroker(Token, PathToSslCaFile, BrokerBaseUri, PactFile, ConsumerVersion, ConsumerVersionTag);
                }

                _disposedValue = true;
            }
        }

        private static void PublishToBroker(string token, string pathToSslCaFile, string brokenBaseUri, string pactFile, string consumerVersion, string tag)
        {
            var brokerUriOptions =
                new PactUriOptions(token).SetSslCaFilePath(pathToSslCaFile);
            var pactPublisher = new PactPublisher(brokenBaseUri, brokerUriOptions);
            pactPublisher.PublishToBroker($"{PactDir}{pactFile}",
                consumerVersion,
                new[] { tag, "uat" });
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