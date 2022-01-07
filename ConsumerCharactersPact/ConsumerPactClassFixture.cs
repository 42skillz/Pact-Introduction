using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using PactNet;
using PactNet.Mocks.MockHttpService;

namespace ConsumerCharactersPact
{
    // Pact Terminology https://docs.pact.io/getting_started/terminology
    public class ConsumerPactClassFixture : IDisposable
    {
        private const string ConsumerName = "ConsumerCharacters";
        private const string ProviderName = "ProviderSuperHeroes";
        private const string CodeDirBase = @"..\..\..\..\";
        private const string PactSpecificationVersion = "2.0.0";
        private const string PactDir = @"pacts";
        private const string PactDirLogs = @"pact_logs";
        private const string Token = "JjO7m8_Dm5DFCgUWsG8GAg";
        private const string SslCaFile = @"ca.crt";
        private const string BrokerBaseUri = "https://42skillz.pactflow.io";
        private const string ConsumerVersion = "2.0.5";
        private const string ConsumerVersionBranch = "master";
        private static readonly string[] Environments = { "uat" };

        private string[] Tags { get; } = { ConsumerVersionBranch, Environments[0] };
        private IPactBuilder PactBuilder { get; }
        public IMockProviderService MockProviderService { get; }
        private static int MockServerPort => 9222;
        public static string MockProviderServiceBaseUri => $"http://localhost:{MockServerPort}";

        public ConsumerPactClassFixture()
        {
            // Using Spec version 2.0.0 more details at https://goo.gl/UrBSRc
            var pactConfig = new PactConfig
            {
                SpecificationVersion = PactSpecificationVersion,
                PactDir = AppendCodeDirBase(PactDir),
                LogDir = AppendCodeDirBase(PactDirLogs)
            };

            PactBuilder = new PactBuilder(pactConfig);

            PactBuilder.ServiceConsumer(ConsumerName)
                .HasPactWith(ProviderName);

            MockProviderService = PactBuilder.MockService(MockServerPort, new JsonSerializerSettings
            {
                ContractResolver = new CamelCasePropertyNamesContractResolver()
            });
        }

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

                    PublishToBroker(ConsumerName, ProviderName, 
                        Token, SslCaFile, BrokerBaseUri, ConsumerVersion, Tags);
                }

                _disposedValue = true;
            }
        }

        private static void PublishToBroker(string consumerName, string providerName, 
            string token, string sslCaFile, 
            string brokerBaseUri, string consumerVersion, IEnumerable<string> tags)
        {
            var brokerUriOptions =
                new PactUriOptions(token).SetSslCaFilePath(AppendCodeDirBase(sslCaFile));
            var pactPublisher = new PactPublisher(brokerBaseUri, brokerUriOptions);
            
            var pactFileUri = $"{AppendCodeDirBase(PactDir)}/{consumerName.ToLower()}-{providerName.ToLower()}.json";   
            
            pactPublisher.PublishToBroker(pactFileUri,
                consumerVersion,
                tags);
        }

        private static string AppendCodeDirBase(string directory)
        {
            return $"{CodeDirBase}/{directory}";
        }

        // This code added to correctly implement the disposable pattern.
        public void Dispose()
        {
            // Do not change this code. Put cleanup code in Dispose(bool disposing) above.
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        #endregion
    }
}