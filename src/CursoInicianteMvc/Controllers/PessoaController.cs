using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using CursoInicianteMvc.Data;
using CursoInicianteMvc.Models;

namespace CursoInicianteMvc.Controllers
{
    public class PessoaController : Controller
    {
        private readonly CursoInicianteContexto _context;

        public PessoaController(CursoInicianteContexto context) => _context = context;

        public ViewResult Index() => View();

        public async Task<JsonResult> Search(int? limit, int? offset, string? search, string? sort, string? order)
        {
            limit = limit.GetValueOrDefault(0) <= 0 ? 15 : limit;
            offset = (offset.GetValueOrDefault(0) <= 0 ? 0 : offset - 1) * limit;
            

            var consulta = _context
                .Pessoa
                .Include(x => x.Tarefas)
                .AsQueryable()
                .AsNoTracking();

            if (!string.IsNullOrWhiteSpace(search))
                consulta = consulta.Where(x =>
                    x.Nome.Contains(search)
                    || x.Email.Contains(search)
                    || x.Celular.Contains(search));

            var total = await consulta.CountAsync();

            consulta = sort switch
            {
                "Nome" when order == "desc" => consulta.OrderByDescending(x => x.Nome),
                "Email" when order == "asc" => consulta.OrderBy(x => x.Email),
                "Email" when order == "desc" => consulta.OrderByDescending(x => x.Email),
                "Celular" when order == "asc" => consulta.OrderBy(x => x.Celular),
                "Celular" when order == "desc" => consulta.OrderByDescending(x => x.Celular),
                _ => consulta.OrderBy(x => x.Nome)
            };

            var rows = await consulta
                .Select(x => new
                {
                    x.Id, x.Nome, x.Email, x.Celular,
                    qntTarefas = x.Tarefas.Count,
                    qntSubtarefas = x.Tarefas.SelectMany(x => x.Subtarefas).Count()
                })
                .Skip(offset.GetValueOrDefault())
                .Take(limit.GetValueOrDefault())
                .ToListAsync();

            return new JsonResult(new { rows, total });
        }

        public async Task<IActionResult> Details(Guid? id)
        {
            if (id == null)
                return NotFound();

            var pessoa = await _context.Pessoa.FirstOrDefaultAsync(m => m.Id == id);

            if (pessoa == null)
                return NotFound();

            return View(pessoa);
        }

        public ViewResult Create() => View();

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(PessoaCadastroViewModel novaPessoa)
        {
            if (ModelState.IsValid)
            {
                var pessoa = new Pessoa()
                {
                    Id = Guid.NewGuid(), Nome = novaPessoa.Nome, Email = novaPessoa.Email, Celular = novaPessoa.Celular
                };
                _context.Add(pessoa);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }

            return View(novaPessoa);
        }

        public async Task<IActionResult> Edit(Guid? id)
        {
            if (id == null)
                return NotFound();

            var pessoa = await _context.Pessoa.FindAsync(id);

            if (pessoa == null)
                return NotFound();

            return View(new PessoaEditarViewModel(pessoa.Id, pessoa.Nome, pessoa.Email, pessoa.Celular));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Guid id, [Bind("Id,Nome,Email,Celular")] PessoaEditarViewModel pessoa)
        {
            if (id != pessoa.Id)
                return NotFound();

            if (ModelState.IsValid)
            {
                try
                {
                    var entidade = await _context.Pessoa.FindAsync(pessoa.Id);
                    entidade.Nome = pessoa.Nome;
                    entidade.Email = pessoa.Email;
                    entidade.Celular = pessoa.Celular;
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!PessoaExists(pessoa.Id))
                        return NotFound();
                    throw;
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

            return View(new PessoaEditarViewModel(pessoa.Id, pessoa.Nome, pessoa.Email, pessoa.Celular));
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