using System;
using System.Collections.Generic;
using System.Linq;
using EmployeeApi;
using Microsoft.AspNetCore.Mvc;

namespace EmployeesApi.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class EmployeesController : ControllerBase
    {
        private readonly List<Employee> _employees = new List<Employee>
        {
            new Employee { Id = 1, Name = "Parker", City = "NY", Summary = "Peter Parker is the secret identity of the character Spider-Man."},
            new Employee { Id = 2, Name = "Stark", City = "NY", Summary = "A wealthy American business magnate, playboy, philanthropist, inventor and ingenious scientist."},
            new Employee { Id = 3, Name = "Clark", City = "NY", Summary = "He was found and adopted by farmers Jonathan and Martha Kent, who named him Clark Kent."},
            new Employee { Id = 4, Name = "Grimm", City = "NY", Summary = "Benjamin Jacob Grimm, also known as The Thing, is a fictional superhero appearing in American comic books published by Marvel Comics."},
            new Employee { Id = 5, Name = "Wayne", City = "Gotham City", Summary = "Bruce Wayne, a wealthy American playboy, philanthropist, and industrialist who resides in Gotham City."}
        };

        [HttpGet]
        public IEnumerable<Employee> Get()
        {
            Console.WriteLine($"Count:{_employees.Count}");
            return _employees;
        }

        [HttpGet("{id:int}")]
        public Employee Get(int id)
        {
            var employee = _employees.FirstOrDefault(e => e.Id == id);
            Console.WriteLine($"Id:{employee?.Id} Name:{employee?.Name}");
            return employee;
        }
    }
}
