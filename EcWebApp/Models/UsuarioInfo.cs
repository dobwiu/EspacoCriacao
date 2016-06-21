using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace EcWebApp.Models
{
    [Table("Usuarios")]
    public class UsuarioInfo
    {
        [Key]
        public Guid? IdUsuario { get; set; }

        [Display(Name = "Nome do Usuário")]
        [StringLength(100), Required(ErrorMessage = "Campo Nome do Usuário obrigatório")]
        public string NomeUsuario { get; set; }

        [Display(Name = "Email do Usuário"), StringLength(255)]
        public string EmailUsuario { get; set; }

        [Index("UK_LoginAcesso", IsUnique = true)]
        [Display(Name = "Login de Acesso")]
        [StringLength(50), Required(ErrorMessage = "Campo Login de Acesso obrigatório")]
        public string LoginUsuario { get; set; }

        [Display(Name = "Senha de Acesso"), DataType(DataType.Password)]
        [StringLength(50, MinimumLength = 5), Required(ErrorMessage = "Campo Senha de Acesso obrigatório. (Min de 5 caracteres)")]
        public string SenhaUsuario { get; set; }

        [Display(Name = "Confirme a Senha"), DataType(DataType.Password), NotMapped]
        [Compare("SenhaUsuario", ErrorMessage = "As senhas não são idênticas")]
        public string SenhaConfirmada { get; set; }

        public bool Ativo { get; set; }

        public virtual PermissaoInfo Permissoes { get; set; }

    }

    [Table("Permissoes")]
    public class PermissaoInfo
    {
        //public Guid IdPermissao { get; set; }
        [Key, ForeignKey("Usuario")]
        public Guid IdUsuario { get; set; }

        public UsuarioInfo Usuario { get; set; }

        [Display(Name = "Módulo de Vendas")]
        public EnumNivelAcesso Vendas { get; set; }

        [Display(Name = "Relatórios")]
        public EnumNivelAcesso Relatorios { get; set; }

        //-- Contas a Pagar/Receber -->
        [Display(Name = "Módulo Financeiro")]
        public EnumNivelAcesso Financeiro { get; set; }

        [Display(Name = "Módulo Adminstrativo")]
        public EnumNivelAcesso Administrativo { get; set; }

    }
}