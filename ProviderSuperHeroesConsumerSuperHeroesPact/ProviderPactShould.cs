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
        private const string BrokerBaseUri = "https://42skillz.pactflow.io";
        private const string Token = "JjO7m8_Dm5DFCgUWsG8GAg";
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
                BrokerBaseUri, ProviderStateUriBase, Token);
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
            string brokerBaseUri, string providerStateUriBase, string token)
        {
            var config = new PactVerifierConfig
            {
                ProviderVersion = "3.0.1",
                PublishVerificationResults = true,
                Outputters = new List<IOutput>
                {
                    new XUnitOutput(_outputHelper)
                },
                //Verbose = true
            };

            var pactUriOptions = new PactUriOptions()
                .SetBearerAuthentication(token);
            var versionTags = new VersionTags
            {
                ConsumerTags = new List<string> { "test", "uat" },
                ProviderTags = new List<string> { "test", "uat" }
            };

            var consumerVersionSelectors = new List<VersionTagSelector>
            {
                new VersionTagSelector("test", latest: true),
                new VersionTagSelector("uat", latest: true),
                new VersionTagSelector("production")
            };

            const bool enablePending = false;
            
            var pactVerifier = new PactVerifier(config);
            pactVerifier
                .ProviderState($"{providerStateUriBase}/provider-states")
                .ServiceProvider(providerName, providerUriBase)
                .HonoursPactWith(consumerName)
                .PactBroker(brokerBaseUri, pactUriOptions, enablePending,
                    versionTags.ConsumerTags, versionTags.ProviderTags,
                    consumerVersionSelectors)
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