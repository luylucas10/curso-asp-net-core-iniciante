using System;
using System.Collections;
using System.Threading.Tasks;
using CursoInicianteMvc.Data;
using CursoInicianteMvc.Models;

namespace CursoInicianteMvc.Services;

public class PessoaService : IPessoaService
{
    private readonly IPessoaRepository _repository;

    public PessoaService(IPessoaRepository repository) =>
        _repository = repository;

    public async Task<PessoaEditarViewModel> Find(Guid id)
    {
        var pessoa = await _repository.Find(id);
        return pessoa == null ? null : new PessoaEditarViewModel(pessoa);
    }

    public Task<Tuple<int, IEnumerable>> Search(Filter filtro) =>
        _repository.Search(filtro);

    public async Task<Guid> Create(PessoaCadastrarViewModel pessoa)
    {
        var pessoaEntidade = new Pessoa
        {
            Id = Guid.NewGuid(),
            Nome = pessoa.Nome,
            Email = pessoa.Email,
            Celular = pessoa.Celular
        };

        await _repository.Create(pessoaEntidade);

        return pessoaEntidade.Id;
    }

    public async Task Edit(PessoaEditarViewModel pessoa)
    {
        var entidade = await _repository.Find(pessoa.Id);
        entidade.Nome = pessoa.Nome;
        entidade.Email = pessoa.Email;
        entidade.Celular = pessoa.Celular;
        await _repository.Edit(entidade);
    }

    public async Task Delete(Guid id) => 
        await _repository.Delete(id);
}