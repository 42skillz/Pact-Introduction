using System.Collections.Generic;
using System.Linq;

namespace Provider.Employees
{
    public class EmployeeRepository
    {
        private readonly List<Employee> _employees = new List<Employee>
        {
            new Employee { Id = 1, Name = "Parker", City = "NY", Summary = "Peter Parker is the secret identity of the character Spider-Man."},
            new Employee { Id = 2, Name = "Stark", City = "NY", Summary = "A wealthy American business magnate, playboy, philanthropist, inventor and ingenious scientist."},
            new Employee { Id = 3, Name = "Clark", City = "NY", Summary = "He was found and adopted by farmers Jonathan and Martha Kent, who named him Clark Kent."},
            new Employee { Id = 4, Name = "Grimm", City = "NY", Summary = "Benjamin Jacob Grimm, also known as The Thing, is a fictional superhero appearing in American comic books published by Marvel Comics."},
            new Employee { Id = 5, Name = "Wayne", City = "Gotham City", Summary = "Bruce Wayne, a wealthy American playboy, philanthropist, and industrialist who resides in Gotham City."},
            new Employee { Id = 6, Name = "Murdock", City = "NY", Summary = "Daredevil (Matt Murdock) is a fictional superhero appearing in American comic books published by Marvel Comics. Daredevil was created by writer-editor Stan Lee and artist Bill Everett, with an unspecified amount of input from Jack Kirby."},
            new Employee { Id = 7, Name = "Rogers", City = "The world", Summary = "The character wears a costume bearing an American flag motif, and he utilizes a nearly-indestructible shield that he throws as a projectile. Captain America is the alter ego of Steve Rogers, a frail young artist enhanced to the peak of human perfection by an experimental \"super-soldier serum\" after joining the military to aid the United States government's efforts in World War II. Near the end of the war, he was trapped in ice and survived in suspended animation until he was revived in modern times.."},
            new Employee { Id = 8, Name = "Romanova", City = "NY", Summary = "Black Widow (Natalia Alianovna \"Natasha\" Romanova; is a fictional character appearing in American comic books published by Marvel Comics."},
            new Employee { Id = 9, Name = "Strange", City = "NY", Summary = "Bruce Wayne, a wealthy American playboy, philanthropist, and industrialist who resides in Gotham City."},
            new Employee { Id = 10, Name = "Wayne", City = "Gotham City", Summary = "The character begins as an extremely talented but egotistical surgeon who loses the ability to operate after a car crash severely damages his hands beyond repair. Searching the globe for healing, he encounters the Ancient One, the Sorcerer Supreme.."}
        };

        public Employee GetById(int id)
        {
            return _employees.FirstOrDefault(e => e.Id == id);
        }

        public IEnumerable<Employee> GetAll()
        {
            return _employees;
        }
    }
}