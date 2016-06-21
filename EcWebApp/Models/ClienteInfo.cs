using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace EcWebApp.Models
{
    [Table("Clientes", Schema = "orc")]
    public class ClienteInfo
    {
        [Key]
        public Guid? IdCliente { get; set; }

        [Display(Name = "Nome Completo / Razão Social")]
        [StringLength(255), Required(ErrorMessage = "Informe o nome do cliente")]
        public string NomeCliente { get; set; }

        [StringLength(255), DataType(DataType.EmailAddress)]
        public string EmailCliente { get; set; }

        [Display(Name = "Celular")]
        [StringLength(20), Required(ErrorMessage = "Informe um telefone para contato")]
        public string Telefone01 { get; set; }

        [Display(Name = "Telefone Residencial"), StringLength(20)]
        public string Telefone02 { get; set; }

        public bool EnderecosIguais { get; set; }

        #region Documentação..
        [Display(Name = "Data de Nascimento"), DataType(DataType.DateTime)]
        public DateTime? DataNascimento { get; set; }

        [Display(Name = "CPF / CNPJ")]
        public string CPFCNPJ { get; set; }

        [Display(Name = "RG / I.E.")]
        public string Documento { get; set; }

        [Display(Name = "Profissão")]
        public string Profissao { get; set; }
        #endregion

        #region Status..
        [Display(Name = "Interesse"), Required(ErrorMessage = "Selecione um Interesse")]
        public EnumInteresse Interesse { get; set; }

        [Display(Name = "Status"), Required(ErrorMessage = "Selecione um Status")]
        public int IdStatus { get; set; }

        [Display(Name = "Status da Planta")]
        public EnumStatusPlanta? StatusPlanta { get; set; }

        [Display(Name = "Data da Apresentação"), DataType(DataType.DateTime)]
        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}", ApplyFormatInEditMode = true)]
        public DateTime? DataApresentacao { get; set; }

        [Display(Name = "Data da Medição"), DataType(DataType.DateTime)]
        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}", ApplyFormatInEditMode = true)]
        public DateTime? DataMedicao { get; set; }
        #endregion

        #region Atendimento..
        [Display(Name = "Último Contato"), DataType(DataType.DateTime)]
        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}", ApplyFormatInEditMode = true)]
        public DateTime? DataUltimoContato { get; set; }

        [Display(Name = "Próximo Contato"), DataType(DataType.DateTime)]
        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}", ApplyFormatInEditMode = true)]
        public DateTime? DataProximoContato { get; set; }

        [Display(Name = "Comentários"), DataType(DataType.MultilineText)]
        [StringLength(500)]
        public string Comentarios { get; set; }
        #endregion

        #region Procedencia..
        public string Empreendimento { get; set; }

        [Display(Name = "Procedência")]
        public EnumProcedencia Procedencia { get; set; }

        [Display(Name = "Especifique")]
        public string OutraProcedencia { get; set; }
        #endregion

        #region Vendedor..
        [Display(Name = "Valor Aprox. do Projeto"), DataType(DataType.Currency)]
        public Decimal? ValorEstimadoProjeto { get; set; }

        [Display(Name = "Prazo de Entrega")]
        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}", ApplyFormatInEditMode = true)]
        public DateTime? PrazoEntrega { get; set; }

        [Display(Name = "Vendedor"), Required(ErrorMessage ="Selecione um Vendedor")]
        public Guid IdVendedor { get; set; }

        [Display(Name = "Data do Cadastro"), DataType(DataType.DateTime)]
        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}", ApplyFormatInEditMode = true)]
        public DateTime DataCadastro { get; set; }
        #endregion

        #region Propriedades de Navegação..
        [ForeignKey("IdVendedor")]
        public UsuarioInfo Vendedor { get; set; }

        [ForeignKey("IdStatus")]
        public StatusAtendimentoInfo StatusAtendimento { get; set; }

        public IList<EnderecoInfo> Enderecos { get; set; }
        #endregion
    }

    [Table("Enderecos", Schema = "orc")]
    public class EnderecoInfo
    {
        [Key]
        public Guid? IdEndereco { get; set; }

        public Guid IdCliente { get; set; }

        [Display(Name = "Tipo do Endereço")]
        public EnumTipoEndereco TipoEndereco { get; set; }

        [Display(Name = "Endereço"), StringLength(255)]
        public string Endereco { get; set; }

        [Display(Name = "Complemento"), StringLength(100)]
        public string Complemento { get; set; }

        [Display(Name = "Bairro"), StringLength(100)]
        public string Bairro { get; set; }

        [Display(Name = "Cidade"), StringLength(100)]
        public string Cidade { get; set; }

        [Display(Name = "Estado"), StringLength(2)]
        public string Estado { get; set; }

        [Display(Name = "CEP"), StringLength(10)]
        public string CEP { get; set; }

        [Display(Name = "Observação"), StringLength(500)]
        public string Observacao { get; set; }

        [ForeignKey("IdCliente")]
        public ClienteInfo Cliente { get; set; }
    }
}