using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using CursoInicianteMvc.Data;
using CursoInicianteMvc.Models;

namespace CursoInicianteMvc.Controllers
{
    public class TarefaController : Controller
    {
        private readonly CursoInicianteContexto _context;

        public TarefaController(CursoInicianteContexto context) =>
            _context = context;

        [HttpGet]
        public async Task<JsonResult> Search(Guid? pessoaId, int? limit, int? offset, string? search, string? sort,
            string? order)
        {
            limit = limit.GetValueOrDefault(0) <= 0 ? 15 : limit;
            offset = (offset.GetValueOrDefault(0) <= 0 ? 0 : offset - 1) * limit;

            var consulta = _context
                .Tarefa
                .Include(x => x.Subtarefas)
                .AsQueryable()
                .AsNoTracking();

            if (pessoaId.HasValue)
                consulta = consulta.Where(x => x.PessoaId == pessoaId);

            if (!string.IsNullOrWhiteSpace(search))
                consulta = consulta.Where(x => x.Descricao.Contains(search));

            var total = await consulta.CountAsync();

            consulta = sort switch
            {
                "RealizadoEm" when order == "asc" => consulta.OrderBy(x => x.RealizadoEm),
                "RealizadoEm" when order == "desc" => consulta.OrderByDescending(x => x.RealizadoEm),
                "Descricao" when order == "desc" => consulta.OrderByDescending(x => x.Descricao),
                _ => consulta.OrderBy(x => x.Descricao)
            };

            var rows = await consulta
                .Select(x => new
                {
                    x.Id, x.Descricao, x.RealizadoEm,
                    qntSubtarefas = x.Subtarefas.Count,
                    qntSubtarefasConcluidas = x.Subtarefas.Count(a => a.RealizadoEm.HasValue)
                })
                .Skip(offset.GetValueOrDefault())
                .Take(limit.GetValueOrDefault())
                .ToListAsync();

            return new JsonResult(new { rows, total });
        }

        [HttpGet]
        public async Task<IActionResult> Details(Guid? id)
        {
            if (id == null)
                return NotFound();

            var tarefa = await _context.Tarefa
                .Include(t => t.Pessoa)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (tarefa == null)
                return NotFound();

            return View(new TarefaDetalhesViewModel()
            {
                Id = tarefa.Id,
                PessoaId = tarefa.PessoaId,
                Descricao = tarefa.Descricao,
                RealizadoEm = tarefa.RealizadoEm,
                Pessoa = new PessoaEditarViewModel()
                {
                    Nome = tarefa.Pessoa.Nome
                }
                
            });
        }

        [HttpGet]
        public IActionResult Create(Guid pessoaId)
        {
            ViewData["PessoaId"] = new SelectList(_context.Pessoa, nameof(Pessoa.Id), nameof(Pessoa.Nome), pessoaId);
            return View(new TarefaCadastrarViewModel { PessoaId = pessoaId });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("PessoaId,Descricao")] TarefaCadastrarViewModel tarefa)
        {
            if (!ModelState.IsValid)
            {
                ViewData["PessoaId"] = new SelectList(_context.Pessoa, nameof(Pessoa.Id), nameof(Pessoa.Nome),
                    tarefa.PessoaId);
                return View(tarefa);
            }

            var entidade = new Tarefa
            {
                PessoaId = tarefa.PessoaId,
                Descricao = tarefa.Descricao
            };

            _context.Add(entidade);
            await _context.SaveChangesAsync();
            return RedirectToAction("Details", "Pessoa", new { Id = tarefa.PessoaId });
        }

        [HttpGet]
        public async Task<IActionResult> Edit(Guid? id)
        {
            if (id == null)
                return NotFound();

            var tarefa = await _context.Tarefa.Include(x => x.Pessoa).Where(x => x.Id == id).FirstOrDefaultAsync();

            if (tarefa == null)
                return NotFound();

            ViewData["PessoaId"] =
                new SelectList(_context.Pessoa, nameof(Pessoa.Id), nameof(Pessoa.Nome), tarefa.PessoaId);

            return View(new TarefaEditarViewModel
            {
                Id = tarefa.Id,
                PessoaId = tarefa.PessoaId,
                Descricao = tarefa.Descricao
            });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Guid id, [Bind("Id,PessoaId,Descricao,RealizadoEm")] TarefaEditarViewModel tarefa)
        {
            if (id != tarefa.Id)
                return NotFound();

            if (!ModelState.IsValid)
            {
                ViewData["PessoaId"] = new SelectList(_context.Pessoa, "Id", nameof(Pessoa.Nome), tarefa.PessoaId);
                return View(tarefa);
            }

            try
            {
                var entidade = await _context.Tarefa.FindAsync(id);
                entidade.Descricao = tarefa.Descricao;
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!await TarefaExists(tarefa.Id))
                    return NotFound();

                throw;
            }

            return RedirectToAction("Details", "Pessoa", new { Id = tarefa.PessoaId });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<RedirectToActionResult> Concluir(Guid id)
        {
            var entidade = await _context.Tarefa.Include(x => x.Subtarefas).Where(x => x.Id == id)
                .FirstOrDefaultAsync();

            if (entidade == null)
                return RedirectToAction("Index", "Pessoa");

            foreach (var sub in entidade.Subtarefas)
                sub.RealizadoEm = DateTime.Now;

            entidade.RealizadoEm = DateTime.Now;

            await _context.SaveChangesAsync();

            return RedirectToAction("Details", new { id });
        }

        [HttpGet]
        public async Task<IActionResult> Delete(Guid? id)
        {
            if (id == null)
                return NotFound();

            var tarefa = await _context.Tarefa
                .Include(t => t.Pessoa)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (tarefa == null)
                return NotFound();

            return View(new TarefaDetalhesViewModel()
            {
                Id = tarefa.Id,
                Descricao = tarefa.Descricao,
                RealizadoEm = tarefa.RealizadoEm,
                Pessoa = new PessoaEditarViewModel()
                {
                    Nome = tarefa.Pessoa.Nome
                }
            });
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(Guid id)
        {
            var tarefa = await _context.Tarefa.FindAsync(id);

            if (tarefa != null)
                _context.Tarefa.Remove(tarefa);

            await _context.SaveChangesAsync();

            return RedirectToAction("Details", "Pessoa", new { Id = tarefa.PessoaId });
        }

        private async Task<bool> TarefaExists(Guid id) =>
            await _context.Tarefa.AnyAsync(e => e.Id == id);
    }
}