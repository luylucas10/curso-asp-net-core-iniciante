using System.ComponentModel.DataAnnotations;

namespace CursoInicianteMvc.Models;

public record ErrorViewModel
{
    public string? RequestId { get; init; }

    public bool ShowRequestId => !string.IsNullOrEmpty(RequestId);
}

public record TarefaCadastrarViewModel
{
    [Required, Display(Name = "Pessoa")]
    public Guid PessoaId { get; init; }
    
    
    [Required, Display(Name = "Descrição")]
    public string Descricao { get; init; }
}

public record TarefaEditarViewModel
{
    public Guid Id { get; init; }
    public Guid PessoaId { get; init; }
    public required string Descricao { get; init; }
    public DateTime? RealizadoEm { get; init; }
}

public record SubtarefaCadastrarViewModel
{
    public Guid TarefaId { get; init; }
    public string Descricao { get; init; }
    public string RealizadoEm { get; init; }
}

public record InicioViewModel
{
    public int QuantidadePessoas { get; init; }
    public int QuantidadeTarafas { get; init; }
}

public record PessoaCadastroViewModel(
    [Required, MinLength(3), MaxLength(100)] string Nome, 
    [Required, MinLength(3), MaxLength(100), EmailAddress, Display(Name = "e-Mail")] string Email, 
    [MinLength(9), MaxLength(14), Display(Name = "Celular")] string? Celular);
    
public record PessoaEditarViewModel(
    Guid Id,
    [Required, MinLength(3), MaxLength(100)] string Nome, 
    [Required, MinLength(3), MaxLength(100), EmailAddress, Display(Name = "e-Mail")] string Email, 
    [MinLength(9), MaxLength(14), Display(Name = "Celular")] string? Celular);