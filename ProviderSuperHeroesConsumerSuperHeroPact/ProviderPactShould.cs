using System;
using System.Collections.Generic;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using PactNet;
using PactNet.Infrastructure.Outputters;
using ProviderSuperHeroesConsumerSuperHeroPact.Middleware;
using ProviderSuperHeroesConsumerSuperHeroPact.XUnitHelpers;
using Xunit;
using Xunit.Abstractions;

namespace ProviderSuperHeroesConsumerSuperHeroPact
{
    public sealed class ProviderPactShould : IDisposable
    {
        private readonly ITestOutputHelper _outputHelper;
        private readonly string _providerUri;
        private IWebHost _webHost;

        public ProviderPactShould(ITestOutputHelper output)
        {
            _outputHelper = output;
            _providerUri = "http://localhost:5000";

            LaunchProviderStateHttpServer("http://localhost:5002");
        }

        [Fact]
        public void Ensure_honors_pact_contract_with_consumer()
        {
            PactVerify("ProviderSuperHeroes", "ConsumerSuperHeroes", 
                "JjO7m8_Dm5DFCgUWsG8GAg", "https://42skillz.pactflow.io/pacts/provider/ProviderSuperHeroes/consumer/ConsumerSuperHeroes/latest");
        }

        private void LaunchProviderStateHttpServer(string pactServiceUri)
        {
            _webHost = WebHost.CreateDefaultBuilder()
                .UseUrls(pactServiceUri)
                .UseStartup<TestStartup>()
                .Build();

            _webHost.Start();
        }

        private void PactVerify(string providerName, string consumerName, string token, string fileUri)
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
                .ProviderState($"{"http://localhost:5002"}/provider-states")
                .ServiceProvider(providerName, _providerUri)
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