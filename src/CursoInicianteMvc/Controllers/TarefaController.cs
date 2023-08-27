using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
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

        // GET: Tarefa
        public async Task<IActionResult> Index()
        {
            var cursoInicianteContexto = _context.Tarefa.Include(t => t.Pessoa);
            return View(await cursoInicianteContexto.ToListAsync());
        }

        // GET: Tarefa/Details/5
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
        public async Task<IActionResult> Create([Bind("PessoaId,Descricao,RealizadoEm")] TarefaViewModel tarefa)
        {
            if (ModelState.IsValid)
            {
                var tarefaEntidade = new Tarefa
                {
                    PessoaId = tarefa.PessoaId,
                    Descricao = tarefa.Descricao
                };

                _context.Add(tarefaEntidade);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }

            ViewData["PessoaId"] = new SelectList(_context.Pessoa, "Id", nameof(Pessoa.Nome), tarefa.PessoaId);
            return View(tarefa);
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
            [Bind("Id,PessoaId,Descricao,RealizadoEm")] TarefaEditarViewModel tarefa)
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