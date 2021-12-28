using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using ProviderSuperHeroes.SuperHeroes;

namespace ProviderSuperHeroes.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class SuperHeroesController : ControllerBase
    {
        private readonly SuperHeroesRepository _superHeroesRepository = new SuperHeroesRepository();

        [HttpGet]
        public IEnumerable<SuperHero> Get()
        {
            return _superHeroesRepository.GetAll();
        }

        [HttpGet("{id:int}")]
        public SuperHero Get(int id)
        {
            return _superHeroesRepository.GetById(id);
        }
    }
}
