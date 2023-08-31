using System;
using System.Collections;
using System.Linq;
using System.Threading.Tasks;
using CursoInicianteMvc.Models;
using Microsoft.EntityFrameworkCore;

namespace CursoInicianteMvc.Data;

public class PessoaRepository : IPessoaRepository
{
    private readonly CursoInicianteContexto _contexto;

    public PessoaRepository(CursoInicianteContexto contexto) =>
        _contexto = contexto;

    public async Task<Pessoa> Find(Guid id) =>
        await _contexto.Pessoa.FindAsync(id);

    public async Task<Tuple<int, IEnumerable>> Search(Filter filtro)
    {
        filtro.PreparePagination();

        var consulta = _contexto
            .Pessoa
            .Include(x => x.Tarefas)
            .AsQueryable()
            .AsNoTracking();

        if (!string.IsNullOrWhiteSpace(filtro.Search))
            consulta = consulta.Where(x =>
                x.Nome.Contains(filtro.Search)
                || x.Email.Contains(filtro.Search)
                || x.Celular.Contains(filtro.Search));

        var total = await consulta.CountAsync();

        consulta = filtro.Sort switch
        {
            "Nome" when filtro.Order == "desc" => consulta.OrderByDescending(x => x.Nome),
            "Email" when filtro.Order == "asc" => consulta.OrderBy(x => x.Email),
            "Email" when filtro.Order == "desc" => consulta.OrderByDescending(x => x.Email),
            "Celular" when filtro.Order == "asc" => consulta.OrderBy(x => x.Celular),
            "Celular" when filtro.Order == "desc" => consulta.OrderByDescending(x => x.Celular),
            _ => consulta.OrderBy(x => x.Nome)
        };

        var rows = await consulta
            .Select(x => new
            {
                x.Id, x.Nome, x.Email, x.Celular,
                qntTarefas = x.Tarefas.Count,
                qntSubtarefas = x.Tarefas.SelectMany(x => x.Subtarefas).Count()
            })
            .Skip(filtro.Offset.GetValueOrDefault())
            .Take(filtro.Limit.GetValueOrDefault())
            .ToListAsync();

        return new Tuple<int, IEnumerable>(total, rows);
    }

    public async Task Create(Pessoa pessoa)
    {
        _contexto.Add(pessoa);
        await _contexto.SaveChangesAsync();
    }

    public async Task Edit(Pessoa? pessoa)
    {
        _contexto.Update(pessoa);
        await _contexto.SaveChangesAsync();
    }

    public async Task Delete(Guid id)
    {
        var pessoa = await _contexto.Pessoa.FindAsync(id);
        if (pessoa != null) _contexto.Pessoa.Remove(pessoa);
        await _contexto.SaveChangesAsync();
    }
}