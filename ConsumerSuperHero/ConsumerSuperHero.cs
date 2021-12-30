using System;
using System.Net.Http;
using System.Threading.Tasks;

namespace ConsumerSuperHero
{
    public class ConsumerSuperHero
    {
        private readonly string _uriSuperHeroService;

        public ConsumerSuperHero(string uriSuperHeroService)
        {
            _uriSuperHeroService = uriSuperHeroService;
        }

        public async Task<HttpResponseMessage> GetSuperHeroById(int id)
        {
            using var client = new HttpClient();

            client.BaseAddress = new Uri(_uriSuperHeroService);
            try
            {
                return await client.GetAsync($"api/superheroes/{id}");
            }
            catch (Exception e)
            {
                throw new Exception("There was a problem connecting to Provider API.", e);
            }
        }
    }
}