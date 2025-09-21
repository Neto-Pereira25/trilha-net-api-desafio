using Microsoft.AspNetCore.Mvc;
using TrilhaApiDesafio.Context;
using TrilhaApiDesafio.Models;

namespace TrilhaApiDesafio.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class TarefaController : ControllerBase
    {
        private readonly OrganizadorContext _context;

        public TarefaController(OrganizadorContext context)
        {
            _context = context;
        }

        [HttpGet("{id}")]
        public IActionResult ObterPorId(int id)
        {
            if (id <= 0)
            {
                return BadRequest(new { Mensagem = "O ID deve ser maior que zero." });
            }

            var tarefa = _context.Tarefas.Find(id);

            if (tarefa == null)
            {
                return NotFound();
            }

            return Ok(tarefa);
        }

        [HttpGet("ObterTodos")]
        public IActionResult ObterTodos()
        {
            var tarefas = _context.Tarefas.AsQueryable();
            return Ok(tarefas);
        }

        [HttpGet("ObterPorTitulo")]
        public IActionResult ObterPorTitulo(string titulo)
        {
            if (string.IsNullOrWhiteSpace(titulo))
            {
                return BadRequest(new { Mensage = "O título não pode ser vazio." });
            }

            var tarefas = _context.Tarefas.Where(tarefa => tarefa.Titulo.ToLower().Contains(titulo.ToLower()));

            return Ok(tarefas);
        }

        [HttpGet("ObterPorData")]
        public IActionResult ObterPorData(DateTime data)
        {
            if (data == DateTime.MinValue)
            {
                return BadRequest(new { Mensagem = "A data informada é inválida." });
            }

            var tarefa = _context.Tarefas.Where(x => x.Data.Date == data.Date);
            return Ok(tarefa);
        }

        [HttpGet("ObterPorStatus")]
        public IActionResult ObterPorStatus(EnumStatusTarefa status)
        {

            if (!Enum.IsDefined(typeof(EnumStatusTarefa), status))
            {
                return BadRequest(new { Mensagem = "Status inválido." });
            }

            var tarefas = _context.Tarefas.Where(x => x.Status == status);
            return Ok(tarefas);
        }

        [HttpPost]
        public IActionResult Criar(Tarefa tarefa)
        {
            var erros = ValidarTarefa(tarefa);
            if (erros.Count > 0)
            {
                return BadRequest(new { Erros = erros });
            }

            _context.Add(tarefa);
            _context.SaveChanges();

            return CreatedAtAction(nameof(ObterPorId), new { id = tarefa.Id }, tarefa);
        }

        [HttpPut("{id}")]
        public IActionResult Atualizar(int id, Tarefa tarefa)
        {

            if (id <= 0)
            {
                return BadRequest(new { Mensagem = "O ID deve ser maior que zero." });
            }

            var tarefaBanco = _context.Tarefas.Find(id);

            if (tarefaBanco == null)
                return NotFound();

            var erros = ValidarTarefa(tarefa);
            if (erros.Count > 0)
            {
                return BadRequest(new { Erros = erros });
            }

            tarefaBanco.Titulo = tarefa.Titulo;
            tarefaBanco.Descricao = tarefa.Descricao;
            tarefaBanco.Data = tarefa.Data;
            tarefaBanco.Status = tarefa.Status;

            _context.Tarefas.Update(tarefaBanco);
            _context.SaveChanges();

            return Ok(tarefaBanco);
        }

        [HttpDelete("{id}")]
        public IActionResult Deletar(int id)
        {

            if (id <= 0)
            {
                return BadRequest(new { Mensagem = "O ID deve ser maior que zero." });
            }

            var tarefaBanco = _context.Tarefas.Find(id);

            if (tarefaBanco == null)
                return NotFound();

            _context.Tarefas.Remove(tarefaBanco);
            _context.SaveChanges();

            return NoContent();
        }

        private List<string> ValidarTarefa(Tarefa tarefa)
        {
            var erros = new List<string>();

            if (string.IsNullOrWhiteSpace(tarefa.Titulo))
            {
                erros.Add("O título é obrigatório.");
            }

            if (tarefa.Titulo?.Length > 100)
            {
                erros.Add("O título deve ter no máximo 100 caracteres.");
            }

            if (!string.IsNullOrWhiteSpace(tarefa.Descricao) && tarefa.Descricao.Length > 500)
            {
                erros.Add("A descrição deve ter no máximo 500 caracteres.");
            }

            if (tarefa.Data == DateTime.MinValue)
            {
                erros.Add("A data da tarefa não pode ser vazia.");
            }

            if (!Enum.IsDefined(typeof(EnumStatusTarefa), tarefa.Status))
            {
                erros.Add("O status da tarefa é inválido.");
            }

            return erros;
        }
    }
}
