using CursoInicianteMvc.Data.Mapping;
using CursoInicianteMvc.Models;
using Microsoft.EntityFrameworkCore;

namespace CursoInicianteMvc.Data;

public class CursoInicianteContexto : DbContext
{
    public CursoInicianteContexto(DbContextOptions<CursoInicianteContexto> options) : base(options)
    {
    }

    public DbSet<Pessoa> Pessoa { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        modelBuilder.ApplyConfiguration(new PessoaConfiguracao());
    }
}