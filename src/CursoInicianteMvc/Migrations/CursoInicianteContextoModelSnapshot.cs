﻿// <auto-generated />
using System;
using CursoInicianteMvc.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

#nullable disable

namespace CursoInicianteMvc.Migrations
{
    [DbContext(typeof(CursoInicianteContexto))]
    partial class CursoInicianteContextoModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "7.0.10")
                .HasAnnotation("Relational:MaxIdentifierLength", 128);

            SqlServerModelBuilderExtensions.UseIdentityColumns(modelBuilder);

            modelBuilder.Entity("CursoInicianteMvc.Models.Pessoa", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uniqueidentifier")
                        .HasDefaultValueSql("newid()");

                    b.Property<string>("Celular")
                        .HasMaxLength(11)
                        .HasColumnType("nvarchar(11)");

                    b.Property<string>("Email")
                        .IsRequired()
                        .HasMaxLength(100)
                        .HasColumnType("nvarchar(100)");

                    b.Property<string>("Nome")
                        .IsRequired()
                        .HasMaxLength(100)
                        .HasColumnType("nvarchar(100)");

                    b.HasKey("Id");

                    b.ToTable("Pessoa");
                });

            modelBuilder.Entity("CursoInicianteMvc.Models.Subtarefa", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uniqueidentifier")
                        .HasDefaultValueSql("newid()");

                    b.Property<string>("Descricao")
                        .IsRequired()
                        .HasMaxLength(200)
                        .HasColumnType("nvarchar(200)");

                    b.Property<DateTime?>("RealizadoEm")
                        .HasColumnType("datetime2");

                    b.Property<Guid>("TarefaId")
                        .HasColumnType("uniqueidentifier");

                    b.HasKey("Id");

                    b.HasIndex("TarefaId");

                    b.ToTable("Subtarefa");
                });

            modelBuilder.Entity("CursoInicianteMvc.Models.Tarefa", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uniqueidentifier")
                        .HasDefaultValueSql("newid()");

                    b.Property<string>("Descricao")
                        .IsRequired()
                        .HasMaxLength(200)
                        .HasColumnType("nvarchar(200)");

                    b.Property<Guid>("PessoaId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<DateTime?>("RealizadoEm")
                        .HasColumnType("datetime2");

                    b.HasKey("Id");

                    b.HasIndex("PessoaId");

                    b.ToTable("Tarefa");
                });

            modelBuilder.Entity("CursoInicianteMvc.Models.Subtarefa", b =>
                {
                    b.HasOne("CursoInicianteMvc.Models.Tarefa", "Tarefa")
                        .WithMany("Subtarefas")
                        .HasForeignKey("TarefaId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Tarefa");
                });

            modelBuilder.Entity("CursoInicianteMvc.Models.Tarefa", b =>
                {
                    b.HasOne("CursoInicianteMvc.Models.Pessoa", "Pessoa")
                        .WithMany("Tarefas")
                        .HasForeignKey("PessoaId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Pessoa");
                });

            modelBuilder.Entity("CursoInicianteMvc.Models.Pessoa", b =>
                {
                    b.Navigation("Tarefas");
                });

            modelBuilder.Entity("CursoInicianteMvc.Models.Tarefa", b =>
                {
                    b.Navigation("Subtarefas");
                });
#pragma warning restore 612, 618
        }
    }
}
