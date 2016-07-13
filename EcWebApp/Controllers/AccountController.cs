using EcWebApp.BLL;
using EcWebApp.Models;
using EcWebApp.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;

namespace EcWebApp.Controllers
{
    [AllowAnonymous]
    public class AccountController : _BaseController
    {
        // GET: Account
        [HttpGet, AllowAnonymous]
        public ActionResult LogOn()
        {
            return View();
        }

        // POST: Account
        [HttpPost, AllowAnonymous]
        public ActionResult LogOn(LogonInfo pessoa, string ReturnUrl)
        {
            string msgErro = string.Empty;

            if (ModelState.IsValid)
            {
                var bllUsuario = new Usuario();
                var usuario = bllUsuario.Autenticar(pessoa.UserName, pessoa.Password);

                if (usuario != null)
                {
                    FormsAuthentication.SetAuthCookie(usuario.IdUsuario.ToString(), pessoa.RememberMe);
                    this.CriaCookieUsuario(usuario);

                    if (string.IsNullOrEmpty(ReturnUrl))
                    {
                        return Redirect("~/");
                    }
                    else
                    {
                        if (Url.IsLocalUrl(ReturnUrl))
                        {
                            return Redirect("~" + ReturnUrl);
                        }
                        else
                        {
                            return Redirect(ReturnUrl);
                        }
                    }
                }
                else
                {
                    msgErro = "Usuário ou Senha inválido";
                }
            }
            else
            {
                msgErro = "Informe seus dados";
            }

            ViewBag.MsgErro = msgErro;
            return View("LogOn");
        }

        [HttpGet, AllowAnonymous]
        public ActionResult ChangePassword(Guid id)
        {
            var usuario = new Usuario().SelecionaUsuario(id);
            var trocaSenha = new ChangePasswordInfo()
            {
                IdUsuario = usuario.IdUsuario.Value,
                NomeUsuario = usuario.NomeUsuario
            };

            return View(trocaSenha);
        }

        public ActionResult ChangePassword(ChangePasswordInfo novaSenha)
        {

            if (ModelState.IsValid)
            {
                var bllUsuario = new Usuario();
                var senhaAtual = bllUsuario.SelecionaUsuario(novaSenha.IdUsuario);

                if (senhaAtual.SenhaUsuario == bllUsuario.CalculateMD5Hash(novaSenha.NewPassword))
                {
                    bllUsuario.RedefinirSenha(novaSenha.IdUsuario, novaSenha.NewPassword);
                    return RedirectToAction("Index", "Home");
                }
            }

            ViewBag.MsgErro = "Não foi possível trocar a senha!";
            return View(novaSenha);
        }

        /// <summary>
        /// Cria o cookie da sessão do usuário 
        /// </summary>
        private void CriaCookieUsuario(UsuarioInfo pUsuario)
        {
            Response.Cookies["UserSettings"]["IdUsuario"] = pUsuario.IdUsuario.Value.ToString();
            Response.Cookies["UserSettings"]["LoginUsuario"] = pUsuario.LoginUsuario;
            Response.Cookies["UserSettings"]["NomeUsuario"] = pUsuario.NomeUsuario;

            Response.Cookies["UserSettings"]["Relatorios"] = pUsuario.Permissoes.Relatorios.ToString(); // EnumNivelAcesso.SemAcesso.ToString(); 
            Response.Cookies["UserSettings"]["Financeiro"] = pUsuario.Permissoes.Financeiro.ToString(); // EnumNivelAcesso.SemAcesso.ToString(); 
            Response.Cookies["UserSettings"]["Administrativo"] = pUsuario.Permissoes.Administrativo.ToString();

            Response.Cookies["UserSettings"].Expires = DateTime.Now.AddDays(1d);
        }

        /// <summary>
        /// Desloga o usuário do sistema
        /// </summary>
        public void LogOff()
        {
            FormsAuthentication.SignOut();
            Session.Abandon();

            //Força a exclusão do cookie de sessão para garantir que aquela sessão não vale mais para nenhum aplicativo
            Response.SetCookie(new HttpCookie("ASP.NET_SessionId", ""));
            Response.Redirect(FormsAuthentication.LoginUrl);
        }
    }
}