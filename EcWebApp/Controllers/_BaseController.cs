using System;
using System.Web.Mvc;

namespace EcWebApp.Controllers
{
    public class _BaseController : Controller
    {
        public const string strMsgErro = "Não foi possivel salvar o registro: ";

        public Guid IdUsuario
        {
            get { return Guid.Parse(Request.Cookies["UserSettings"]["IdUsuario"]); }
        }

        public string LoginUsuario
        {
            get { return Request.Cookies["UserSettings"]["LoginUsuario"].ToString(); }
        }

        public string NomeUsuario
        {
            get { return Request.Cookies["UserSettings"]["NomeUsuario"].ToString(); }
        }

        //public PermissaoInfo Permissoes { get; set; }

        public static string FormataTelefone(string telefone)
        {
            if (string.IsNullOrEmpty(telefone)) { return string.Empty; }

            string novoTelefone = telefone.Replace("_", "");
            int posicaoTraco = novoTelefone.IndexOf('-');
            if (novoTelefone.Length == 14 && posicaoTraco > 9)
            {
                novoTelefone = novoTelefone.Substring(0, posicaoTraco - 1) + "-"
                             + novoTelefone.Substring(posicaoTraco - 1).Replace("-", "");
            }
            return novoTelefone;
        }
    }
}
