using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using Provider.Employees;

namespace Provider.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class EmployeesController : ControllerBase
    {
        private readonly EmployeeRepository _employeeRepository = new EmployeeRepository();

        [HttpGet]
        public IEnumerable<Employee> Get()
        {
            return _employeeRepository.GetAll();
        }

        [HttpGet("{id:int}")]
        public Employee Get(int id)
        {
            return _employeeRepository.GetById(id);
        }
    }
}
