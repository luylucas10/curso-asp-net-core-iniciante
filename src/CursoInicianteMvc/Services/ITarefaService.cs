using System.Collections;
using CursoInicianteMvc.Models;

namespace CursoInicianteMvc.Services;

public interface ITarefaService
{
    Task<TarefaCadastrarViewModel?> FindCreate(Guid pessoaId);
    Task<TarefaEditarViewModel?> FindEdit(Guid id);
    Task<TarefaDetalharViewModel?> FindDetails(Guid id);
    Task<Tuple<int, IEnumerable>> Search(TarefaFilter filtro);
    Task<Guid> Create(TarefaCadastrarViewModel tarefa);
    Task Edit(TarefaEditarViewModel tarefa);
    Task Delete(Guid id);
    Task Finish(Guid id);
}