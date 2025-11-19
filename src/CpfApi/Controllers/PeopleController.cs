using CpfApi.Models;
using CpfApi.Services;
using Microsoft.AspNetCore.Mvc;

namespace CpfApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class PeopleController : ControllerBase
    {
        // "banco de dados" temporário só pra testar
        private static readonly List<Person> _people = new();
        private static int _nextId = 1;

        private readonly CpfValidator _cpfValidator;

        public PeopleController(CpfValidator cpfValidator)
        {
            _cpfValidator = cpfValidator;
        }

        // GET api/people
        [HttpGet]
        public IActionResult GetAll()
        {
            return Ok(_people);
        }

        // GET api/people/1
        [HttpGet("{id:int}")]
        public IActionResult GetById(int id)
        {
            var person = _people.FirstOrDefault(p => p.Id == id);
            if (person is null)
                return NotFound();

            return Ok(person);
        }

        // POST api/people
        [HttpPost]
        public IActionResult Create([FromBody] Person person)
        {
            // 1. validar nome
            if (string.IsNullOrWhiteSpace(person.Name))
                return BadRequest("Nome é obrigatório.");

            // 2. validar CPF usando o serviço (DRY e KISS)
            var cpfNormalizado = _cpfValidator.NormalizeAndValidate(person.Cpf);
            if (cpfNormalizado is null)
                return BadRequest("CPF inválido.");

            // 3. checar duplicado
            if (_people.Any(p => p.Cpf == cpfNormalizado))
                return Conflict("Já existe uma pessoa com esse CPF.");

            person.Id = _nextId++;
            person.Cpf = cpfNormalizado;

            _people.Add(person);

            // 201 Created apontando pro GET /api/people/{id}
            return CreatedAtAction(nameof(GetById), new { id = person.Id }, person);
        }
    }
}
