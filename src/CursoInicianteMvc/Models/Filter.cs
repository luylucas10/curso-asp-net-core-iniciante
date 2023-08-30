namespace CursoInicianteMvc.Models;

public class Filter
{
    public int? Limit { get; set; }
    public int? Offset { get; set; }
    public string? Search { get; set; }
    public string? Sort { get; set; }
    public string? Order { get; set; }

    public void PreparePagination()
    {
        Limit = Limit.GetValueOrDefault(0) <= 0 ? 15 : Limit;
        Offset = (Offset.GetValueOrDefault(0) <= 0 ? 0 : Offset - 1) * Limit;
    }
}

public class TarefaFilter : Filter
{
    public Guid PessoaId { get; set; }
}

public class SubtarefaFilter : Filter
{
    public Guid TarefaId { get; set; }
}