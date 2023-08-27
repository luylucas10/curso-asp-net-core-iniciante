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

        // GET: Subtarefa
        public async Task<IActionResult> Index()
        {
            var cursoInicianteContexto = _context.Subtarefa.Include(s => s.Tarefa);
            return View(await cursoInicianteContexto.ToListAsync());
        }

        // GET: Subtarefa/Details/5
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

            return View(subtarefa);
        }

        // GET: Subtarefa/Create
        public IActionResult Create()
        {
            ViewData["TarefaId"] = new SelectList(_context.Tarefa, "Id", "Id");
            return View();
        }

        // POST: Subtarefa/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,TarefaId,Descricao,RealizadoEm")] Subtarefa subtarefa)
        {
            if (ModelState.IsValid)
            {
                subtarefa.Id = Guid.NewGuid();
                _context.Add(subtarefa);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["TarefaId"] = new SelectList(_context.Tarefa, "Id", "Id", subtarefa.TarefaId);
            return View(subtarefa);
        }

        // GET: Subtarefa/Edit/5
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
            ViewData["TarefaId"] = new SelectList(_context.Tarefa, "Id", "Id", subtarefa.TarefaId);
            return View(subtarefa);
        }

        // POST: Subtarefa/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Guid id, [Bind("Id,TarefaId,Descricao,RealizadoEm")] Subtarefa subtarefa)
        {
            if (id != subtarefa.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(subtarefa);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!SubtarefaExists(subtarefa.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            ViewData["TarefaId"] = new SelectList(_context.Tarefa, "Id", "Id", subtarefa.TarefaId);
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

            return View(subtarefa);
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
            return RedirectToAction(nameof(Index));
        }

        private bool SubtarefaExists(Guid id)
        {
          return (_context.Subtarefa?.Any(e => e.Id == id)).GetValueOrDefault();
        }
    }
}
