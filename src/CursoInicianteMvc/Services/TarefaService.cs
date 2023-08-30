using System.Collections;
using CursoInicianteMvc.Data;
using CursoInicianteMvc.Models;

namespace CursoInicianteMvc.Services;

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