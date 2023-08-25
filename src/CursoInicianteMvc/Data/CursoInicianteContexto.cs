using System.Reflection;
using CursoInicianteMvc.Models;
using Microsoft.EntityFrameworkCore;

namespace CursoInicianteMvc.Data;

public class CursoInicianteContexto : DbContext
{
    public CursoInicianteContexto(DbContextOptions<CursoInicianteContexto> options) : base(options)
    {
    }

    public DbSet<Pessoa> Pessoa { get; set; }
    public DbSet<Tarefa> Tarefa { get; set; }
    public DbSet<Subtarefa> Subtarefa { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());
    }
}