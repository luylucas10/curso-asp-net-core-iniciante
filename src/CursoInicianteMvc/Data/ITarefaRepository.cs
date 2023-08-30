using System.Collections;
using CursoInicianteMvc.Models;

namespace CursoInicianteMvc.Data;

public interface ITarefaRepository
{
    Task<Tarefa?> Find(Guid id);
    Task<Tuple<int, IEnumerable>> Search(TarefaFilter filtro);
    Task Create(Tarefa tarefa);
    Task Edit(Tarefa tarefa);
    Task Delete(Guid id);
}