namespace CursoInicianteMvc.Models;

public class Pessoa
{
    public Guid Id { get; set; }
    public required string Nome { get; set; }
    public required string Email { get; set; }
    public string? Celular { get; set; }
}