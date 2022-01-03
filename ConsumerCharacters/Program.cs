using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace ConsumerCharacters
{
    internal static class Program
    {
        private static async Task Main(string[] args)
        {
            var characterId = 1;
            var baseUri = "http://localhost:5000";

            if (args.Length > 1)
            {
                characterId = int.Parse(args[0]);
                baseUri = args[1];
            }

            var consumerCharacter = new ConsumerCharacters(baseUri);

            // Retrieve one character
            var character = await AdaptCharacter(await consumerCharacter.GetCharacterById(characterId));

            Console.WriteLine($"Retrieve character: ID: {character.Id} Name: {character.Name} City: {character.City}.");

            // Retrieve all characters
            var characters = await AdaptCharacters(await consumerCharacter.GetCharacters());

            Console.WriteLine($"Retrieve all characters: {string.Join(", ", characters.Select(e => e.Name))}.");
        }

        private static async Task<Character> AdaptCharacter(HttpResponseMessage response)
        {
            return JsonConvert.DeserializeObject<Character>(await response.Content.ReadAsStringAsync());
        }

        private static async Task<List<Character>> AdaptCharacters(HttpResponseMessage response)
        {
            return JsonConvert.DeserializeObject<List<Character>>(await response.Content.ReadAsStringAsync());
        }
    }
}