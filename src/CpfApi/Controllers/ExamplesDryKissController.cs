using System.Text.RegularExpressions;
using CpfApi.Models;
using Microsoft.AspNetCore.Mvc;

namespace CpfApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ExamplesDryKissController : ControllerBase
    {
        // ==============================
        // 1) EXEMPLOS DRY
        // ==============================

        // ❌ EXEMPLO QUE FERE DRY
        // A MESMA lógica de validação de CPF aparece copiada em mais de um lugar.
        // Se a regra mudar, eu teria que lembrar de mudar em todos os lugares.
        [HttpPost("bad-dry-1")]
        public IActionResult CreatePersonBadDry1([FromBody] Person person)
        {
            // Lógica de validação de CPF copiada aqui:
            var cpf = Regex.Replace(person.Cpf ?? "", "[^0-9]", "");
            if (cpf.Length != 11 || new string(cpf[0], 11) == cpf)
            {
                return BadRequest("CPF inválido (bad-dry-1).");
            }


            return Ok("Exemplo bad-dry-1 executado.");
        }

        [HttpPost("bad-dry-2")]
        public IActionResult CreatePersonBadDry2([FromBody] Person person)
        {
            // MESMA lógica copiada de novo -> VIOLA DRY
            var cpf = Regex.Replace(person.Cpf ?? "", "[^0-9]", "");
            if (cpf.Length != 11 || new string(cpf[0], 11) == cpf)
            {
                return BadRequest("CPF inválido (bad-dry-2).");
            }

            return Ok("Exemplo bad-dry-2 executado.");
        }

        // ✅ EXEMPLO QUE RESPEITA DRY
        // A lógica de validação de CPF está em UM único método auxiliar.
        private string? ValidateCpfOnce(string? cpfOriginal)
        {
            if (string.IsNullOrWhiteSpace(cpfOriginal))
                return null;

            var cpf = Regex.Replace(cpfOriginal, "[^0-9]", "");
            if (cpf.Length != 11 || new string(cpf[0], 11) == cpf)
                return null;

            return cpf;
        }

        [HttpPost("good-dry")]
        public IActionResult CreatePersonGoodDry([FromBody] Person person)
        {
            var cpf = ValidateCpfOnce(person.Cpf);

            if (cpf is null)
                return BadRequest("CPF inválido (good-dry).");

            
            // Qualquer lugar que precisar validar CPF chama ValidateCpfOnce.
            return Ok("Exemplo que RESPEITA DRY.");
        }

        // ==============================
        // 2) EXEMPLOS KISS
        // ==============================

        // ❌ EXEMPLO QUE FERE KISS
        // Interface confusa, usa Dictionary em vez de um modelo simples,
        // muita conversão manual, difícil de ler.
        [HttpPost("bad-kiss")]
        public IActionResult CreateBadKiss([FromBody] Dictionary<string, object> body)
        {
            // Poderia ser só: public IActionResult CreateBadKiss(Person p)
            // mas aqui está desnecessariamente complicado.
            if (!body.ContainsKey("name") || !body.ContainsKey("cpf"))
            {
                return BadRequest("Corpo da requisição está confuso. Use 'name' e 'cpf'.");
            }

            var name = body["name"]?.ToString() ?? "";
            var cpf = body["cpf"]?.ToString() ?? "";

            if (string.IsNullOrWhiteSpace(name))
            {
                return BadRequest("Nome obrigatório (bad-kiss).");
            }

            cpf = Regex.Replace(cpf, "[^0-9]", "");
            if (cpf.Length != 11)
            {
                return BadRequest("CPF inválido (bad-kiss, lógica verbosa).");
            }

            // Muita coisa pra fazer algo simples -> FERE KISS.
            return Ok("Este endpoint é propositalmente confuso e NÃO segue KISS.");
        }

        // ✅ EXEMPLO QUE RESPEITA KISS
        // Assinatura simples, recebe um modelo forte (Person),
        // valida poucas coisas e retorna.
        [HttpPost("good-kiss")]
        public IActionResult CreateGoodKiss([FromBody] Person person)
        {
            if (string.IsNullOrWhiteSpace(person.Name))
                return BadRequest("Nome é obrigatório (good-kiss).");

            var cpf = Regex.Replace(person.Cpf ?? "", "[^0-9]", "");
            if (cpf.Length != 11)
                return BadRequest("CPF inválido (good-kiss).");

            return Ok("Este endpoint é simples, direto e segue KISS.");
        }
    }
}
