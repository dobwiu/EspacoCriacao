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
    }
}