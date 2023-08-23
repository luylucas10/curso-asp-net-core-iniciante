using CursoInicianteMvc.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CursoInicianteMvc.Data.Mapping;

public class PessoaConfiguracao : IEntityTypeConfiguration<Pessoa> 
{
    public void Configure(EntityTypeBuilder<Pessoa> builder)
    {
        builder.HasKey(x => x.Id);

        builder.Property(x => x.Id).HasDefaultValueSql("newid()");
        builder.Property(x => x.Nome).HasMaxLength(100);
        builder.Property(x => x.Email).HasMaxLength(100);
        builder.Property(x => x.Celular).HasMaxLength(11);
    }
}