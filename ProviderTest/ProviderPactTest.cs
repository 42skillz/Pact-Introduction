using System;
using System.Collections.Generic;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using PactNet;
using ProviderPact.Middleware;
using ProviderPact.XUnitHelpers;
using Xunit;
using Xunit.Abstractions;

namespace ProviderPact
{
    public sealed class ProviderPactTest : IDisposable
    {
        private readonly string _providerUri;
        private readonly string _pactServiceUri;
        private readonly IWebHost _webHost;
        private readonly ITestOutputHelper _outputHelper;

        public ProviderPactTest(ITestOutputHelper output)
        {
            _outputHelper = output;
            _providerUri = "http://localhost:5000";
            _pactServiceUri = "http://localhost:5002";

            _webHost = WebHost.CreateDefaultBuilder()
                .UseUrls(_pactServiceUri)
                .UseStartup<TestStartup>()
                .Build();

            _webHost.Start();
        }

        [Fact]
        public void EnsureProviderApiHonorsPactWithConsumer()
        {
            // Arrange
            var config = new PactVerifierConfig
            {
                Outputters = new List<PactNet.Infrastructure.Outputters.IOutput>
                {
                    new XUnitOutput(_outputHelper)
                },
                Verbose = true
            };

            //Act / Assert
            IPactVerifier pactVerifier = new PactVerifier(config);
            pactVerifier.ProviderState($"{_pactServiceUri}/provider-states")
                .ServiceProvider("Provider", _providerUri)
                .HonoursPactWith("Consumer")
                .PactUri(@"..\..\..\..\pacts\consumer-provider.json")
                .Verify("A valid GET employee", "There is data");
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
