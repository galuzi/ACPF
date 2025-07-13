using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using ACPF.Core.Enums;

namespace ACPF.Core.Models
{
    /// <summary>
    /// Representa uma categoria de transação financeira
    /// </summary>
    public class Categoria
    {
        /// <summary>
        /// Identificador único da categoria
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// Nome da categoria (ex: Alimentação, Transporte, Salário)
        /// </summary>
        [Required(ErrorMessage = "O nome da categoria é obrigatório")]
        [StringLength(100, ErrorMessage = "O nome da categoria deve ter no máximo 100 caracteres")]
        public string Nome { get; set; } = string.Empty;

        /// <summary>
        /// Descrição opcional da categoria
        /// </summary>
        [StringLength(500, ErrorMessage = "A descrição deve ter no máximo 500 caracteres")]
        public string? Descricao { get; set; }

        /// <summary>
        /// Tipo da categoria (Receita ou Despesa)
        /// </summary>
        [Required(ErrorMessage = "O tipo da categoria é obrigatório")]
        public TipoTransacao Tipo { get; set; }

        /// <summary>
        /// Cor da categoria para identificação visual
        /// </summary>
        [StringLength(7, ErrorMessage = "A cor deve estar no formato #RRGGBB")]
        public string? Cor { get; set; }

        /// <summary>
        /// Data de criação da categoria
        /// </summary>
        public DateTime DataCriacao { get; set; } = DateTime.Now;

        /// <summary>
        /// Indica se a categoria está ativa
        /// </summary>
        public bool Ativa { get; set; } = true;

        // Propriedade de navegação para as transações
        public virtual ICollection<Transacao> Transacoes { get; set; } = new List<Transacao>();

        /// <summary>
        /// Retorna o nome da categoria para exibição
        /// </summary>
        public override string ToString()
        {
            return Nome;
        }
    }
} 