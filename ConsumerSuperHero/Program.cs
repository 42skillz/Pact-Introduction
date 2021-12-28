using System;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace ConsumerSuperHeroes
{
    internal static class Program
    {
        static async Task Main(string[] args)
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
            SuperHeroFan superHeroFan = JsonConvert.DeserializeObject<SuperHeroFan>(await response.Content.ReadAsStringAsync());
            Console.WriteLine($"Retrieve superHeroFan: ID: {superHeroFan.Id} FirstName: {superHeroFan.FirstName} Name: {superHeroFan.Name} Summary: {superHeroFan.Summary}.");
        }
    }
}
