namespace CursoInicianteMvc.Models;

public class Pessoa
{
    public Pessoa()
    {
        Tarefas = new List<Tarefa>();
    }

    public Guid Id { get; set; }
    public string Nome { get; set; }
    public string Email { get; set; }
    public string? Celular { get; set; }

    public ICollection<Tarefa> Tarefas { get; set; }
}

public class Tarefa
{
    public Tarefa()
    {
        Subtarefas = new List<Subtarefa>();
    }

    public Guid Id { get; set; }
    public Guid PessoaId { get; set; }
    public string Descricao { get; set; }
    public DateTime? RealizadoEm { get; set; }
    public Pessoa Pessoa { get; set; }

    public ICollection<Subtarefa> Subtarefas { get; set; }
}

public class Subtarefa
{
    public Guid Id { get; set; }
    public Guid TarefaId { get; set; }
    public string Descricao { get; set; }
    public DateTime? RealizadoEm { get; set; }
    public Tarefa Tarefa { get; set; }
}