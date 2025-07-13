using System;
using Microsoft.EntityFrameworkCore;
using ACPF.Core.Models;
using ACPF.Core.Enums;

namespace ACPF.DataAccess
{
    /// <summary>
    /// Contexto principal do Entity Framework Core para a aplicação ACPF
    /// </summary>
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
        }

        /// <summary>
        /// DbSet para as categorias
        /// </summary>
        public DbSet<Categoria> Categorias { get; set; }

        /// <summary>
        /// DbSet para as transações
        /// </summary>
        public DbSet<Transacao> Transacoes { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Configuração da entidade Categoria
            modelBuilder.Entity<Categoria>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Id).ValueGeneratedOnAdd();
                
                entity.Property(e => e.Nome)
                    .IsRequired()
                    .HasMaxLength(100);
                
                entity.Property(e => e.Descricao)
                    .HasMaxLength(500);
                
                entity.Property(e => e.Tipo)
                    .IsRequired()
                    .HasConversion<int>();
                
                entity.Property(e => e.Cor)
                    .HasMaxLength(7);
                
                entity.Property(e => e.DataCriacao)
                    .IsRequired();
                
                entity.Property(e => e.Ativa)
                    .IsRequired()
                    .HasDefaultValue(true);

                // Índices para melhor performance
                entity.HasIndex(e => e.Nome);
                entity.HasIndex(e => e.Tipo);
                entity.HasIndex(e => e.Ativa);
            });

            // Configuração da entidade Transacao
            modelBuilder.Entity<Transacao>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Id).ValueGeneratedOnAdd();
                
                entity.Property(e => e.Descricao)
                    .IsRequired()
                    .HasMaxLength(200);
                
                entity.Property(e => e.Valor)
                    .IsRequired()
                    .HasColumnType("DECIMAL(18,2)");
                
                entity.Property(e => e.Data)
                    .IsRequired();
                
                entity.Property(e => e.Tipo)
                    .IsRequired()
                    .HasConversion<int>();
                
                entity.Property(e => e.CategoriaId)
                    .IsRequired();
                
                entity.Property(e => e.Observacoes)
                    .HasMaxLength(1000);
                
                entity.Property(e => e.DataCriacao)
                    .IsRequired();
                
                entity.Property(e => e.DataModificacao)
                    .IsRequired();

                // Relacionamento com Categoria
                entity.HasOne(e => e.Categoria)
                    .WithMany(c => c.Transacoes)
                    .HasForeignKey(e => e.CategoriaId)
                    .OnDelete(DeleteBehavior.Restrict);

                // Índices para melhor performance
                entity.HasIndex(e => e.Data);
                entity.HasIndex(e => e.Tipo);
                entity.HasIndex(e => e.CategoriaId);
                entity.HasIndex(e => e.Valor);
            });

            // Dados iniciais para categorias
            SeedData(modelBuilder);
        }

        /// <summary>
        /// Popula o banco de dados com dados iniciais
        /// </summary>
        private void SeedData(ModelBuilder modelBuilder)
        {
            // Categorias de Receita
            modelBuilder.Entity<Categoria>().HasData(
                new Categoria
                {
                    Id = 1,
                    Nome = "Salário",
                    Descricao = "Rendimentos do trabalho principal",
                    Tipo = TipoTransacao.Receita,
                    Cor = "#28A745",
                    DataCriacao = DateTime.Now,
                    Ativa = true
                },
                new Categoria
                {
                    Id = 2,
                    Nome = "Freelance",
                    Descricao = "Trabalhos extras e projetos",
                    Tipo = TipoTransacao.Receita,
                    Cor = "#17A2B8",
                    DataCriacao = DateTime.Now,
                    Ativa = true
                },
                new Categoria
                {
                    Id = 3,
                    Nome = "Investimentos",
                    Descricao = "Rendimentos de aplicações financeiras",
                    Tipo = TipoTransacao.Receita,
                    Cor = "#FFC107",
                    DataCriacao = DateTime.Now,
                    Ativa = true
                }
            );

            // Categorias de Despesa
            modelBuilder.Entity<Categoria>().HasData(
                new Categoria
                {
                    Id = 4,
                    Nome = "Alimentação",
                    Descricao = "Gastos com comida e refeições",
                    Tipo = TipoTransacao.Despesa,
                    Cor = "#DC3545",
                    DataCriacao = DateTime.Now,
                    Ativa = true
                },
                new Categoria
                {
                    Id = 5,
                    Nome = "Transporte",
                    Descricao = "Combustível, transporte público, etc.",
                    Tipo = TipoTransacao.Despesa,
                    Cor = "#6F42C1",
                    DataCriacao = DateTime.Now,
                    Ativa = true
                },
                new Categoria
                {
                    Id = 6,
                    Nome = "Moradia",
                    Descricao = "Aluguel, contas de casa, etc.",
                    Tipo = TipoTransacao.Despesa,
                    Cor = "#FD7E14",
                    DataCriacao = DateTime.Now,
                    Ativa = true
                },
                new Categoria
                {
                    Id = 7,
                    Nome = "Lazer",
                    Descricao = "Entretenimento e diversão",
                    Tipo = TipoTransacao.Despesa,
                    Cor = "#E83E8C",
                    DataCriacao = DateTime.Now,
                    Ativa = true
                },
                new Categoria
                {
                    Id = 8,
                    Nome = "Saúde",
                    Descricao = "Consultas médicas, medicamentos, etc.",
                    Tipo = TipoTransacao.Despesa,
                    Cor = "#20C997",
                    DataCriacao = DateTime.Now,
                    Ativa = true
                }
            );
        }
    }
} 