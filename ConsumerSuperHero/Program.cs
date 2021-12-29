using System;
using System.Net.Http;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace ConsumerSuperHero
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

            var superHeroAdapter = new SuperHeroAdapter(baseUri);

            // Retrieve one superHeroFan
            var response = await superHeroAdapter.GetSuperHeroById(superHeroId);
            var superHeroFan = await AdaptSuperHero(response);
            Console.WriteLine(
                $"Retrieve superHeroFan: ID: {superHeroFan.Id} FirstName: {superHeroFan.FirstName} Name: {superHeroFan.Name} Summary: {superHeroFan.Summary}.");
        }

        private static async Task<FanOfSuperHero> AdaptSuperHero(HttpResponseMessage response)
        {
            return JsonConvert.DeserializeObject<FanOfSuperHero>(await response.Content.ReadAsStringAsync());
        }
    }
}