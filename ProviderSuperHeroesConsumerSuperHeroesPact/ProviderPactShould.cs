using System;
using System.Collections.Generic;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using PactNet;
using PactNet.Infrastructure.Outputters;
using ProviderSuperHeroesConsumerSuperHeroesPact.Middleware;
using ProviderSuperHeroesConsumerSuperHeroesPact.XUnitHelpers;
using Xunit;
using Xunit.Abstractions;

namespace ProviderSuperHeroesConsumerSuperHeroesPact
{
    public sealed class ProviderPactShould : IDisposable
    {
        private const string ProviderName = "ProviderSuperHeroes";
        private const string ConsumerName = "ConsumerSuperHeroes";
        private const string ProviderUriBase = "http://localhost:5000";
        private const string ProviderStateUriBase = "http://localhost:5002";
        private const string FileUri = @"..\..\..\..\pacts\consumersuperheroes-providersuperheroes.json";

        private readonly ITestOutputHelper _outputHelper;
        private IWebHost _webHost;


        public ProviderPactShould(ITestOutputHelper output)
        {
            _outputHelper = output;

            LaunchProviderStateHttpServer(ProviderStateUriBase);
        }

        [Fact]
        public void Ensure_honors_pact_contract_with_consumer()
        {
            PactVerify(ProviderUriBase, ProviderName, ConsumerName,
                ProviderStateUriBase, FileUri);
        }

        private void LaunchProviderStateHttpServer(string pactServiceUri)
        {
            _webHost = WebHost
                .CreateDefaultBuilder()
                .UseUrls(pactServiceUri)
                .UseStartup<TestStartup>()
                .Build();

            _webHost.Start();
        }

        private void PactVerify(string providerUri, string providerName, string consumerName,
            string uriBaseProviderState, string fileUri)
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
            pactVerifier.ProviderState($"{uriBaseProviderState}/provider-states")
                .ServiceProvider(providerName, providerUri)
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