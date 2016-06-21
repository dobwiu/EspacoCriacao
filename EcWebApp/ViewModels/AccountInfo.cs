using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace EcWebApp.ViewModels
{
    /// <summary>
    /// Entidade para Autenticação no Sistema
    /// </summary>
    public class LogonInfo
    {
        [MaxLength(50), Required]
        public string UserName { get; set; }

        [DataType(DataType.Password)]
        [MaxLength(50), Required]
        public string Password { get; set; }

        [Display(Name = "Lembre me")]
        public bool RememberMe { get; set; }
    }

    /// <summary>
    /// Entidade para a troca de senha do Usuário
    /// </summary>
    public class ChangePasswordInfo
    {
        [Required]
        public Guid IdUsuario { get; set; }

        [Display(Name = "Nome do Usuário")]
        public string NomeUsuario { get; set; }

        [DataType(DataType.Password), Required]
        [Display(Name = "Senha Atual")]
        public string OldPassword { get; set; }

        [DataType(DataType.Password), Required]
        [Display(Name = "Nova Senha")]
        [MaxLength(50), MinLength(5)]
        public string NewPassword { get; set; }

        [Compare("NewPassword", ErrorMessage = "As senhas não são idênticas")]
        [DataType(DataType.Password), Required]
        [Display(Name = "Confirme a Nova Senha")]
        [MaxLength(50), MinLength(5)]
        public string ConfirmPassword { get; set; }
    }
}