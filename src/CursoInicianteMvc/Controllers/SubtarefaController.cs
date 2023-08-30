using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using CursoInicianteMvc.Data;
using CursoInicianteMvc.Models;

namespace CursoInicianteMvc.Controllers
{
    public class SubtarefaController : Controller
    {
        private readonly CursoInicianteContexto _context;

        public SubtarefaController(CursoInicianteContexto context)
        {
            _context = context;
        }

        public async Task<JsonResult> Search(Guid? tarefaId, int? limit, int? offset, string? search, string? sort,
            string? order)
        {
            limit = limit.GetValueOrDefault(0) <= 0 ? 15 : limit;
            offset = (offset.GetValueOrDefault(0) <= 0 ? 0 : offset - 1) * limit;

            var consulta = _context
                .Subtarefa
                .AsQueryable()
                .AsNoTracking()
                .Where(x => x.TarefaId == tarefaId);

            if (!string.IsNullOrWhiteSpace(search))
                consulta = consulta.Where(x => x.Descricao.Contains(search));

            var total = await consulta.CountAsync();

            consulta = sort switch
            {
                "RealizadoEm" when order == "asc" => consulta.OrderBy(x => x.RealizadoEm),
                "RealizadoEm" when order == "desc" => consulta.OrderByDescending(x => x.RealizadoEm),
                "Descricao" when order == "desc" => consulta.OrderByDescending(x => x.Descricao),
                _ => consulta.OrderBy(x => x.Descricao).ThenBy(x => x.RealizadoEm)
            };

            var rows = await consulta
                .Select(x => new
                {
                    x.Id, x.Descricao, x.RealizadoEm
                })
                .Skip(offset.GetValueOrDefault())
                .Take(limit.GetValueOrDefault())
                .ToListAsync();

            return new JsonResult(new { rows, total });
        }

        public async Task<IActionResult> Details(Guid? id)
        {
            if (id == null || _context.Subtarefa == null)
            {
                return NotFound();
            }

            var subtarefa = await _context.Subtarefa
                .Include(s => s.Tarefa)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (subtarefa == null)
            {
                return NotFound();
            }

            return View(new SubtarefaDetalhesViewModel()
            {
                Id = subtarefa.Id,
                TarefaId = subtarefa.TarefaId,
                Descricao = subtarefa.Descricao,
                RealizadoEm = subtarefa.RealizadoEm,
                Tarefa = new TarefaDetalhesViewModel()
                {
                    Descricao = subtarefa.Tarefa.Descricao
                }
            });
        }

        public IActionResult Create(Guid tarefaId)
        {
            ViewData["TarefaId"] = new SelectList(_context.Tarefa, "Id", nameof(Tarefa.Descricao), tarefaId);
            return View(new SubtarefaCadastrarViewModel() { TarefaId = tarefaId });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("TarefaId,Descricao")] SubtarefaCadastrarViewModel subtarefa)
        {
            if (ModelState.IsValid)
            {
                _context.Add(new Subtarefa
                {
                    Id = Guid.NewGuid(),
                    TarefaId = subtarefa.TarefaId,
                    Descricao = subtarefa.Descricao
                });

                await _context.SaveChangesAsync();
                return RedirectToAction("Details", "Tarefa", new { Id = subtarefa.TarefaId });
            }

            ViewData["TarefaId"] = new SelectList(_context.Tarefa, "Id", nameof(Tarefa.Descricao), subtarefa.TarefaId);
            return View(subtarefa);
        }

        public async Task<IActionResult> Edit(Guid? id)
        {
            if (id == null || _context.Subtarefa == null)
            {
                return NotFound();
            }

            var subtarefa = await _context.Subtarefa.FindAsync(id);
            if (subtarefa == null)
            {
                return NotFound();
            }

            ViewData["TarefaId"] = new SelectList(_context.Tarefa, "Id", nameof(Tarefa.Descricao), subtarefa.TarefaId);
            return View(new SubtarefaEditarViewModel
                { TarefaId = subtarefa.TarefaId, Descricao = subtarefa.Descricao, Id = subtarefa.Id });
        }

        // POST: Subtarefa/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Guid id,
            [Bind("Id,TarefaId,Descricao,RealizadoEm")]
            SubtarefaEditarViewModel subtarefa)
        {
            if (id != subtarefa.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    var entidade = await _context.Subtarefa.FindAsync(subtarefa.Id);
                    entidade.Descricao = subtarefa.Descricao;
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!SubtarefaExists(subtarefa.Id))
                        return NotFound();
                    throw;
                }

                return RedirectToAction("Details", "Tarefa", new { Id = subtarefa.TarefaId });
            }

            ViewData["TarefaId"] = new SelectList(_context.Tarefa, "Id", nameof(Tarefa.Descricao), subtarefa.TarefaId);
            return View(subtarefa);
        }

        // GET: Subtarefa/Delete/5
        public async Task<IActionResult> Delete(Guid? id)
        {
            if (id == null || _context.Subtarefa == null)
            {
                return NotFound();
            }

            var subtarefa = await _context.Subtarefa
                .Include(s => s.Tarefa)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (subtarefa == null)
            {
                return NotFound();
            }

            return View(new SubtarefaDetalhesViewModel { 
                    TarefaId = subtarefa.TarefaId,
                    Descricao = subtarefa.Descricao, 
                    Id = subtarefa.Id,
                    Tarefa = new TarefaDetalhesViewModel(){Descricao = subtarefa.Tarefa.Descricao}
            });
        }

        // POST: Subtarefa/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(Guid id)
        {
            if (_context.Subtarefa == null)
            {
                return Problem("Entity set 'CursoInicianteContexto.Subtarefa'  is null.");
            }

            var subtarefa = await _context.Subtarefa.FindAsync(id);
            if (subtarefa != null)
            {
                _context.Subtarefa.Remove(subtarefa);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction("Details", "Tarefa", new { Id = subtarefa.TarefaId });
        }

        private bool SubtarefaExists(Guid id)
        {
            return (_context.Subtarefa?.Any(e => e.Id == id)).GetValueOrDefault();
        }

        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> Concluir(Guid id)
        {
            var entidade = await _context.Subtarefa.FindAsync(id);

            if (entidade == null)
                return RedirectToAction("Index", "Pessoa");

            entidade.RealizadoEm = DateTime.Now;

            await _context.SaveChangesAsync();

            return RedirectToAction("Details", new { id });
        }
    }
}