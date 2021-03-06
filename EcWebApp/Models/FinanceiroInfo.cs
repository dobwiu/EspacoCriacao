using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace EcWebApp.Models
{
    [Table("PlanoContas", Schema = "fin")]
    public class PlanoContaInfo
    {
        [Key]
        public Guid? IdConta { get; set; }

        [StringLength(50), Required]
        public string Descricao { get; set; }

        [Display(Name = "Data de Abertura"), DataType(DataType.DateTime)]
        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}", ApplyFormatInEditMode = true)]
        public DateTime DataAberturaConta { get; set; }

        [Display(Name = "Saldo Inicial"), DataType(DataType.Currency)]
        public Decimal? SaldoInicial { get; set; }

        [Display(Name = "Saldo Anterior"), DataType(DataType.Currency)]
        public Decimal? SaldoAnterior { get; set; }

        [Display(Name = "Saldo Atual"), DataType(DataType.Currency)]
        public Decimal? SaldoAtual { get; set; }

        [Display(Name = "Última Atualização") DataType(DataType.DateTime)]
        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}", ApplyFormatInEditMode = true)]
        public DateTime DataUltimaAtualizacao { get; set; }

        public bool Ativo { get; set; }

        public Guid IdUsuario { get; set; }

        [ForeignKey("IdUsuario")]
        public UsuarioInfo Usuario { get; set; }
    }

    [Table("Lancamentos", Schema = "fin")]
    public class LancamentoInfo
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int IdLancamento { get; set; }

        [Required]
        public Guid IdConta { get; set; }

        [Required] /* Credito ou Debito */
        public EnumTipoLancamento TipoLancamento { get; set; }

        [Display(Name = "Categoria"), Required]
        public int IdCategoria { get; set; }

        [Display(Name = "Descrição do Lançamento")]
        [StringLength(100), Required]
        public string Descricao { get; set; }

        [DataType(DataType.Currency), Required]
        public Decimal Valor { get; set; }

        [DataType(DataType.DateTime), Required]
        public DateTime DataProcessamento { get; set; }

        [Display(Name = "Comentário")]
        [StringLength(100), DataType(DataType.MultilineText)]
        public string Comentario { get; set; }

        public bool Processado { get; set; }

        [Display(Name = "Lançado Por")]
        public Guid IdUsuario { get; set; }

        [Display(Name = "Lançado Em")]
        [DataType(DataType.DateTime), Required]
        public DateTime DataLancamento { get; set; }

        [NotMapped]
        public EnumTipoRepeticao Repeticao { get; set; }

        [NotMapped]
        public int? Vezes { get; set; }

        #region Propriedades de Navegação..
        [ForeignKey("IdConta")]
        public PlanoContaInfo Conta { get; set; }

        [ForeignKey("IdCategoria")]
        public CategoriaLancamentoInfo Categoria { get; set; }

        [ForeignKey("IdUsuario")]
        public UsuarioInfo Usuario { get; set; }
        #endregion

        [NotMapped]
        public string DataFormatada
        {
            get
            {
                return DataProcessamento.ToShortDateString();
            }
        }

        [NotMapped]
        public string ClasseCSS
        {
            get
            {
                return (TipoLancamento == EnumTipoLancamento.Credito ? "text-black" : "text-red");
            }
        }
    }

    [Table("CategoriasLancamento", Schema = "fin")]
    public class CategoriaLancamentoInfo
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int IdCategoria { get; set; }

        [Display(Name = "Descrição")]
        [StringLength(50), Required]
        public string Descricao { get; set; }

        [Display(Name = "Tipo de Lançamento")]
        public EnumTipoLancamento TipoLancamento { get; set; }

        public bool Ativo { get; set; }
    }

    [Table("Fechamentos", Schema = "fin")]
    public class FechamentoInfo
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int IdFechamento { get; set; }

        public Guid IdConta { get; set; }

        [DataType(DataType.DateTime), Required]
        public DateTime DataFechamento { get; set; }

        [DataType(DataType.Currency), Required]
        public decimal ValorFechamento { get; set; }

        [ForeignKey("IdConta")]
        public PlanoContaInfo Conta { get; set; }
    }
}