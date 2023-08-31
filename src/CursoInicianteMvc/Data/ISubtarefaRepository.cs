using System;
using System.Collections;
using System.Threading.Tasks;
using CursoInicianteMvc.Models;

namespace CursoInicianteMvc.Data;

public interface ISubtarefaRepository
{
    Task<Subtarefa> Find(Guid id);
    Task<Tuple<int, IEnumerable>> Search(SubtarefaFilter filtro);
    Task Create(Subtarefa subtarefa);
    Task Edit(Subtarefa subtarefa);
    Task Delete(Guid id);
}