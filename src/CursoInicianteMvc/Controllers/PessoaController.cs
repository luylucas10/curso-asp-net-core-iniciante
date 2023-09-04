using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using CursoInicianteMvc.Models;
using CursoInicianteMvc.Services;

namespace CursoInicianteMvc.Controllers
{
    public class PessoaController : CursoControllerBase
    {
        private readonly IPessoaService _pessoaService;

        public PessoaController(IPessoaService pessoaService) =>
            _pessoaService = pessoaService;

        [HttpGet]
        public ViewResult Index() => View();

        [HttpGet]
        public async Task<JsonResult> Search(Filter filtro)
        {
            var resultado = await _pessoaService.Search(filtro);
            return new JsonResult(new { total = resultado.Item1, rows = resultado.Item2 });
        }

        [HttpGet]
        public async Task<IActionResult> Details(Guid? id)
        {
            var pessoa = await _pessoaService.Find(id.GetValueOrDefault());
            if (pessoa == null) return RedirectToAction("NaoEncontrado");
            return View(pessoa);
        }

        [HttpGet]
        public ViewResult Create() => View();

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(PessoaCadastrarViewModel pessoa)
        {
            if (!ModelState.IsValid) return View(pessoa);
            var id = await _pessoaService.Create(pessoa);
            return RedirectToAction(nameof(Details), new { id });
        }

        [HttpGet]
        public async Task<IActionResult> Edit(Guid? id)
        {
            var pessoa = await _pessoaService.Find(id.GetValueOrDefault());
            if (pessoa == null) return RedirectToAction("NaoEncontrado");
            return View(pessoa);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(PessoaEditarViewModel pessoa)
        {
            if (!ModelState.IsValid) return View(pessoa);
            await _pessoaService.Edit(pessoa);
            return RedirectToAction(nameof(Details), new { pessoa.Id });
        }

        [HttpGet]
        public async Task<IActionResult> Delete(Guid? id)
        {
            var pessoa = await _pessoaService.Find(id.GetValueOrDefault());
            if (pessoa == null) return RedirectToAction("NaoEncontrado");
            return View(pessoa);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(Guid id)
        {
            await _pessoaService.Delete(id);
            return RedirectToAction(nameof(Index));
        }
    }
}