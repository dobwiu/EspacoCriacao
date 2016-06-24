using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace EcWebApp.Models
{
    [Table("Dicionario")]
    public class DicionarioInfo
    {
        [Key, Column(Order = 1)]
        public string Enumerador { get; set; }

        [Key, Column(Order = 2)]
        public int Valor { get; set; }

        [MaxLength(100)]
        public string Texto { get; set; }

        public bool Ativo { get; set; }
    }

    #region Tabelas Auxiliares..
    [Table("Metas")]
    public class MetaInfo
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int IdMeta { get; set; }

        [Index("UK_PeriodoMeta", IsUnique = true, Order = 1)]
        [Display(Name = "Mês"), Range(1, 12, ErrorMessage = "Mês Inválido")]
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:00}")]
        public int Mes { get; set; }

        [Index("UK_PeriodoMeta", IsUnique = true, Order = 2)]
        [Display(Name = "Ano"), Range(2015, 2099, ErrorMessage = "Ano Inválido")]
        public int Ano { get; set; }

        [Display(Name = "Meta"), DataType(DataType.Currency)]
        public Decimal? ValorMeta { get; set; }

        [Display(Name = "Desafio"), DataType(DataType.Currency)]
        public Decimal? ValorDesafio { get; set; }
    }

    [Table("Feriados")]
    public class FeriadoInfo
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int IdFeriado { get; set; }

        [Index("UK_DataFeriado", IsUnique = true)]
        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}", ApplyFormatInEditMode = true)]
        [Display(Name = "Data"), Required(ErrorMessage = "Informe a Data do Feriado")]
        public DateTime DataFeriado { get; set; }

        [Display(Name = "Descrição"), StringLength(50)]
        [Required(ErrorMessage = "Informe a Descrição do Feriado")]
        public string Descricao { get; set; }
    }

    [Table("FormasPagamento")]
    public class FormaPagamentoInfo
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int IdFormaPagto { get; set; }

        [Display(Name = "Descrição"), StringLength(50)]
        [Required(ErrorMessage = "Informe a Descrição da Forma de Pagamento")]
        public string Descricao { get; set; }

        public bool Ativo { get; set; }
    }

    [Table("StatusAtendimento")]
    public class StatusAtendimentoInfo
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int IdStatus { get; set; }

        [Display(Name = "Descrição")]
        [StringLength(50), Required]
        public string Descricao { get; set; }

        [Display(Name = "Tipo do Status")]
        public EnumTipoStatus TipoStatus { get; set; }

        public bool Ativo { get; set; }
    }

    #endregion

    #region Enumeradores..
    public enum EnumNivelAcesso : int
    {
        [Display(Name = "Sem Acesso")]
        SemAcesso = 0,
        [Display(Name = "Somente Consulta")]
        SomenteConsulta = 1,
        Operacional = 2,
        Administrador = 3
    }

    public enum EnumTipoStatus : int
    {
        [Display(Name = "Em Andamento")]
        EmAndamento = 0,
        [Display(Name = "Finalizador - Venda")]
        FinalizadoVenda = 1,
        [Display(Name = "Finalizador - Não Venda")]
        FinalizadoNaoVenda = 2
    }

    public enum EnumStatusPedido : int
    {
        Aberto = 0,
        Fechado = 1,
        Cancelado = 2
    }

    public enum EnumInteresse : int
    {
        [Display(Name = "Orçamento")]
        Orcamento = 1,
        [Display(Name = "Negociação")]
        Negociacao = 2,
        [Display(Name = "Fechamento")]
        Fechamento = 3,
        [Display(Name = "Sem Mais Interesse")]
        SemInteresse = 9
    }

    public enum EnumFasePedido : int
    {
        Fabricacao = 1,
        Entrega = 2,
        Montagem = 3,
        Assistencia = 4
    }

    public enum EnumStatusPlanta : int
    {
        Desenvolver,
        [Display(Name = "Em Desenvolvimento")]
        EmDesenvolvimento,
        [Display(Name = "Aguardando Aprovação")]
        AguardandoAprovacao,
        [Display(Name = "Em Revisão")]
        EmRevisao,
        Finalizada
    }

    public enum EnumTipoEndereco : int
    {
        Atual = 1,
        Entrega = 2
    }

    public enum EnumMelhorHorario : int
    {
        [Display(Name = "Qualquer Horário")]
        QualquerHorario = 0,

        [Display(Name = "Período da Manhã")]
        Manha = 1,

        [Display(Name = "Período da Tarde")]
        Tarde = 2,

        [Display(Name = "Período da Noite")]
        Noite = 3
    }

    public enum EnumProcedencia : int
    {
        Porta = 1,
        [Display(Name = "Captação")]
        Captacao = 2,
        ProCompra = 3,
        Internet = 4,
        [Display(Name = "Indicação")]
        Indicacao = 5,
        Outros = 9
    }

    public enum EnumCondicaoPagamento : int
    {
        [Display(Name = "A Vista")]
        Vista = 1,
        [Display(Name = "A Prazo")]
        Prazo = 2
    }

    public enum EnumTipoLancamento : int
    {
        [Display(Name = "Crédito")]
        Credito = 1,
        [Display(Name = "Débito")]
        Debito = 2
    }

    /*  public enum EnumFormaPagamento : int
        {
            Boleto,
            [Display(Name = "Cartão de Crédito")]
            CartaoCredito,
            [Display(Name = "Cartão de Débito")]
            CartaoDebito,
            [Display(Name = "Cheque")]
            Cheque,
            ConstruCard,
            [Display(Name = "Débito em Conta")]
            DebitoEmConta,
            Dinheiro,
            Financiamento
        } */
    #endregion

    //[Display(ResourceType = typeof(Resources.Anexo.Model), Name = "FileName_DisplayName")]
}