using System.Collections;
using CursoInicianteMvc.Data;
using CursoInicianteMvc.Models;
using Microsoft.EntityFrameworkCore;

namespace CursoInicianteMvc.Services;

public interface ISubtarefaService
{
    Task<SubtarefaCadastrarViewModel?> FindCreate(Guid tarefaId);
    Task<SubtarefaEditarViewModel?> FindEdit(Guid id);
    Task<SubtarefaDetalharViewModel?> FindDetails(Guid id);
    Task<Tuple<int, IEnumerable>> Search(SubtarefaFilter filtro);
    Task<Guid> Create(SubtarefaCadastrarViewModel subtarefa);
    Task Edit(SubtarefaEditarViewModel tarefa);
    Task Delete(Guid id);
    Task Finish(Guid id);
}

public class SubtarefaService : ISubtarefaService
{
    private readonly ISubtarefaRepository _repository;
    private readonly ITarefaRepository _tarefaRepository;

    public SubtarefaService(ISubtarefaRepository repository, ITarefaRepository tarefaRepository)
    {
        _repository = repository;
        _tarefaRepository = tarefaRepository;
    }

    public async Task<SubtarefaCadastrarViewModel?> FindCreate(Guid tarefaId)
    {
        var tarefa = await _tarefaRepository.Find(tarefaId);
        return new SubtarefaCadastrarViewModel(tarefa!);
    }

    public async Task<SubtarefaDetalharViewModel?> FindDetails(Guid id)
    {
        var subtarefa = await _repository.Find(id);
        return subtarefa != null ? new SubtarefaDetalharViewModel(subtarefa) : null;
    }

    public async Task<SubtarefaEditarViewModel?> FindEdit(Guid id)
    {
        var subtarefa = await _repository.Find(id);
        return subtarefa != null ? new SubtarefaEditarViewModel(subtarefa) : null;
    }

    public Task<Tuple<int, IEnumerable>> Search(SubtarefaFilter filtro) =>
        _repository.Search(filtro);

    public async Task<Guid> Create(SubtarefaCadastrarViewModel subtarefa)
    {
        var subTarefaEntidade = new Subtarefa
        {
            Id = Guid.NewGuid(),
            TarefaId = subtarefa.TarefaId,
            Descricao = subtarefa.Descricao
        };

        await _repository.Create(subTarefaEntidade);

        return subTarefaEntidade.Id;
    }

    public async Task Edit(SubtarefaEditarViewModel tarefa)
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
        
        entidade.RealizadoEm = DateTime.Now;
        await _repository.Edit(entidade);
    }
}

public interface ISubtarefaRepository
{
    Task<Subtarefa?> Find(Guid id);
    Task<Tuple<int, IEnumerable>> Search(SubtarefaFilter filtro);
    Task Create(Subtarefa subtarefa);
    Task Edit(Subtarefa subtarefa);
    Task Delete(Guid id);
}

public class SubtarefaRepository : ISubtarefaRepository
{
    private readonly CursoInicianteContexto _contexto;

    public SubtarefaRepository(CursoInicianteContexto contexto) =>
        _contexto = contexto;

    public async Task<Subtarefa?> Find(Guid id) =>
        await _contexto
            .Subtarefa
            .Include(x => x.Tarefa)
            .Where(x => x.Id == id)
            .FirstOrDefaultAsync();

    public async Task<Tuple<int, IEnumerable>> Search(SubtarefaFilter filtro)
    {
        filtro.PreparePagination();

        var consulta = _contexto
            .Subtarefa
            .AsQueryable()
            .AsNoTracking()
            .Where(x => x.TarefaId == filtro.TarefaId);

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
            })
            .Skip(filtro.Offset.GetValueOrDefault())
            .Take(filtro.Limit.GetValueOrDefault())
            .ToListAsync();

        return new Tuple<int, IEnumerable>(total, rows);
    }

    public async Task Create(Subtarefa subtarefa)
    {
        _contexto.Add(subtarefa);
        await _contexto.SaveChangesAsync();
    }

    public async Task Edit(Subtarefa subtarefa)
    {
        _contexto.Update(subtarefa);
        await _contexto.SaveChangesAsync();
    }

    public async Task Delete(Guid id)
    {
        var subtarefa = await _contexto.Subtarefa.FindAsync(id);
        if (subtarefa != null) _contexto.Subtarefa.Remove(subtarefa);
        await _contexto.SaveChangesAsync();
    }
}