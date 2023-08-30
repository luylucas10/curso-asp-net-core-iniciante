using System.Collections;
using CursoInicianteMvc.Data;
using CursoInicianteMvc.Models;
using Microsoft.EntityFrameworkCore;

namespace CursoInicianteMvc.Services;

public interface IPessoaService
{
    Task<PessoaEditarViewModel?> Find(Guid id);
    Task<Tuple<int, IEnumerable>> Search(Filter filtro);
    Task<Guid> Create(PessoaCadastrarViewModel pessoa);
    Task Edit(PessoaEditarViewModel pessoa);
    Task Delete(Guid id);
}

public class PessoaService : IPessoaService
{
    private readonly IPessoaRepository _repository;

    public PessoaService(IPessoaRepository repository) =>
        _repository = repository;

    public async Task<PessoaEditarViewModel?> Find(Guid id)
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

public interface IPessoaRepository
{
    Task<Pessoa?> Find(Guid id);
    Task<Tuple<int, IEnumerable>> Search(Filter filtro);
    Task Create(Pessoa pessoa);
    Task Edit(Pessoa? pessoa);
    Task Delete(Guid id);
}

public class PessoaRepository : IPessoaRepository
{
    private readonly CursoInicianteContexto _contexto;

    public PessoaRepository(CursoInicianteContexto contexto) =>
        _contexto = contexto;

    public async Task<Pessoa?> Find(Guid id) =>
        await _contexto.Pessoa.FindAsync(id);

    public async Task<Tuple<int, IEnumerable>> Search(Filter filtro)
    {
        filtro.PreparePagination();

        var consulta = _contexto
            .Pessoa
            .Include(x => x.Tarefas)
            .AsQueryable()
            .AsNoTracking();

        if (!string.IsNullOrWhiteSpace(filtro.Search))
            consulta = consulta.Where(x =>
                x.Nome.Contains(filtro.Search)
                || x.Email.Contains(filtro.Search)
                || x.Celular.Contains(filtro.Search));

        var total = await consulta.CountAsync();

        consulta = filtro.Sort switch
        {
            "Nome" when filtro.Order == "desc" => consulta.OrderByDescending(x => x.Nome),
            "Email" when filtro.Order == "asc" => consulta.OrderBy(x => x.Email),
            "Email" when filtro.Order == "desc" => consulta.OrderByDescending(x => x.Email),
            "Celular" when filtro.Order == "asc" => consulta.OrderBy(x => x.Celular),
            "Celular" when filtro.Order == "desc" => consulta.OrderByDescending(x => x.Celular),
            _ => consulta.OrderBy(x => x.Nome)
        };

        var rows = await consulta
            .Select(x => new
            {
                x.Id, x.Nome, x.Email, x.Celular,
                qntTarefas = x.Tarefas.Count,
                qntSubtarefas = x.Tarefas.SelectMany(x => x.Subtarefas).Count()
            })
            .Skip(filtro.Offset.GetValueOrDefault())
            .Take(filtro.Limit.GetValueOrDefault())
            .ToListAsync();

        return new Tuple<int, IEnumerable>(total, rows);
    }

    public async Task Create(Pessoa pessoa)
    {
        _contexto.Add(pessoa);
        await _contexto.SaveChangesAsync();
    }

    public async Task Edit(Pessoa? pessoa)
    {
        _contexto.Update(pessoa);
        await _contexto.SaveChangesAsync();
    }

    public async Task Delete(Guid id)
    {
        var pessoa = await _contexto.Pessoa.FindAsync(id);
        if (pessoa != null) _contexto.Pessoa.Remove(pessoa);
        await _contexto.SaveChangesAsync();
    }
}