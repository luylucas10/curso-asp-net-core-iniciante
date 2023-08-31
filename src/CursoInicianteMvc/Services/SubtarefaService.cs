using System;
using System.Collections;
using System.Threading.Tasks;
using CursoInicianteMvc.Data;
using CursoInicianteMvc.Models;

namespace CursoInicianteMvc.Services;

public class SubtarefaService : ISubtarefaService
{
    private readonly ISubtarefaRepository _repository;
    private readonly ITarefaRepository _tarefaRepository;

    public SubtarefaService(ISubtarefaRepository repository, ITarefaRepository tarefaRepository)
    {
        _repository = repository;
        _tarefaRepository = tarefaRepository;
    }

    public async Task<SubtarefaCadastrarViewModel> FindCreate(Guid tarefaId)
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