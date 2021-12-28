using System;
using System.Net.Http;
using System.Threading.Tasks;

namespace ConsumerCharacter
{
    public class CharacterAdapter
    {
        private readonly string _uriEmployeeService;

        public CharacterAdapter(string uriEmployeeService)
        {
            _uriEmployeeService = uriEmployeeService;
        }

        public async Task<HttpResponseMessage> GetCharacterById(int id)
        {
            using var client = new HttpClient();

            client.BaseAddress = new Uri(_uriEmployeeService);
            try
            {
                return await client.GetAsync($"api/superheroes/{id}");
            }
            catch (Exception e)
            {
                throw new Exception("There was a problem connecting to Provider API.", e);
            }
        }

        public async Task<HttpResponseMessage> GetCharacters()
        {
            using var client = new HttpClient();

            client.BaseAddress = new Uri(_uriEmployeeService);

            try
            {
                return await client.GetAsync("api/superheroes");
            }
            catch (Exception e)
            {
                throw new Exception("There was a problem connecting to Provider API.", e);
            }
        }
    }
}