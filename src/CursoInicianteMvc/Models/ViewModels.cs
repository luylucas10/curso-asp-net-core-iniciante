namespace CursoInicianteMvc.Models;

public record ErrorViewModel
{
    public string? RequestId { get; set; }

    public bool ShowRequestId => !string.IsNullOrEmpty(RequestId);
}

public record TarefaViewModel
{
    public Guid PessoaId { get; set; }
    public required string Descricao { get; set; }
    public DateTime? RealizadoEm { get; set; }
}

public record TarefaEditarViewModel
{
    public Guid Id { get; set; }
    public Guid PessoaId { get; set; }
    public required string Descricao { get; set; }
    public DateTime? RealizadoEm { get; set; }
}

public record InicioViewModel
{
    public int QuantidadePessoas { get; set; }
    public int QuantidadeTarafas { get; set; }
}