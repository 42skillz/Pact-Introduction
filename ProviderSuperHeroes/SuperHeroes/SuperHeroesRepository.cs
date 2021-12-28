using System.Collections.Generic;
using System.Linq;

namespace ProviderSuperHeroes.SuperHeroes
{
    public class SuperHeroesRepository
    {
        private readonly List<SuperHero> _employees = new List<SuperHero>
        {
            new SuperHero
            {
                Id = 1, FirstName = "Peter", Name = "Parker", City = "NY",
                Summary = "Peter Parker is the secret identity of the character Spider-Man."
            },
            new SuperHero
            {
                Id = 2, FirstName = "Tony", Name = "Stark", City = "NY",
                Summary =
                    "A wealthy American business magnate, playboy, philanthropist, inventor and ingenious scientist."
            },
            new SuperHero
            {
                Id = 3, FirstName = "Kent", Name = "Clark", City = "NY",
                Summary = "He was found and adopted by farmers Jonathan and Martha Kent, who named him Clark Kent."
            },
            new SuperHero
            {
                Id = 4, FirstName = "Ben", Name = "Grimm", City = "NY",
                Summary =
                    "Benjamin Jacob Grimm, also known as The Thing, is a fictional superhero appearing in American comic books published by Marvel Comics."
            },
            new SuperHero
            {
                Id = 5, FirstName = "Bruce", Name = "Wayne", City = "Gotham City",
                Summary =
                    "Bruce Wayne, a wealthy American playboy, philanthropist, and industrialist who resides in Gotham City."
            },
            new SuperHero
            {
                Id = 6, FirstName = "Matt", Name = "Murdock", City = "NY",
                Summary =
                    "Daredevil (Matt Murdock) is a fictional superhero appearing in American comic books published by Marvel Comics. Daredevil was created by writer-editor Stan Lee and artist Bill Everett, with an unspecified amount of input from Jack Kirby."
            },
            new SuperHero
            {
                Id = 7, FirstName = "Steven", Name = "Rogers", City = "The world",
                Summary =
                    "The character wears a costume bearing an American flag motif, and he utilizes a nearly-indestructible shield that he throws as a projectile. Captain America is the alter ego of Steve Rogers, a frail young artist enhanced to the peak of human perfection by an experimental \"super-soldier serum\" after joining the military to aid the United States government's efforts in World War II. Near the end of the war, he was trapped in ice and survived in suspended animation until he was revived in modern times.."
            },
            new SuperHero
            {
                Id = 8, FirstName = "Natalia", Name = "Romanova", City = "NY",
                Summary =
                    "Black Widow (Natalia Alianovna \"Natasha\" Romanova; is a fictional character appearing in American comic books published by Marvel Comics."
            },
            new SuperHero
            {
                Id = 9, FirstName = "Stephen", Name = "Strange", City = "NY",
                Summary =
                    "Bruce Wayne, a wealthy American playboy, philanthropist, and industrialist who resides in Gotham City."
            },
            new SuperHero
            {
                Id = 10, FirstName = "Adrian", Name = "Toomes", City = "NY",
                Summary =
                    "Toomes is a brilliant but maniacal electronics engineer who designed a suit that allows him to fly at great speeds. After turning to a life of crime, he became a recurring enemy of the superhero Spider-Man and a founding member of the super-villain team known as the Sinister Six. Other characters have taken the mantle."
            }
        };

        public SuperHero GetById(int id)
        {
            return _employees.FirstOrDefault(e => e.Id == id);
        }

        public IEnumerable<SuperHero> GetAll()
        {
            return _employees;
        }
    }
}