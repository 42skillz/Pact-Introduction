using System;
using System.Net.Http;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace ConsumerSuperHeroes
{
    internal static class Program
    {
        private static async Task Main(string[] args)
        {
            var superHeroId = 1;
            var baseUri = "http://localhost:5000";

            if (args.Length > 1)
            {
                superHeroId = int.Parse(args[0]);
                baseUri = args[1];
            }

            var consumerSuperHero = new ConsumerSuperHeroes(baseUri);

            // Retrieve one superHeroFan
            var superHeroFan = await AdaptSuperHero(await consumerSuperHero.GetSuperHeroById(superHeroId));

            Console.WriteLine(
                $"Retrieve superHeroFan: ID: {superHeroFan.Id}, FirstName: {superHeroFan.FirstName}, Name: {superHeroFan.Name}, Summary: {superHeroFan.Summary}");
        }

        private static async Task<FanOfSuperHero> AdaptSuperHero(HttpResponseMessage response)
        {
            return JsonConvert.DeserializeObject<FanOfSuperHero>(await response.Content.ReadAsStringAsync());
        }
    }
}