using System;
using System.Collections;
using System.Threading.Tasks;
using CursoInicianteMvc.Models;

namespace CursoInicianteMvc.Services;

public interface IPessoaService
{
    Task<PessoaEditarViewModel> Find(Guid id);
    Task<Tuple<int, IEnumerable>> Search(Filter filtro);
    Task<Guid> Create(PessoaCadastrarViewModel pessoa);
    Task Edit(PessoaEditarViewModel pessoa);
    Task Delete(Guid id);
}