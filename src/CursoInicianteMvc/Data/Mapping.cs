using CursoInicianteMvc.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CursoInicianteMvc.Data;

public class PessoaConfiguracao : IEntityTypeConfiguration<Pessoa>
{
    public void Configure(EntityTypeBuilder<Pessoa> builder)
    {
        builder.HasKey(x => x.Id);

        builder.Property(x => x.Id).HasDefaultValueSql("newid()");
        builder.Property(x => x.Nome).HasMaxLength(100);
        builder.Property(x => x.Email).HasMaxLength(100);
        builder.Property(x => x.Celular).HasMaxLength(11);

        builder.HasMany(x => x.Tarefas)
            .WithOne(x => x.Pessoa)
            .HasPrincipalKey(x => x.Id)
            .HasForeignKey(x => x.PessoaId);
    }
}

public class TarefaConfiguracao : IEntityTypeConfiguration<Tarefa>
{
    public void Configure(EntityTypeBuilder<Tarefa> builder)
    {
        builder.HasKey(x => x.Id);

        builder.Property(x => x.Id).HasDefaultValueSql("newid()");

        builder.Property(x => x.Descricao).HasMaxLength(200);

        builder.HasOne(x => x.Pessoa)
            .WithMany(x => x.Tarefas)
            .HasPrincipalKey(x => x.Id)
            .HasForeignKey(x => x.PessoaId);

        builder
            .HasMany(x => x.Subtarefas)
            .WithOne(x => x.Tarefa)
            .HasPrincipalKey(x => x.Id)
            .HasForeignKey(x => x.TarefaId);
    }
}

public class SubtarefaConfiguracao : IEntityTypeConfiguration<Subtarefa>
{
    public void Configure(EntityTypeBuilder<Subtarefa> builder)
    {
        builder.HasKey(x => x.Id);

        builder.Property(x => x.Id).HasDefaultValueSql("newid()");
        builder.Property(x => x.Descricao).HasMaxLength(200);

        builder.HasOne(x => x.Tarefa)
            .WithMany(x => x.Subtarefas)
            .HasPrincipalKey(x => x.Id)
            .HasForeignKey(x => x.TarefaId);
    }
}