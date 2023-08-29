using System.ComponentModel.DataAnnotations;

namespace CursoInicianteMvc.Models;

public class ErrorViewModel
{
    public string? RequestId { get; set; }

    public bool ShowRequestId => !string.IsNullOrEmpty(RequestId);
}

public class PessoaCadastroViewModel
{
    [Required, MinLength(3), MaxLength(100)]
    public string Nome { get; set; }

    [Required, MinLength(3), MaxLength(100), EmailAddress, Display(Name = "e-Mail")]
    public string Email { get; set; }

    [MinLength(9), MaxLength(14), Display(Name = "Celular")]
    public string? Celular { get; set; }
}

public class PessoaEditarViewModel : PessoaCadastroViewModel
{
    public Guid Id { get; set; }
}

public class TarefaCadastrarViewModel
{
    [Required, Display(Name = "Pessoa")] 
    public Guid PessoaId { get; set; }

    [Required, Display(Name = "Descrição"), MinLength(5), MaxLength(100)]
    public string Descricao { get; set; }
}

public class TarefaEditarViewModel : TarefaCadastrarViewModel
{
    public Guid Id { get; set; }
}

public class SubtarefaCadastrarViewModel
{
    [Display(Name = "Tarefa")] 
    public Guid TarefaId { get; set; }

    [Display(Name = "Descrição"), Required]
    public string Descricao { get; set; }
}

public class SubtarefaEditarViewModel : SubtarefaCadastrarViewModel
{
    public Guid Id { get; set; }
}

public class InicioViewModel
{
    public int QuantidadePessoas { get; set; }
    public int QuantidadeTarafas { get; set; }
}