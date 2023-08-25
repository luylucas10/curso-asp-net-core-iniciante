namespace CursoInicianteMvc.Models;

public record ErrorViewModel
{
    public string? RequestId { get; init; }

    public bool ShowRequestId => !string.IsNullOrEmpty(RequestId);
}

public record TarefaViewModel
{
    public Guid PessoaId { get; init; }
    public required string Descricao { get; init; }
    public DateTime? RealizadoEm { get; init; }
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