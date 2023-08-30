using Microsoft.AspNetCore.Mvc;
using CursoInicianteMvc.Models;
using CursoInicianteMvc.Services;

namespace CursoInicianteMvc.Controllers
{
    public class TarefaController : Controller
    {
        private readonly ITarefaService _tarefaService;

        public TarefaController(ITarefaService tarefaService) =>
            _tarefaService = tarefaService;

        [HttpGet]
        public async Task<JsonResult> Search(TarefaFilter filtro)
        {
            var result = await _tarefaService.Search(filtro);
            return new JsonResult(new { rows = result.Item2, total = result.Item1 });
        }

        [HttpGet]
        public async Task<IActionResult> Details(Guid? id)
        {
            var tarefa = await _tarefaService.FindDetails(id.GetValueOrDefault());
            if (tarefa == null) return NotFound();
            return View(tarefa);
        }

        [HttpGet]
        public async Task<IActionResult> Create(Guid pessoaId)
        {
            var tarefa = await _tarefaService.FindCreate(pessoaId);
            return tarefa == null
                ? NotFound()
                : View(tarefa);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(TarefaCadastroViewModel tarefa)
        {
            if (!ModelState.IsValid)
            {
                tarefa = (await _tarefaService.FindCreate(tarefa.PessoaId))!;
                return View(tarefa);
            }

            await _tarefaService.Create(tarefa);
            return RedirectToAction(nameof(Details), "Pessoa", new { Id = tarefa.PessoaId });
        }

        [HttpGet]
        public async Task<IActionResult> Edit(Guid? id)
        {
            var tarefa = await _tarefaService.FindEdit(id.GetValueOrDefault());
            if (tarefa == null) return NotFound();
            return View(tarefa);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(TarefaEditarViewModel tarefa)
        {
            if (!ModelState.IsValid)
            {
                tarefa = (await _tarefaService.FindEdit(tarefa.Id))!;
                return View(tarefa);
            }

            await _tarefaService.Edit(tarefa);

            return RedirectToAction(nameof(Details), new { tarefa.Id });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<RedirectToActionResult> Finish(Guid? id)
        {
            await _tarefaService.Finish(id.GetValueOrDefault());
            return RedirectToAction(nameof(Details), new { id });
        }

        [HttpGet]
        public async Task<IActionResult> Delete(Guid? id)
        {
            var tarefa = await _tarefaService.FindDetails(id.GetValueOrDefault());
            if (tarefa == null) return NotFound();
            return View(tarefa);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(Guid id)
        {
            var tarefa = await _tarefaService.FindDetails(id);
            await _tarefaService.Delete(id);
            return RedirectToAction(nameof(Details), "Pessoa", new { Id = tarefa.PessoaId });
        }
    }
}