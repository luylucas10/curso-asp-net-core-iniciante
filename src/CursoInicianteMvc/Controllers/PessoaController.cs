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

        [HttpGet]
        public ViewResult Index() => View();

        [HttpGet]
        public async Task<JsonResult> Search(Filter filtro)
        {
            filtro.PreparePagination();

            var consulta = _context
                .Pessoa
                .Include(x => x.Tarefas)
                .AsQueryable()
                .AsNoTracking();

            if (!string.IsNullOrWhiteSpace(filtro.Search))
                consulta = consulta.Where(x =>
                    x.Nome.Contains(filtro.Search)
                    || x.Email.Contains(filtro.Search)
                    || x.Celular.Contains(filtro.Search));

            var total = await consulta.CountAsync();

            consulta = filtro.Sort switch
            {
                "Nome" when filtro.Order == "desc" => consulta.OrderByDescending(x => x.Nome),
                "Email" when filtro.Order == "asc" => consulta.OrderBy(x => x.Email),
                "Email" when filtro.Order == "desc" => consulta.OrderByDescending(x => x.Email),
                "Celular" when filtro.Order == "asc" => consulta.OrderBy(x => x.Celular),
                "Celular" when filtro.Order == "desc" => consulta.OrderByDescending(x => x.Celular),
                _ => consulta.OrderBy(x => x.Nome)
            };

            var rows = await consulta
                .Select(x => new
                {
                    x.Id, x.Nome, x.Email, x.Celular,
                    qntTarefas = x.Tarefas.Count,
                    qntSubtarefas = x.Tarefas.SelectMany(x => x.Subtarefas).Count()
                })
                .Skip(filtro.Offset.GetValueOrDefault())
                .Take(filtro.Limit.GetValueOrDefault())
                .ToListAsync();

            return new JsonResult(new { rows, total });
        }

        [HttpGet]
        public async Task<IActionResult> Details(Guid? id)
        {
            var pessoa = await _context.Pessoa.FindAsync(id);

            if (pessoa == null)
                return NotFound();

            return View(new PessoaEditarViewModel
            {
                Id = pessoa.Id,
                Nome = pessoa.Nome,
                Email = pessoa.Email,
                Celular = pessoa.Celular
            });
        }

        [HttpGet]
        public ViewResult Create() => View();

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(PessoaCadastroViewModel nova)
        {
            if (!ModelState.IsValid)
                return View(nova);

            var pessoa = new Pessoa
            {
                Id = Guid.NewGuid(),
                Nome = nova.Nome,
                Email = nova.Email,
                Celular = nova.Celular
            };
            _context.Add(pessoa);
            await _context.SaveChangesAsync();
            return RedirectToAction("Details", new { pessoa.Id });
        }

        [HttpGet]
        public async Task<IActionResult> Edit(Guid? id)
        {
            var pessoa = await _context.Pessoa.FindAsync(id);

            if (pessoa == null)
                return NotFound();

            return View(new PessoaEditarViewModel
                { Id = pessoa.Id, Nome = pessoa.Nome, Email = pessoa.Email, Celular = pessoa.Celular });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Guid id, [Bind("Id,Nome,Email,Celular")] PessoaEditarViewModel pessoa)
        {
            if (!ModelState.IsValid)
                return View(pessoa);

            var entidade = await _context.Pessoa.FindAsync(pessoa.Id);

            if (id != pessoa.Id || entidade == null)
                return NotFound();

            entidade.Nome = pessoa.Nome;
            entidade.Email = pessoa.Email;
            entidade.Celular = pessoa.Celular;
            await _context.SaveChangesAsync();

            return RedirectToAction("Details", new { pessoa.Id });
        }

        [HttpGet]
        public async Task<IActionResult> Delete(Guid? id)
        {
            var pessoa = await _context.Pessoa.FindAsync(id);

            if (id == null || pessoa == null)
                return NotFound();

            return View(new PessoaEditarViewModel
            {
                Id = pessoa.Id,
                Nome = pessoa.Nome,
                Email = pessoa.Email,
                Celular = pessoa.Celular
            });
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(Guid id)
        {
            var pessoa = await _context.Pessoa.FindAsync(id);
            if (pessoa != null) _context.Pessoa.Remove(pessoa);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
    }
}