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
        private const string ProviderVersion = "2.0.1";
        private const string ProviderName = "ProviderSuperHeroes";
        private const string ConsumerName = "ConsumerCharacters";
        private const string ProviderUriBase = "http://localhost:5000";
        private const string ProviderStateUriBase = "http://localhost:5002";
        private const string BrokerBaseUri = "https://42skillz.pactflow.io";
        private const string BearToken = "JjO7m8_Dm5DFCgUWsG8GAg";
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
            // Provider verification badges <a href="https://stackoverflow.com/questions/6960426/c-sharp-xml-documentation-website-link">here</a>
            var versionTags = new VersionTags
            {
                ConsumerTags = new List<string> { "test", "uat" },
                ProviderTags = new List<string> { "test", "uat" }
            };

            // Consumer Version Selectors <a href="https://docs.pact.io/pact_broker/advanced_topics/consumer_version_selectors">here</a>
            var consumerVersionSelectors = new List<VersionTagSelector>
            {
                new VersionTagSelector("test", latest: true),
                new VersionTagSelector("uat", latest: true),
                new VersionTagSelector("production")
            };

            // Pending pacts <a href="https://docs.pact.io/pact_broker/advanced_topics/pending_pacts">here</a>
            const bool enablePending = false;

            // Work In Progress pacts <a href="https://docs.pact.io/pact_broker/advanced_topics/wip_pacts">here</a>
            const string includeWipPactsSince = "2022-01-01";
            
            PactVerify(ProviderUriBase, ProviderName, ConsumerName,
                BrokerBaseUri, ProviderStateUriBase, BearToken,
                versionTags, consumerVersionSelectors, enablePending, includeWipPactsSince);
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
            string brokerBaseUri, string providerStateUriBase, string bearToken, VersionTags versionTags,
            IEnumerable<VersionTagSelector> consumerVersionSelectors, bool enablePending, string includeWipPactsSince)
        {
            var config = new PactVerifierConfig
            {
                ProviderVersion = ProviderVersion,
                PublishVerificationResults = true,
                Outputters = new List<IOutput>
                {
                    new XUnitOutput(_outputHelper)
                }
                //Verbose = true
            };

            var pactUriOptions = new PactUriOptions()
                .SetBearerAuthentication(bearToken);

            var pactVerifier = new PactVerifier(config);

            pactVerifier
                .ProviderState($"{providerStateUriBase}/provider-states")
                .ServiceProvider(providerName, providerUriBase)
                .HonoursPactWith(consumerName)
                .PactBroker(brokerBaseUri, pactUriOptions, enablePending,
                    versionTags.ConsumerTags.ToArray(),
                    versionTags.ProviderTags.ToArray(),
                    consumerVersionSelectors, includeWipPactsSince)
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