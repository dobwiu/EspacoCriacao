using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace EcWebApp.Models
{
    [Table("Atendimentos", Schema = "orc")]
    public class AtendimentoInfo
    {
        [Key]
        public Guid IdAtendimento { get; set; }

        [Required]
        public Guid IdCliente { get; set; }

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

        [Display(Name = "Data do Próximo Contato"), NotMapped]
        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}", ApplyFormatInEditMode = true)]
        public DateTime? DataProximoContato { get; set; }

        [Display(Name = "Comentários")]
        [StringLength(500), DataType(DataType.MultilineText)]
        public string Comentario { get; set; }

        [Display(Name = "Vendedor")]
        public Guid IdVendedor { get; set; }

        [Display(Name = "Data de Atendimento"), DataType(DataType.DateTime)]
        public DateTime DataAtendimento { get; set; }

        public string RegistradoPor { get; set; }

        #region Propriedades de Navegação..
        [ForeignKey("IdStatus")]
        public StatusAtendimentoInfo Status { get; set; }

        [ForeignKey("IdCliente")]
        public ClienteInfo Cliente { get; set; }

        [ForeignKey("IdVendedor")]
        public UsuarioInfo Vendedor { get; set; }
        #endregion

    }
}