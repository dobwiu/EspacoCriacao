using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace EcWebApp.Models
{
    [Table("Pedidos", Schema = "orc")]
    public class PedidoInfo
    {
        [Key]
        public Guid? IdPedido { get; set; }

        [Display(Name = "Número do Pedido")]
        [StringLength(20), Required]
        public string NumeroPedido { get; set; }

        [Display(Name = "Cliente"), Required]
        public Guid IdCliente { get; set; }

        [Display(Name = "Vendedor"), Required]
        public Guid IdVendedor { get; set; }

        [Display(Name = "Data do Pedido"), DataType(DataType.DateTime), Required]
        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}", ApplyFormatInEditMode = true)]
        public DateTime DataPedido { get; set; }

        [Display(Name = "Versão")]
        public string VersaoProMob { get; set; }

        #region Status e Fase..
        [Display(Name = "Prazo de Entrega"), DataType(DataType.DateTime)]
        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}", ApplyFormatInEditMode = true)]
        public DateTime? PrazoEntrega { get; set; }

        [Display(Name = "Status do Pedido")]
        public EnumStatusPedido StatusPedido { get; set; }

        [Display(Name = "Fase do Pedido")]
        public EnumFasePedido? FasePedido { get; set; }

        [Display(Name = "Observações")]
        [StringLength(1000), DataType(DataType.MultilineText)]
        public string Observacoes { get; set; }

        [NotMapped]
        public double? DiasParaEntrega
        {
            get
            {
                if (PrazoEntrega.HasValue)
                    return (PrazoEntrega.Value - DateTime.Today).TotalDays;
                else
                    return null;
            }
        }
        #endregion

        #region Pagamento..
        [Display(Name = "Valor")]
        [DataType(DataType.Currency), Required]
        public Decimal ValorOrcamento { get; set; }

        [Display(Name = "Total a Prazo"), DataType(DataType.Currency)]
        public Decimal? ValorPrazo { get; set; }

        [Display(Name = "Entrada"), DataType(DataType.Currency)]
        public Decimal? ValorEntrada { get; set; }

        [Display(Name = "Forma de Pagamento")]
        public int? IdFormaPagamento { get; set; }

        [Display(Name = "Condição de Pagamento"), StringLength(50)]
        public string CondicaoPagamento { get; set; }

        [Display(Name = "Parcelas")]
        public int? NumeroParcelas { get; set; }
        #endregion

        #region Propriedades de Navegação..
        [ForeignKey("IdCliente")]
        public ClienteInfo Cliente { get; set; }

        [ForeignKey("IdVendedor")]
        public UsuarioInfo Vendedor { get; set; }

        [ForeignKey("IdFormaPagamento")]
        public FormaPagamentoInfo FormaPagamento { get; set; }

        //public ICollection<AmbienteInfo> Ambientes {get; set; }
        //public ICollection<ItemProMobInfo> Itens { get; set; }
        #endregion
    }

    [Table("PedidoAmbientes", Schema = "orc")]
    public class AmbienteInfo
    {
        [Key]
        public Guid? IdAmbiente { get; set; }

        [Required]
        public int Item { get; set; }

        public Decimal? Qtde { get; set; }

        [StringLength(100)]
        public string Descricao { get; set; }

        [StringLength(100)]
        public string Fornecedor { get; set; }

        [StringLength(100)]
        public string Linha { get; set; }

        public int? Prazo { get; set; }

        [DataType(DataType.Currency)]
        public Decimal? Valor { get; set; }

        [StringLength(100)]
        public string Corpo { get; set; }

        [StringLength(100)]
        public string Portas { get; set; }

        [StringLength(100)]
        public string Puxador { get; set; }

        [StringLength(100)]
        public string Complemento { get; set; }

        [StringLength(100)]
        public string Modelo { get; set; }

        [StringLength(50)] /* Tmaponamento - Espessura */
        public string Espessura { get; set; }

        //[StringLength(500)]
        //public string Observacao { get; set; }

        #region Propriedades de Navegação..
        [Required]
        public Guid IdPedido { get; set; }

        [ForeignKey("IdPedido")]
        public PedidoInfo Pedido { get; set; }
        #endregion
    }

    // [Table("PedidoObservacoes", Schema = "orc")]
    // public class PedidoObservacaoInfo
    // {
    //     [Key]
    //     public Guid? IdObservacao { get; set; }

    //     [Required]
    //     public int Item { get; set; }

    //     [StringLength(500)]
    //     public string Observacao { get; set; }

    //     #region Propriedades de Navegação..
    //     [Required]
    //     public Guid IdPedido { get; set; }

    //     [ForeignKey("IdPedido")]
    //     public PedidoInfo Pedido { get; set; }
    //     #endregion
    // }

    [Table("PedidoItensProMob", Schema = "orc")]
    public class ItemProMobInfo
    {
        [Key]
        public Guid? IdItem { get; set; }

        public int Item { get; set; }

        public Decimal Qtde { get; set; }

        [Display(Name = "Referência")]
        public string Referencia { get; set; }

        [Display(Name = "Descrição")]
        public string Descricao { get; set; }

        [Display(Name = "Dimensões")]
        public string Dimensoes { get; set; }

        [Display(Name = "Preço CT"), DataType(DataType.Currency)]
        public Decimal PrecoCT { get; set; }

        #region Propriedades de Navegação
        public Guid? IdAmbiente { get; set; }

        public Guid IdPedido { get; set; }

        [ForeignKey("IdPedido")]
        public PedidoInfo Pedido { get; set; }
        #endregion
    }

    [Table("Parcelas", Schema = "orc")]
    public class ParcelaInfo
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int IdParcela { get; set; }

        [Required]
        public Guid IdPedido { get; set; }

        [Required]
        public Guid IdCliente { get; set; }

        [Display(Name = "Parcela")]
        public int NumeroParcela { get; set; }

        [Display(Name = "Nro. do Cheque"), StringLength(25)]
        public string NumeroCheque { get; set; }

        [Display(Name = "Data para a Compensação"), DataType(DataType.DateTime)]
        public DateTime? DataPagamento { get; set; }

        [Display(Name = "Valor"), DataType(DataType.Currency)]
        public Decimal ValorParcela { get; set; }

        public bool Contabilizado { get; set; }

        [ForeignKey("IdPedido")]
        public virtual PedidoInfo Pedido { get; set; }

        [NotMapped]
        public string DataPagto
        {
            get
            {
                return (DataPagamento.HasValue) ? DataPagamento.Value.ToShortDateString() : string.Empty;
            }
        }
    }
}