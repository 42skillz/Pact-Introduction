using System;
using System.Collections.Generic;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using PactNet;
using PactNet.Infrastructure.Outputters;
using ProviderSuperHeroesConsumerCharacterPact.Middleware;
using ProviderSuperHeroesConsumerCharacterPact.XUnitHelpers;
using Xunit;
using Xunit.Abstractions;

namespace ProviderSuperHeroesConsumerCharacterPact
{
    public sealed class ProviderPactShould : IDisposable
    {
        private readonly ITestOutputHelper _outputHelper;
        private readonly string _pactServiceUri;
        private readonly string _providerUri;
        private IWebHost _webHost;

        public ProviderPactShould(ITestOutputHelper output)
        {
            _outputHelper = output;
            _providerUri = "http://localhost:5000";
            _pactServiceUri = "http://localhost:5002";

            LaunchProviderStateHttpServer();
        }

        [Fact]
        public void Ensure_honors_pact_contract_with_consumer()
        {
            PactVerify("ProviderSuperHeroes", "ConsumerCharacter", 
                @"..\..\..\..\pacts\consumercharacter-providersuperheroes.json");
        }

        private void LaunchProviderStateHttpServer()
        {
            _webHost = WebHost.CreateDefaultBuilder()
                .UseUrls(_pactServiceUri)
                .UseStartup<TestStartup>()
                .Build();

            _webHost.Start();
        }

        private void PactVerify(string providerName, string consumerName, string fileUri)
        {
            var config = new PactVerifierConfig
            {
                Outputters = new List<IOutput>
                {
                    new XUnitOutput(_outputHelper)
                },
                Verbose = true
            };

            var pactVerifier = new PactVerifier(config);

            pactVerifier
                .ProviderState($"{_pactServiceUri}/provider-states")
                .ServiceProvider(providerName, _providerUri)
                .HonoursPactWith(consumerName)
                .PactUri(fileUri)
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