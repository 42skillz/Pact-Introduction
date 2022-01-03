using System;
using System.Collections.Generic;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using PactNet;
using PactNet.Infrastructure.Outputters;
using ProviderSuperHeroesConsumerCharactersPact.Middleware;
using ProviderSuperHeroesConsumerCharactersPact.XUnitHelpers;
using Xunit;
using Xunit.Abstractions;

namespace ProviderSuperHeroesConsumerCharactersPact
{
    public sealed class ProviderPactShould : IDisposable
    {
        private readonly ITestOutputHelper _outputHelper;
        private const string ProviderUriBase = "http://localhost:5000";
        private const string ProviderStateUriBase = "http://localhost:5002";
        private IWebHost _webHost;


        public ProviderPactShould(ITestOutputHelper output)
        {
            _outputHelper = output;

            LaunchProviderStateHttpServer(ProviderStateUriBase);
        }

        [Fact]
        public void Ensure_honors_pact_contract_with_consumer()
        {
            PactVerify(ProviderUriBase, "ProviderSuperHeroes", "ConsumerCharacters",
                "https://42skillz.pactflow.io/pacts/provider/ProviderSuperHeroes/consumer/ConsumerCharacters/latest", 
                ProviderStateUriBase, "JjO7m8_Dm5DFCgUWsG8GAg");
        }

        private void LaunchProviderStateHttpServer(string pactServiceUri)
        {
            _webHost = WebHost.CreateDefaultBuilder()
                .UseUrls(pactServiceUri)
                .UseStartup<TestStartup>()
                .Build();

            _webHost.Start();
        }

        private void PactVerify(string providerUriBase, string providerName, string consumerName,
            string fileUri, string providerStateUriBase, string token)
        {
            var config = new PactVerifierConfig
            {
                ProviderVersion = "1.0.0",
                PublishVerificationResults = true,
                Outputters = new List<IOutput>
                {
                    new XUnitOutput(_outputHelper)
                },
                Verbose = true
            };

            var pactUriOptions = new PactUriOptions()
                .SetBearerAuthentication(token);

            var pactVerifier = new PactVerifier(config);

            pactVerifier
                .ProviderState($"{providerStateUriBase}/provider-states")
                .ServiceProvider(providerName, providerUriBase)
                .HonoursPactWith(consumerName)
                .PactUri(fileUri, pactUriOptions)
                .Verify();
        }

        #region IDisposable Support

        private bool _disposedValue; // To detect redundant calls

        private void Dispose(bool disposing)
        {
            if (!_disposedValue)
            {
                if (disposing)
                {
                    _webHost.StopAsync().GetAwaiter().GetResult();
                    _webHost.Dispose();
                }

                _disposedValue = true;
            }
        }

        public void Dispose()
        {
            Dispose(true);
        }

        #endregion
    }
}