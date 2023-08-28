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

        public TarefaController(CursoInicianteContexto context) => _context = context;

        public ViewResult Index() => View();

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
        
        public async Task<IActionResult> Details(Guid? id)
        {
            if (id == null || _context.Tarefa == null)
            {
                return NotFound();
            }

            var tarefa = await _context.Tarefa
                .Include(t => t.Pessoa)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (tarefa == null)
            {
                return NotFound();
            }

            return View(tarefa);
        }

        // GET: Tarefa/Create
        public IActionResult Create()
        {
            ViewData["PessoaId"] = new SelectList(_context.Pessoa, "Id", nameof(Pessoa.Nome));
            return View();
        }

        // POST: Tarefa/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("PessoaId,Descricao")] TarefaCadastrarViewModel tarefaCadastrar)
        {
            if (ModelState.IsValid)
            {
                var tarefaEntidade = new Tarefa
                {
                    PessoaId = tarefaCadastrar.PessoaId,
                    Descricao = tarefaCadastrar.Descricao,
                    RealizadoEm = null
                };

                _context.Add(tarefaEntidade);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }

            ViewData["PessoaId"] = new SelectList(_context.Pessoa, "Id", nameof(Pessoa.Nome), tarefaCadastrar.PessoaId);
            return View(tarefaCadastrar);
        }

        // GET: Tarefa/Edit/5
        public async Task<IActionResult> Edit(Guid? id)
        {
            if (id == null || _context.Tarefa == null)
            {
                return NotFound();
            }

            var tarefa = await _context.Tarefa.FindAsync(id);
            if (tarefa == null)
            {
                return NotFound();
            }

            ViewData["PessoaId"] = new SelectList(_context.Pessoa, "Id", nameof(Pessoa.Nome), tarefa.PessoaId);

            return View(new TarefaEditarViewModel
            {
                Descricao = tarefa.Descricao,
                PessoaId = tarefa.PessoaId,
                Id = tarefa.Id,
                RealizadoEm = tarefa.RealizadoEm
            });
        }

        // POST: Tarefa/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Guid id,
            [Bind("Id,PessoaId,Descricao,RealizadoEm")]
            TarefaEditarViewModel tarefa)
        {
            if (id != tarefa.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    var entidade = await _context.Tarefa.FindAsync(id);
                    entidade.Descricao = tarefa.Descricao;
                    entidade.RealizadoEm = tarefa.RealizadoEm;
                    entidade.PessoaId = tarefa.PessoaId;
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!TarefaExists(tarefa.Id))
                        return NotFound();

                    throw;
                }

                return RedirectToAction(nameof(Index));
            }

            ViewData["PessoaId"] = new SelectList(_context.Pessoa, "Id", nameof(Pessoa.Nome), tarefa.PessoaId);
            return View(tarefa);
        }

        // GET: Tarefa/Delete/5
        public async Task<IActionResult> Delete(Guid? id)
        {
            if (id == null || _context.Tarefa == null)
            {
                return NotFound();
            }

            var tarefa = await _context.Tarefa
                .Include(t => t.Pessoa)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (tarefa == null)
            {
                return NotFound();
            }

            return View(tarefa);
        }

        // POST: Tarefa/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(Guid id)
        {
            if (_context.Tarefa == null)
            {
                return Problem("Entity set 'CursoInicianteContexto.Tarefa'  is null.");
            }

            var tarefa = await _context.Tarefa.FindAsync(id);
            if (tarefa != null)
            {
                _context.Tarefa.Remove(tarefa);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool TarefaExists(Guid id)
        {
            return (_context.Tarefa?.Any(e => e.Id == id)).GetValueOrDefault();
        }
    }
}