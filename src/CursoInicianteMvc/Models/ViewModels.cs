using System.ComponentModel.DataAnnotations;

namespace CursoInicianteMvc.Models;

public class InicioViewModel
{
    public int QuantidadePessoas { get; set; }
    public int QuantidadeTarafas { get; set; }
}
public class ErrorViewModel
{
    public string? RequestId { get; set; }

    public bool ShowRequestId => !string.IsNullOrEmpty(RequestId);
}

public class PessoaCadastrarViewModel
{
    [Required, MinLength(3), MaxLength(100)]
    public string Nome { get; set; }

    [Required, MinLength(3), MaxLength(100), EmailAddress, Display(Name = "e-Mail")]
    public string Email { get; set; }

    [MinLength(9), MaxLength(14), Display(Name = "Celular")]
    public string? Celular { get; set; }
}

public class PessoaEditarViewModel : PessoaCadastrarViewModel
{
    public PessoaEditarViewModel()
    {
    }

    public PessoaEditarViewModel(Pessoa pessoa)
    {
        Id = pessoa.Id;
        Nome = pessoa.Nome;
        Email = pessoa.Email;
        Celular = pessoa.Celular;
    }

    public Guid Id { get; set; }
}

public class TarefaCadastrarViewModel
{
    public TarefaCadastrarViewModel()
    {
    }

    public TarefaCadastrarViewModel(Pessoa pessoa)
    {
        PessoaId = pessoa.Id;
        PessoaNome = pessoa.Nome;
    }

    [Required, Display(Name = "Pessoa")] public Guid PessoaId { get; set; }

    [Required, Display(Name = "Descrição"), MinLength(5), MaxLength(100)]
    public string Descricao { get; set; } = null!;

    public string? PessoaNome { get; set; }
}

public class TarefaEditarViewModel : TarefaCadastrarViewModel
{
    public TarefaEditarViewModel()
    {
    }

    public TarefaEditarViewModel(Tarefa tarefa)
    {
        Id = tarefa.Id;
        Descricao = tarefa.Descricao;
        PessoaId = tarefa.PessoaId;
        PessoaNome = tarefa.Pessoa.Nome;
    }

    public Guid Id { get; set; }
}

public class TarefaDetalharViewModel : TarefaEditarViewModel
{
    public TarefaDetalharViewModel()
    {
    }

    public TarefaDetalharViewModel(Tarefa tarefa) : base(tarefa)
    {
        RealizadoEm = tarefa.RealizadoEm;
    }

    public DateTime? RealizadoEm { get; set; }
}

public class SubtarefaCadastrarViewModel
{
    public SubtarefaCadastrarViewModel()
    {
    }

    public SubtarefaCadastrarViewModel(Tarefa tarefa)
    {
        TarefaId = tarefa.Id;
        TarefaDescricao = tarefa.Descricao;
    }

    [Display(Name = "Tarefa")] 
    public Guid TarefaId { get; set; }

    [Required, Display(Name = "Descrição")]
    public string Descricao { get; set; }
    public string? TarefaDescricao { get; set; }
}

public class SubtarefaEditarViewModel : SubtarefaCadastrarViewModel
{
    public SubtarefaEditarViewModel()
    {
    }
    public SubtarefaEditarViewModel(Subtarefa subtarefa) : base(subtarefa.Tarefa)
    {
        Id = subtarefa.Id;
        Descricao = subtarefa.Descricao;
    }
    
    public Guid Id { get; set; }
}

public class SubtarefaDetalharViewModel : SubtarefaEditarViewModel
{
    public SubtarefaDetalharViewModel()
    {
    }

    public SubtarefaDetalharViewModel(Subtarefa subtarefa): base(subtarefa)
    {
        RealizadoEm = subtarefa.RealizadoEm;
    }
    
    public DateTime? RealizadoEm { get; set; }
}

