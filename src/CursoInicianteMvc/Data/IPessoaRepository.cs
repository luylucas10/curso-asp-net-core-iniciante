using System.Collections;
using CursoInicianteMvc.Models;

namespace CursoInicianteMvc.Data;

public interface IPessoaRepository
{
    Task<Pessoa?> Find(Guid id);
    Task<Tuple<int, IEnumerable>> Search(Filter filtro);
    Task Create(Pessoa pessoa);
    Task Edit(Pessoa? pessoa);
    Task Delete(Guid id);
}