using System;
using System.Collections;
using System.Linq;
using System.Threading.Tasks;
using CursoInicianteMvc.Models;
using Microsoft.EntityFrameworkCore;

namespace CursoInicianteMvc.Data;

public class SubtarefaRepository : ISubtarefaRepository
{
    private readonly CursoInicianteContexto _contexto;

    public SubtarefaRepository(CursoInicianteContexto contexto) =>
        _contexto = contexto;

    public async Task<Subtarefa> Find(Guid id) =>
        await _contexto
            .Subtarefa
            .Include(x => x.Tarefa)
            .Where(x => x.Id == id)
            .FirstOrDefaultAsync();

    public async Task<Tuple<int, IEnumerable>> Search(SubtarefaFilter filtro)
    {
        filtro.PreparePagination();

        var consulta = _contexto
            .Subtarefa
            .AsQueryable()
            .AsNoTracking()
            .Where(x => x.TarefaId == filtro.TarefaId);

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
            })
            .Skip(filtro.Offset.GetValueOrDefault())
            .Take(filtro.Limit.GetValueOrDefault())
            .ToListAsync();

        return new Tuple<int, IEnumerable>(total, rows);
    }

    public async Task Create(Subtarefa subtarefa)
    {
        _contexto.Add(subtarefa);
        await _contexto.SaveChangesAsync();
    }

    public async Task Edit(Subtarefa subtarefa)
    {
        _contexto.Update(subtarefa);
        await _contexto.SaveChangesAsync();
    }

    public async Task Delete(Guid id)
    {
        var subtarefa = await _contexto.Subtarefa.FindAsync(id);
        if (subtarefa != null) _contexto.Subtarefa.Remove(subtarefa);
        await _contexto.SaveChangesAsync();
    }
}