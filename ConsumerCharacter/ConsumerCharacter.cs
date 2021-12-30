using System;
using System.Net.Http;
using System.Threading.Tasks;

namespace ConsumerCharacter
{
    public class ConsumerCharacter
    {
        private readonly string _uriEmployeeService;

        public ConsumerCharacter(string uriEmployeeService)
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