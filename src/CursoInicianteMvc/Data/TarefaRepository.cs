using System;
using System.Collections;
using System.Linq;
using System.Threading.Tasks;
using CursoInicianteMvc.Models;
using Microsoft.EntityFrameworkCore;

namespace CursoInicianteMvc.Data;

public class TarefaRepository : ITarefaRepository
{
    private readonly CursoInicianteContexto _contexto;

    public TarefaRepository(CursoInicianteContexto contexto) =>
        _contexto = contexto;

    public async Task<Tarefa> Find(Guid id) =>
        await _contexto
            .Tarefa
            .Include(x => x.Subtarefas)
            .Include(x => x.Pessoa)
            .Where(x => x.Id == id)
            .FirstOrDefaultAsync();

    public async Task<Tuple<int, IEnumerable>> Search(TarefaFilter filtro)
    {
        filtro.PreparePagination();

        var consulta = _contexto
            .Tarefa
            .Include(x => x.Subtarefas)
            .AsQueryable()
            .AsNoTracking()
            .Where(x => x.PessoaId == filtro.PessoaId);

        if (!string.IsNullOrWhiteSpace(filtro.Search))
            consulta = consulta.Where(x => x.Descricao.Contains(filtro.Search));

        var total = await consulta.CountAsync();

        consulta = filtro.Sort switch
        {
            "RealizadoEm" when filtro.Order == "asc" => consulta.OrderBy(x => x.RealizadoEm),
            "RealizadoEm" when filtro.Order == "desc" => consulta.OrderByDescending(x => x.RealizadoEm),
            "Descricao" when filtro.Order == "desc" => consulta.OrderByDescending(x => x.Descricao),
            _ => consulta.OrderBy(x => x.RealizadoEm)
        };

        var rows = await consulta
            .Select(x => new
            {
                x.Id, x.Descricao, x.RealizadoEm,
                qntSubtarefas = x.Subtarefas.Count,
                qntSubtarefasConcluidas = x.Subtarefas.Count(a => a.RealizadoEm.HasValue)
            })
            .Skip(filtro.Offset.GetValueOrDefault())
            .Take(filtro.Limit.GetValueOrDefault())
            .ToListAsync();

        return new Tuple<int, IEnumerable>(total, rows);
    }

    public async Task Create(Tarefa tarefa)
    {
        _contexto.Add(tarefa);
        await _contexto.SaveChangesAsync();
    }

    public async Task Edit(Tarefa tarefa)
    {
        _contexto.Update(tarefa);
        await _contexto.SaveChangesAsync();
    }

    public async Task Delete(Guid id)
    {
        var tarefa = await _contexto.Tarefa.FindAsync(id);
        if (tarefa != null) _contexto.Tarefa.Remove(tarefa);
        await _contexto.SaveChangesAsync();
    }
}