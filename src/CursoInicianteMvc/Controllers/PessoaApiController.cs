using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using CursoInicianteMvc.Models;
using CursoInicianteMvc.Services;

namespace CursoInicianteMvc.Controllers
{
    [Route("api/pessoa")]
    [ApiController]
    public class PessoaApiController : ControllerBase
    {
        private readonly IPessoaService _pessoaService;

        public PessoaApiController(IPessoaService pessoaService)
        {
            _pessoaService = pessoaService;
        }

        [HttpGet("{id:guid?}")]
        public async Task<IActionResult> Get(Guid? id, [FromQuery] Filter filtro)
        {
            if (id.HasValue)
                return Ok(await _pessoaService.Find(id.GetValueOrDefault()));

            var resultado = await _pessoaService.Search(filtro);
            return Ok(new { total = resultado.Item1, rows = resultado.Item2 });
        }

        [HttpPost]
        public async Task<ActionResult<Pessoa>> Post(PessoaCadastrarViewModel pessoa)
        {
            var id = await _pessoaService.Create(pessoa);
            return CreatedAtAction("Get", new { id }, pessoa);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Put(Guid id, PessoaEditarViewModel pessoa)
        {
            if (id != pessoa.Id)
                return BadRequest();

            await _pessoaService.Edit(pessoa);
            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(Guid id)
        {
            await _pessoaService.Delete(id);
            return NoContent();
        }
    }
}