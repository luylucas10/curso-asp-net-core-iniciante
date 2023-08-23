namespace CursoInicianteMvc.Models;

public class Pessoa
{
    public Guid Id { get; set; }
    public required string Nome { get; set; }
    public required string Email { get; set; }
    public string? Celular { get; set; }

    public ICollection<Tarefa> Tarefas { get; set; }
}

public class Tarefa
{
    public Guid Id { get; set; }
    public Guid PessoaId { get; set; }
    public required string Descricao { get; set; }
    
    public DateTime? RealizadoEm { get; set; }

    public Pessoa Pessoa { get; set; }
}