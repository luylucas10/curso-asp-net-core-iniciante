using System.Collections;
using CursoInicianteMvc.Data;
using CursoInicianteMvc.Models;
using Microsoft.EntityFrameworkCore;

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

public class TarefaService : ITarefaService
{
    private readonly ITarefaRepository _repository;
    private readonly IPessoaRepository _pessoaRepository;

    public TarefaService(ITarefaRepository repository, IPessoaRepository pessoaRepository)
    {
        _repository = repository;
        _pessoaRepository = pessoaRepository;
    }

    public async Task<TarefaCadastrarViewModel?> FindCreate(Guid pessoaId)
    {
        var pessoa = await _pessoaRepository.Find(pessoaId);
        return new TarefaCadastrarViewModel(pessoa);
    }
    public async Task<TarefaDetalharViewModel?> FindDetails(Guid id)
    {
        var tarefa = await _repository.Find(id);
        return tarefa != null ? new TarefaDetalharViewModel(tarefa) : null;
    }
    
    public async Task<TarefaEditarViewModel?> FindEdit(Guid id)
    {
        var tarefa = await _repository.Find(id);
        return tarefa != null ? new TarefaEditarViewModel(tarefa) : null;
    }

    public Task<Tuple<int, IEnumerable>> Search(TarefaFilter filtro) =>
        _repository.Search(filtro);

    public async Task<Guid> Create(TarefaCadastrarViewModel tarefa)
    {
        var tarefaEntidade = new Tarefa
        {
            Id = Guid.NewGuid(),
            PessoaId = tarefa.PessoaId,
            Descricao = tarefa.Descricao
        };

        await _repository.Create(tarefaEntidade);

        return tarefaEntidade.Id;
    }

    public async Task Edit(TarefaEditarViewModel tarefa)
    {
        var entidade = await _repository.Find(tarefa.Id);
        entidade.Descricao = tarefa.Descricao;
        await _repository.Edit(entidade);
    }

    public async Task Delete(Guid id) =>
        await _repository.Delete(id);

    public async Task Finish(Guid id)
    {
        var entidade = await _repository.Find(id);
        if (entidade == null)
            return;
        
        foreach (var sub in entidade.Subtarefas)
            sub.RealizadoEm = DateTime.Now;

        entidade.RealizadoEm = DateTime.Now;

        await _repository.Edit(entidade);
    }
}

public interface ITarefaRepository
{
    Task<Tarefa?> Find(Guid id);
    Task<Tuple<int, IEnumerable>> Search(TarefaFilter filtro);
    Task Create(Tarefa tarefa);
    Task Edit(Tarefa tarefa);
    Task Delete(Guid id);
}

public class TarefaRepository : ITarefaRepository
{
    private readonly CursoInicianteContexto _contexto;

    public TarefaRepository(CursoInicianteContexto contexto) =>
        _contexto = contexto;

    public async Task<Tarefa?> Find(Guid id) =>
        await _contexto
            .Tarefa
            .Include(x => x.Subtarefas)
            .Include(x => x.Pessoa)
            .Where(x => x.Id == id)
            .FirstOrDefaultAsync();

    public async Task<Tuple<int, IEnumerable>> Search(TarefaFilter filtro)
    {
        filtro.PreparePagination();

        var consulta = _contexto
            .Tarefa
            .Include(x => x.Subtarefas)
            .AsQueryable()
            .AsNoTracking()
            .Where(x => x.PessoaId == filtro.PessoaId);

        if (!string.IsNullOrWhiteSpace(filtro.Search))
            consulta = consulta.Where(x => x.Descricao.Contains(filtro.Search));

        var total = await consulta.CountAsync();

        consulta = filtro.Sort switch
        {
            "RealizadoEm" when filtro.Order == "asc" => consulta.OrderBy(x => x.RealizadoEm),
            "RealizadoEm" when filtro.Order == "desc" => consulta.OrderByDescending(x => x.RealizadoEm),
            "Descricao" when filtro.Order == "desc" => consulta.OrderByDescending(x => x.Descricao),
            _ => consulta.OrderBy(x => x.RealizadoEm)
        };

        var rows = await consulta
            .Select(x => new
            {
                x.Id, x.Descricao, x.RealizadoEm,
                qntSubtarefas = x.Subtarefas.Count,
                qntSubtarefasConcluidas = x.Subtarefas.Count(a => a.RealizadoEm.HasValue)
            })
            .Skip(filtro.Offset.GetValueOrDefault())
            .Take(filtro.Limit.GetValueOrDefault())
            .ToListAsync();

        return new Tuple<int, IEnumerable>(total, rows);
    }

    public async Task Create(Tarefa tarefa)
    {
        _contexto.Add(tarefa);
        await _contexto.SaveChangesAsync();
    }

    public async Task Edit(Tarefa tarefa)
    {
        _contexto.Update(tarefa);
        await _contexto.SaveChangesAsync();
    }

    public async Task Delete(Guid id)
    {
        var tarefa = await _contexto.Tarefa.FindAsync(id);
        if (tarefa != null) _contexto.Tarefa.Remove(tarefa);
        await _contexto.SaveChangesAsync();
    }
}