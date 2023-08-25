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
    public class PessoaController : Controller
    {
        private readonly CursoInicianteContexto _context;

        public PessoaController(CursoInicianteContexto context)
        {
            _context = context;
        }

        // GET: Pessoa
        public ViewResult Index() => View();

        public async Task<JsonResult> Search(int? limit, int? offset, string? search, string? sort, string? order)
        {
            limit = limit.GetValueOrDefault(0) <= 0 ? 0 : limit - 1;
            offset = offset.GetValueOrDefault(0) <= 0 ? 15 : offset;

            var consulta = _context.Pessoa.AsNoTracking();

            if (!string.IsNullOrWhiteSpace(search))
                consulta = consulta.Where(x =>
                    x.Nome.Contains(search) || x.Email.Contains(search) || x.Celular.Contains(search));

            var total = await consulta.CountAsync();

            
            if (sort == "Nome" && order == "desc") consulta = consulta.OrderByDescending(x => x.Nome);
            else if (sort == "Email" && order == "asc") consulta = consulta.OrderBy(x => x.Email);
            else if (sort == "Email" && order == "desc") consulta = consulta.OrderByDescending(x => x.Email);
            else if (sort == "Celular" && order == "asc") consulta = consulta.OrderBy(x => x.Celular);
            else if (sort == "Celular" && order == "desc") consulta = consulta.OrderByDescending(x => x.Celular);
            else consulta = consulta.OrderBy(x => x.Nome);

            var rows = await consulta.ToListAsync();
            
            return new JsonResult(new { rows, total });
        }

        // GET: Pessoa/Details/5
        public async Task<IActionResult> Details(Guid? id)
        {
            if (id == null || _context.Pessoa == null)
            {
                return NotFound();
            }

            var pessoa = await _context.Pessoa
                .FirstOrDefaultAsync(m => m.Id == id);
            if (pessoa == null)
            {
                return NotFound();
            }

            return View(pessoa);
        }

        // GET: Pessoa/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Pessoa/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Nome,Email,Celular")] Pessoa pessoa)
        {
            if (ModelState.IsValid)
            {
                pessoa.Id = Guid.NewGuid();
                _context.Add(pessoa);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }

            return View(pessoa);
        }

        // GET: Pessoa/Edit/5
        public async Task<IActionResult> Edit(Guid? id)
        {
            if (id == null || _context.Pessoa == null)
            {
                return NotFound();
            }

            var pessoa = await _context.Pessoa.FindAsync(id);
            if (pessoa == null)
            {
                return NotFound();
            }

            return View(pessoa);
        }

        // POST: Pessoa/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Guid id, [Bind("Id,Nome,Email,Celular")] Pessoa pessoa)
        {
            if (id != pessoa.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(pessoa);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!PessoaExists(pessoa.Id))
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

            return View(pessoa);
        }

        // GET: Pessoa/Delete/5
        public async Task<IActionResult> Delete(Guid? id)
        {
            if (id == null || _context.Pessoa == null)
            {
                return NotFound();
            }

            var pessoa = await _context.Pessoa
                .FirstOrDefaultAsync(m => m.Id == id);
            if (pessoa == null)
            {
                return NotFound();
            }

            return View(pessoa);
        }

        // POST: Pessoa/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(Guid id)
        {
            if (_context.Pessoa == null)
            {
                return Problem("Entity set 'CursoInicianteContexto.Pessoa'  is null.");
            }

            var pessoa = await _context.Pessoa.FindAsync(id);
            if (pessoa != null)
            {
                _context.Pessoa.Remove(pessoa);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool PessoaExists(Guid id)
        {
            return (_context.Pessoa?.Any(e => e.Id == id)).GetValueOrDefault();
        }
    }
}