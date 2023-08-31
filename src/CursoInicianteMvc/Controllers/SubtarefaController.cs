using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using CursoInicianteMvc.Models;
using CursoInicianteMvc.Services;

namespace CursoInicianteMvc.Controllers
{
    public class SubtarefaController : Controller
    {
        private readonly ISubtarefaService _subtarefaService;

        public SubtarefaController(ISubtarefaService subtarefaService) =>
            _subtarefaService = subtarefaService;

        [HttpGet]
        public async Task<JsonResult> Search(SubtarefaFilter filtro)
        {
            var resultado = await _subtarefaService.Search(filtro);
            return new JsonResult(new { rows = resultado.Item2, total = resultado.Item1 });
        }

        [HttpGet]
        public async Task<IActionResult> Details(Guid? id)
        {
            var subtarefa = await _subtarefaService.FindDetails(id.GetValueOrDefault());
            if (subtarefa == null) return NotFound();
            return View(subtarefa);
        }

        [HttpGet]
        public async Task<IActionResult> Create(Guid tarefaId) =>
            View(await _subtarefaService.FindCreate(tarefaId));

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(SubtarefaCadastrarViewModel subtarefa)
        {
            if (!ModelState.IsValid) return View(await _subtarefaService.FindCreate(subtarefa.TarefaId));
            var id = await _subtarefaService.Create(subtarefa);
            return RedirectToAction(nameof(Details), new { id });
        }

        [HttpGet]
        public async Task<IActionResult> Edit(Guid? id)
        {
            var subtarefa = await _subtarefaService.FindEdit(id.GetValueOrDefault());
            if (subtarefa == null) return NotFound();
            return View(subtarefa);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(SubtarefaEditarViewModel subtarefa)
        {
            if (!ModelState.IsValid) return View(await _subtarefaService.FindEdit(subtarefa.Id));
            await _subtarefaService.Edit(subtarefa);
            return RedirectToAction(nameof(Details), new { subtarefa.Id });
        }

        [HttpGet]
        public async Task<IActionResult> Delete(Guid? id)
        {
            var subtarefa = await _subtarefaService.FindDetails(id.GetValueOrDefault());
            if (subtarefa == null) return NotFound();
            return View(subtarefa);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(Guid id)
        {
            var subtarefa = await _subtarefaService.FindDetails(id);
            await _subtarefaService.Delete(id);
            return RedirectToAction("Details", "Tarefa", new { Id = subtarefa.TarefaId });
        }


        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> Finish(Guid id)
        {
            await _subtarefaService.Finish(id);
            return RedirectToAction("Details", new { id });
        }
    }
}