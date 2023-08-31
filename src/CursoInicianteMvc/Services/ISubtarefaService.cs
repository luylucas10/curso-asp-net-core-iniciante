using System;
using System.Collections;
using System.Threading.Tasks;
using CursoInicianteMvc.Models;

namespace CursoInicianteMvc.Services;

public interface ISubtarefaService
{
    Task<SubtarefaCadastrarViewModel> FindCreate(Guid tarefaId);
    Task<SubtarefaEditarViewModel?> FindEdit(Guid id);
    Task<SubtarefaDetalharViewModel?> FindDetails(Guid id);
    Task<Tuple<int, IEnumerable>> Search(SubtarefaFilter filtro);
    Task<Guid> Create(SubtarefaCadastrarViewModel subtarefa);
    Task Edit(SubtarefaEditarViewModel tarefa);
    Task Delete(Guid id);
    Task Finish(Guid id);
}