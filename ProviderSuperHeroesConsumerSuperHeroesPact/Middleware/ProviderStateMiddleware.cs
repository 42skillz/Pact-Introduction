using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Server.Kestrel.Core.Internal.Http;
using Newtonsoft.Json;

namespace ProviderSuperHeroesConsumerSuperHeroesPact.Middleware
{
    public class ProviderStateMiddleware
    {
        private const string ConsumerName = "ConsumerSuperHeroes";
        private const string DataPath = @"..\..\..\..\data";
        private readonly RequestDelegate _next;
        private readonly IDictionary<string, Action> _providerStates;

        public ProviderStateMiddleware(RequestDelegate next)
        {
            _next = next;
            _providerStates = new Dictionary<string, Action>
            {
                ["There is no superhero"] = RemoveAllData,
                ["There are superheroes"] = AddData
            };
        }

        private void RemoveAllData()
        {
            var path = Path.Combine(Directory.GetCurrentDirectory(), DataPath);
            var deletePath = Path.Combine(path, "someData.txt");

            if (File.Exists(deletePath)) File.Delete(deletePath);
        }

        private void AddData()
        {
            var path = Path.Combine(Directory.GetCurrentDirectory(), DataPath);
            var writePath = Path.Combine(path, "someData.txt");

            if (!File.Exists(writePath)) File.Create(writePath);
        }

        public async Task Invoke(HttpContext context)
        {
            if (context.Request.Path.Value == "/provider-states")
            {
                await HandleProviderStatesRequestAsync(context);
                await context.Response.WriteAsync(string.Empty);
            }
            else
            {
                await _next(context);
            }
        }

        private async Task HandleProviderStatesRequestAsync(HttpContext context)
        {
            context.Response.StatusCode = (int)HttpStatusCode.OK;

            if (context.Request.Method.ToUpper() == HttpMethod.Post.ToString().ToUpper() &&
                context.Request.Body != null)
            {
                string jsonRequestBody;
                using (var reader = new StreamReader(context.Request.Body, Encoding.UTF8))
                {
                    jsonRequestBody = await reader.ReadToEndAsync();
                }

                var providerState = JsonConvert.DeserializeObject<ProviderState>(jsonRequestBody);

                //A null or empty provider state key must be handled
                if (providerState != null && !string.IsNullOrEmpty(providerState.State) &&
                    providerState.Consumer == ConsumerName)
                    _providerStates[providerState.State].Invoke();
            }
        }
    }
}