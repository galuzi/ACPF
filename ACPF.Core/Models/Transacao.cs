using System;
using System.ComponentModel.DataAnnotations;
using ACPF.Core.Enums;

namespace ACPF.Core.Models
{
    /// <summary>
    /// Representa uma transação financeira
    /// </summary>
    public class Transacao
    {
        /// <summary>
        /// Identificador único da transação
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// Descrição da transação
        /// </summary>
        [Required(ErrorMessage = "A descrição da transação é obrigatória")]
        [StringLength(200, ErrorMessage = "A descrição deve ter no máximo 200 caracteres")]
        public string Descricao { get; set; } = string.Empty;

        /// <summary>
        /// Valor da transação
        /// </summary>
        [Required(ErrorMessage = "O valor da transação é obrigatório")]
        [Range(0.01, double.MaxValue, ErrorMessage = "O valor deve ser maior que zero")]
        public decimal Valor { get; set; }

        /// <summary>
        /// Data da transação
        /// </summary>
        [Required(ErrorMessage = "A data da transação é obrigatória")]
        public DateTime Data { get; set; } = DateTime.Now;

        /// <summary>
        /// Tipo da transação (Receita ou Despesa)
        /// </summary>
        [Required(ErrorMessage = "O tipo da transação é obrigatório")]
        public TipoTransacao Tipo { get; set; }

        /// <summary>
        /// Categoria da transação
        /// </summary>
        [Required(ErrorMessage = "A categoria da transação é obrigatória")]
        public int CategoriaId { get; set; }

        /// <summary>
        /// Propriedade de navegação para a categoria
        /// </summary>
        public virtual Categoria Categoria { get; set; } = null!;

        /// <summary>
        /// Observações adicionais sobre a transação
        /// </summary>
        [StringLength(1000, ErrorMessage = "As observações devem ter no máximo 1000 caracteres")]
        public string? Observacoes { get; set; }

        /// <summary>
        /// Data de criação do registro
        /// </summary>
        public DateTime DataCriacao { get; set; } = DateTime.Now;

        /// <summary>
        /// Data da última modificação
        /// </summary>
        public DateTime DataModificacao { get; set; } = DateTime.Now;

        /// <summary>
        /// Retorna uma representação em string da transação
        /// </summary>
        public override string ToString()
        {
            return $"{Data:dd/MM/yyyy} - {Descricao} - {Valor:C}";
        }
    }
} 